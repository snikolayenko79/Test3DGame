using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
    // Скорость настраивается ТОЛЬКО здесь
    [SerializeField] private float baseSpeed = 10f; 
    
    private Rigidbody rb;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = baseSpeed;
        
        // Мяч стоит на месте, пока мы не вызовем Launch
        rb.linearVelocity = Vector3.zero; 
    }

    public void Launch(Vector3 direction)
    {
        // .normalized гарантирует, что длина вектора равна 1, 
        // и мы умножаем её на внутреннюю скорость мяча
        rb.linearVelocity = direction.normalized * currentSpeed;
    }

    // Метод для изменения скорости извне (например, для бонусов)
    public void ModifySpeed(float multiplier)
    {
        currentSpeed = baseSpeed * multiplier;
        // Сразу обновляем текущую скорость физического тела, сохраняя направление
        if (rb.linearVelocity != Vector3.zero)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
        }
    }

    private void FixedUpdate()
    {
        // Поддерживаем скорость строго стабильной, основываясь ТОЛЬКО на currentSpeed
        if (rb.linearVelocity != Vector3.zero)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
        }
    }
}