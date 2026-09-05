using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallMovement ballMovement;
    [SerializeField] private BallCollisionHandler collisionHandler;
    private Rigidbody ballRigidbody;

    private void Awake()
    {
        ballRigidbody = ballMovement.GetComponent<Rigidbody>();
    }

    private void OnEnable() => collisionHandler.OnBallCollision += HandleCollision;
    private void OnDisable() => collisionHandler.OnBallCollision -= HandleCollision;

    private void Start()
    {
        // Запускаем мяч (для XZ плоскости)
        ballMovement.Launch(new Vector3(5f, 0f, 10f));
    }
    
    private void HandleCollision(Collision collision)
    {
        // Берем первую точку контакта для точности физики
        ContactPoint contact = collision.GetContact(0);

        // Ищем любой компонент, который умеет реагировать на удар мяча
        if (collision.gameObject.TryGetComponent<IBallHitResponder>(out var responder))
        {
            // Просто передаем управление самому объекту
            responder.HandleBallHit(ballRigidbody, contact);
        }
    }
}