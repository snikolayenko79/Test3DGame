using UnityEngine;
using System;

public class PaddleController : MonoBehaviour, IBallHitResponder
{
    public float speed = 15f;
    public float movementLimit = 7f; // Ограничение, чтобы не выехать за стены
    private float currentDirection = 0f;
    private IInputEventSource inputSource;

    private bool bInputInitialized = false;
    
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    public void InitializeInput(IInputEventSource source)
    {
        // Отписываемся от старого источника, если он был
        if (inputSource != null)
            inputSource.OnHorizontalMovementChanged -= UpdateDirection;

        inputSource = source;
        
        // Подписываемся на события нового ввода
        inputSource.OnHorizontalMovementChanged += UpdateDirection;
        
        bInputInitialized = true;
    }
    
    private void UpdateDirection(float direction)
    {
        currentDirection = direction;
    }

    private void FixedUpdate()
    {
        // Двигаем платформу через Rigidbody (физический способ)
        Vector3 newVelocity = new Vector3(currentDirection * speed, 0f, 0f);
        rb.linearVelocity = newVelocity; // В старых версиях: rb.velocity

        // Ограничиваем позицию у стен
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -movementLimit, movementLimit);
        rb.MovePosition(clampedPosition);
    }
    
    private void OnDestroy()
    {
        if (inputSource != null && bInputInitialized)
            inputSource.OnHorizontalMovementChanged -= UpdateDirection;
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