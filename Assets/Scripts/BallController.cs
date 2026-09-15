using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class BallController : MonoBehaviour, IActionTriggerable
{
    [SerializeField] private BallCollisionHandler collisionHandler;
    // Ссылка на компонент движения, которую мы перетащим вручную в инспекторе
    [SerializeField] private BallMovement ballMovement;
    private TargetRegistry _targetRegistry;
    
    // Флаг, который определяет: летит мяч или еще прилип к платформе
    private bool isLaunched = false;
    
    private List<PaddleController> _allPaddles; // Список всех платформ в игре
    
    // Внедряем зависимости через Zenject
    [Inject]
    public void Construct(List<PaddleController> paddles, TargetRegistry registry)
    {
        this._allPaddles = paddles;
        _targetRegistry = registry;
    }
    
    private void OnEnable()
    {
        collisionHandler.OnBallCollision += HandleCollision;
        
        // Подписываемся на события отскока от обеих платформ
        if (_allPaddles != null)
        {
            foreach (var paddle in _allPaddles)
            {
                paddle.OnBallHitPaddle += RedirectBall;
            }
        }
        
        // Подписываемся на событие победы
        if (_targetRegistry != null)
            _targetRegistry.OnAllTargetsDestroyed -= HandleVictory;
    }

    private void OnDisable()
    {
        collisionHandler.OnBallCollision -= HandleCollision;
        
        // Подписываемся на события отскока от обеих платформ
        if (_allPaddles != null)
        {
            foreach (var paddle in _allPaddles)
            {
                paddle.OnBallHitPaddle += RedirectBall;
            }
        }
        
        // Подписываемся на событие победы
        if (_targetRegistry != null)
            _targetRegistry.OnAllTargetsDestroyed -= HandleVictory;
    }

    private void Start()
    {
        
    }
    
    // Реализуем метод интерфейса IActionTriggerable
    public void ExecuteAction()
    {
        TryLaunchBall(); // Запускаем мяч
    }
    
    private void TryLaunchBall()
    {
        // Если мяч уже запущен, повторные клики ничего не делают
        if (isLaunched) return;

        isLaunched = true;
        
        // Задаем направление полета вверх и немного вбок.
        Vector3 launchDirection = new Vector3(5f, 0f, 10f);
        
        ballMovement.Launch(launchDirection);
        //Debug.Log("Мяч успешно запущен по клику/пробелу!");
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