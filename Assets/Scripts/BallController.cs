using UnityEngine;
using Unity.Netcode;
using Zenject;
using System.Collections.Generic;

public class BallController : NetworkBehaviour, IActionTriggerable
{
    [SerializeField] private BallCollisionHandler collisionHandler;
    // Ссылка на компонент движения, которую мы перетащим вручную в инспекторе
    [SerializeField] private BallMovement ballMovement;
    private TargetRegistry _targetRegistry;
    
    // Флаг, который определяет: летит мяч или еще прилип к платформе
    private bool isLaunched = false;
    
    private List<PaddleController> _allPaddles; // Список всех платформ в игре
    
    private Rigidbody rb;
    
    // Внедряем зависимости через Zenject
    [Inject]
    public void Construct(List<PaddleController> paddles, TargetRegistry registry)
    {
        this._allPaddles = paddles;
        _targetRegistry = registry;
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // СЕТЕВАЯ НАСТРОЙКА ФИЗИКИ:
        if (!IsServer)
        {
            // На компьютере Клиента отключаем физику мяча. 
            // Он будет просто перемещаться по координатам сервера.
            if (rb != null)
                rb.isKinematic = true;
        }
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
    
    // Этот метод вызывается Роутером, когда КТО-УГОДНО из игроков нажимает кнопку действия
    public void ExecuteAction()
    {
        // СЕТЕВАЯ ПОДСТРАХОВКА: Если сеть еще не успела полностью запуститься 
        // и мяч не заспавнен в NGO — мы просто игнорируем нажатие и выходим!
        // if (!IsSpawned) 
        // {
        //     Debug.LogWarning("NGO: Мяч еще не готов в сети, подождите секунду.");
        //     return;
        // }

        // Железобетонная проверка: запущен ли этот экземпляр как Сервер/Хост?
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            // Если мы Хост — запускаем физический мяч напрямую
            LaunchBall();
        }
        // else
        // {
        //     // Если мы Клиент — отправляем безопасный запрос на сервер
        //     RequestLaunchServerRpc();
        // }
    }

    // [ServerRpc(RequireOwnership = false)]
    // private void RequestLaunchServerRpc()
    // {
    //     LaunchBall();
    // }
    
    private void LaunchBall()
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
    
    private void FixedUpdate()
    {
        // // ТОЛЬКО Сервер рассчитывает движение и транслирует его в сеть
        // if (!IsServer)
        //     return;
        //
        // // Отправляем текущую позицию мяча всем подключенным клиентам
        // SyncBallPositionClientRpc(transform.position);
    }

    // СЕРВЕР принудительно двигает мяч на экранах всех КЛИЕНТОВ
    // [ClientRpc]
    // private void SyncBallPositionClientRpc(Vector3 newPosition)
    // {
    //     // Клиент просто перемещает 3D-модель в пространстве
    //     if (!IsServer && IsOwner)
    //     {
    //         transform.position = newPosition;
    //     }
    // }
    
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