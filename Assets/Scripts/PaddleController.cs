using UnityEngine;
using System;
using Unity.Netcode; // Подключаем NGO только для RPC
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PaddleController : NetworkBehaviour, IHorizontalMovable, IBallHitResponder
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float acceleration = 50f; 
    [SerializeField] private float deceleration = 40f; 
    [SerializeField] private float movementLimit = 7f;
    
    [Header("Visual Settings")]
    [SerializeField] private MeshRenderer meshRenderer; 
    [SerializeField] private Color activeColor = Color.green;    
    [SerializeField] private Color inactiveColor = Color.gray;  
    
    // В инспекторе у Левой платформы выберите Player1, у Правой — Player2
    [SerializeField] private PlayerMode selectedMode;

    private float targetDirection = 0f; 
    private float currentHorizontalSpeed = 0f; 
    
    private Rigidbody rb;
    private GameSettings gameSettings;

    [Inject]
    public void Construct(GameSettings _gameSettings)
    {
        gameSettings = _gameSettings;
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        // К моменту Start Zenject гарантированно внес настройки из меню.
        // Каждая доска на ЛОКАЛЬНОМ компьютере красит себя сама на основе выбора в меню!
        bool bSelected = gameSettings.SelectedMode == selectedMode;
        SetVisualState(bSelected);
        rb.isKinematic = !bSelected;
    }
    
    public void SetVisualState(bool isActive)
    {
        if (meshRenderer != null) meshRenderer.material.color = isActive ? activeColor : inactiveColor;
    }
    
    public void SetMoveDirection(float direction)
    {
        // Локальный инпут: управляем только той доской, которую выбрали в меню
        if (gameSettings.SelectedMode != selectedMode) return; 
        targetDirection = direction;
    }
    
    private void FixedUpdate()
    {
        // Если эта доска НЕ выбрана в меню на этом компьютере, мы ЕЙ НЕ УПРАВЛЯЕМ.
        // Её координаты будут плавно прилетать по сети через RPC от второго игрока!
        if (gameSettings.SelectedMode != selectedMode)
            return;

        // --- ВАШ ОРИГИНАЛЬНЫЙ ФИЗИЧЕСКИЙ РАСЧЕТ ДВИЖЕНИЯ ---
        float targetSpeed = targetDirection * maxSpeed;
        float currentSpeedFactor = (Mathf.Approximately(targetSpeed, 0f)) ? deceleration : acceleration;

        currentHorizontalSpeed = Mathf.MoveTowards(
            currentHorizontalSpeed, 
            targetSpeed, 
            currentSpeedFactor * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(currentHorizontalSpeed, 0f, 0f);

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -movementLimit, movementLimit);
        
        if (Mathf.Approximately(clampedPosition.x, -movementLimit) || Mathf.Approximately(clampedPosition.x, movementLimit))
        {
            currentHorizontalSpeed = 0f;
        }

        rb.MovePosition(clampedPosition);

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            // --- СЕТЕВОЙ СИНХРОН ---
            // Так как мы сдвинули НАШУ доску физикой, мы отправляем её новые координаты по сети!
            if (IsServer)
            {
                // Если мы Хост — отправляем координаты Клиенту
                SyncPositionClientRpc(transform.position);
            }
            else if (IsClient)
            {
                // Если мы Клиент — отправляем координаты Хосту (через Сервер)
                SyncPositionServerRpc(transform.position);
            }
        }
    }

    // КЛИЕНТ отправляет координаты СЕРВЕРУ
    //[ServerRpc(RequireOwnership = false)] устарело
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SyncPositionServerRpc(Vector3 newPosition)
    {
        // Сервер принимает координаты от Клиента и двигает правую доску у себя на экране
        transform.position = newPosition;

        // И сразу же пересылает эти координаты ВСЕМ остальным (если бы игроков было больше)
        SyncPositionClientRpc(newPosition);
    }

    // СЕРВЕР рассылает координаты ВСЕМ КЛИЕНТАМ
    //[ClientRpc] устарело
    [Rpc(SendTo.ClientsAndHost)]
    private void SyncPositionClientRpc(Vector3 newPosition)
    {
        // Если этот компьютер НЕ управляет этой доской (она чужая для него), 
        // он просто плавно перемещает её туда, куда приказала сеть!
        if (gameSettings.SelectedMode != selectedMode)
        {
            transform.position = newPosition;
        }
    }

    public event Action<float> OnBallHitPaddle;
    public void HandleBallHit(ContactPoint contactPoint)
    {
        if (!IsServer)
            return;
        
        float hitPoint = transform.position.x - contactPoint.point.x;
        OnBallHitPaddle?.Invoke(hitPoint);
    }
}