using UnityEngine;

public class PaddleController : MonoBehaviour, IBallHitResponder
{
    public float speed = 15f;
    public float movementLimit = 7f; // Ограничение, чтобы не выехать за стены
    [SerializeField] private float RedirectInfluence = 2f;
    
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector3 newPosition = transform.position + Vector3.right * moveInput * speed * Time.deltaTime;
        
        // Ограничиваем движение по оси X
        newPosition.x = Mathf.Clamp(newPosition.x, -movementLimit, movementLimit);
        transform.position = newPosition;
    }
    
    public void HandleBallHit(Rigidbody ballRigidbody, ContactPoint contactPoint)
    {
        // Считаем смещение от центра платформы
        float hitPoint = transform.position.x - ballRigidbody.transform.position.x;
        
        Vector3 currentVelocity = ballRigidbody.linearVelocity;
        currentVelocity.x = -hitPoint * RedirectInfluence;
        
        // Пересчитываем скорость через компонент движения мяча
        if (ballRigidbody.TryGetComponent<BallMovement>(out var movement))
        {
            movement.Launch(currentVelocity);
        }
    }
}