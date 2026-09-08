using UnityEngine;
using Zenject;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallCollisionHandler collisionHandler;
    private BallMovement ballMovement;
    private PaddleController paddle;
    
    // Внедряем зависимости через Zenject метода-конструктора
    [Inject]
    public void Construct(BallMovement movement, PaddleController paddleInstance)
    {
        this.ballMovement = movement;
        this.paddle = paddleInstance;
    }
    
    private void OnEnable()
    {
        collisionHandler.OnBallCollision += HandleCollision;
        if (paddle != null)
            paddle.OnBallHitPaddle += RedirectBall;
    }

    private void OnDisable()
    {
        collisionHandler.OnBallCollision -= HandleCollision;
        if (paddle != null)
            paddle.OnBallHitPaddle -= RedirectBall;
    }

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
            responder.HandleBallHit(contact);
        }
    }
    
    private void RedirectBall(float hitPoint)
    {
        // Вся логика изменения скорости инкапсулирована здесь
        Vector3 currentVelocity = ballMovement.GetComponent<Rigidbody>().linearVelocity.normalized;
        currentVelocity.x = -hitPoint * 2f;
        
        ballMovement.Launch(currentVelocity);
    }
}