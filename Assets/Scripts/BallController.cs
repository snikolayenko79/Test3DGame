using UnityEngine;
using Zenject;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallCollisionHandler collisionHandler;
    private BallMovement ballMovement;
    private PaddleController paddle;
    private IInputEventSource inputSource;
    private TargetRegistry _targetRegistry;
    
    // Флаг, который определяет: летит мяч или еще прилип к платформе
    private bool isLaunched = false;
    
    // Внедряем зависимости через Zenject
    [Inject]
    public void Construct(BallMovement movement, PaddleController paddleInstance, IInputEventSource input, TargetRegistry registry)
    {
        this.ballMovement = movement;
        this.paddle = paddleInstance;
        this.inputSource = input;
        _targetRegistry = registry;
    }
    
    private void OnEnable()
    {
        collisionHandler.OnBallCollision += HandleCollision;
        if (paddle != null)
            paddle.OnBallHitPaddle += RedirectBall;
        
        // Подписываемся на событие клика/действия из глобального ввода
        if (inputSource != null)
            inputSource.OnActionTriggered += TryLaunchBall;
        
        // Подписываемся на событие победы
        if (_targetRegistry != null)
            _targetRegistry.OnAllTargetsDestroyed += HandleVictory;
    }

    private void OnDisable()
    {
        collisionHandler.OnBallCollision -= HandleCollision;
        if (paddle != null)
            paddle.OnBallHitPaddle -= RedirectBall;
        
        // Подписываемся на событие победы
        if (_targetRegistry != null)
            _targetRegistry.OnAllTargetsDestroyed -= HandleVictory;
    }

    private void Start()
    {
        
    }
    
    private void TryLaunchBall()
    {
        // Если мяч уже запущен, повторные клики ничего не делают
        if (isLaunched) return;

        isLaunched = true;
        
        // Задаем направление полета вверх и немного вбок.
        Vector3 launchDirection = new Vector3(5f, 0f, 10f);
        
        ballMovement.Launch(launchDirection);
        
        Debug.Log("Мяч успешно запущен по клику/пробелу!");
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
    
    private void HandleVictory()
    {
        Debug.Log("🏆 ПОБЕДА! Все мишени на сцене уничтожены!");
        // Здесь можно включить UI экран победы или перезапустить сцену
    }
}