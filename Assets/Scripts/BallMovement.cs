using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void FixedUpdate()
    {
        // Поддерживаем скорость стабильной
        if (rb.linearVelocity != Vector3.zero)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
    }
}