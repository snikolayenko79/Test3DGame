using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallMovement ballMovement;
    [SerializeField] private BallCollisionHandler collisionHandler;

    private void OnEnable()
    {
        collisionHandler.OnBallCollision += HandleCollision;
    }

    private void OnDisable()
    {
        collisionHandler.OnBallCollision -= HandleCollision;
    }

    private void Start()
    {
        // Запускаем мяч (для XZ плоскости)
        ballMovement.Launch(new Vector3(5f, 0f, 10f));
    }

    private void HandleCollision(Collision collision)
    {
        // Проверяем, можно ли нанести урон объекту (DIP в действии - зависим от абстракции IDamageable, а не от класса Brick)
        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage();
        }

        // Логика изменения угла отскока от платформы
        if (collision.gameObject.TryGetComponent<PaddleController>(out var paddle))
        {
            //RedirectBallFromPaddle(collision.transform);
        }
    }

    private void RedirectBallFromPaddle(Transform paddleTransform)
    {
        float hitPoint = paddleTransform.position.x - ballMovement.transform.position.x;
        Vector3 currentVelocity = ballMovement.GetComponent<Rigidbody>().linearVelocity;
        currentVelocity.x = -hitPoint * 2f;
        
        ballMovement.Launch(currentVelocity);
    }
}