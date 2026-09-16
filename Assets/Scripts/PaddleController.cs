using UnityEngine;
using System;
using Zenject;

public class PaddleController : MonoBehaviour, IHorizontalMovable, IBallHitResponder
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float acceleration = 50f; // Сила разгона
    [SerializeField] private float deceleration = 40f; // Сила торможения (инерция)
    [SerializeField] private float movementLimit = 7f;
    
    private MeshRenderer meshRenderer; // Ссылка на компонент отображения
    [SerializeField] private Color activeColor = Color.green;    // Цвет активного игрока
    [SerializeField] private Color inactiveColor = Color.gray;  // Цвет пассивного игрока
    
    [SerializeField] private PlayerMode selectedMode;

    private float targetDirection = 0f; // Куда игрок ХОЧЕТ двигаться (-1, 0, 1)
    private float currentHorizontalSpeed = 0f; // Текущая плавная скорость платформы
    
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
        meshRenderer = GetComponent<MeshRenderer>();
        
        // Вроде бы нельзя писать здесь. Awake может вызываться до Construct. Перенесено в Start.
        //SetVisualState(gameSettings.SelectedMode == selectedMode);
    }
    
    private void Start()
    {
        // Перенесли логику сюда. К моменту вызова Start Zenject ГАРАНТИРОВАННО 
        // внесет gameSettings, и мы избежим NullReferenceException!
        if (gameSettings != null)
        {
            SetVisualState(gameSettings.SelectedMode == selectedMode);
        }
    }
    
    public void SetVisualState(bool isActive)
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = isActive ? activeColor : inactiveColor;
        }
    }
    
    // Реализуем метод интерфейса IHorizontalMovable
    public void SetMoveDirection(float direction)
    {
        targetDirection = direction;
    }

    private void FixedUpdate()
    {
        // 1. Считаем целевую скорость, к которой мы стремимся
        float targetSpeed = targetDirection * maxSpeed;

        // 2. Выбираем, что использовать: силу разгона или силу торможения
        // Если игрок отпустил кнопку (targetSpeed == 0), плавно тормозим с силой deceleration.
        // Если игрок жмет кнопку, плавно разгоняемся с силой acceleration.
        float currentSpeedFactor = (Mathf.Approximately(targetSpeed, 0f)) ? deceleration : acceleration;

        // 3. Плавно приближаем текущую скорость к целевой с учетом физического дельта-времени
        currentHorizontalSpeed = Mathf.MoveTowards(
            currentHorizontalSpeed, 
            targetSpeed, 
            currentSpeedFactor * Time.fixedDeltaTime
        );

        // 4. Применяем получившуюся плавную скорость к Rigidbody
        rb.linearVelocity = new Vector3(currentHorizontalSpeed, 0f, 0f);

        // 5. Ограничиваем позицию платформы у стен, чтобы она не вылетала за экран
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -movementLimit, movementLimit);
        
        // Если уперлись в стену — сбрасываем накопленную скорость, чтобы не было "залипания"
        if (Mathf.Approximately(clampedPosition.x, -movementLimit) || Mathf.Approximately(clampedPosition.x, movementLimit))
        {
            currentHorizontalSpeed = 0f;
        }

        rb.MovePosition(clampedPosition);
    }
    // Событие передает координату X точки удара относительно центра платформы
    public event Action<float> OnBallHitPaddle;
    
    public void HandleBallHit(ContactPoint contactPoint)
    {
        float hitPoint = transform.position.x - contactPoint.point.x;

        // Просто сообщаем миру: "В нас ударились вот в этой точке"
        OnBallHitPaddle?.Invoke(hitPoint);
    }
}