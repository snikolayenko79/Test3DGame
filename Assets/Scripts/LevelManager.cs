using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class LevelManager : NetworkBehaviour
{
    [Header("Список всех кирпичей на сцене")]
    [SerializeField] private List<Brick> allBricks = new();

    private TargetRegistry _targetRegistry;
    private IScoreAdder _scoreAdder;

    // Сетевой массив: хранит флаг ЖИВ/МЕРТВ для каждого кирпича
    // Он идеально синхронизируется при позднем подключении!
    private NetworkList<bool> bricksState;

    [Inject]
    public void Construct(TargetRegistry targetRegistry, IScoreAdder scoreAdder)
    {
        _targetRegistry = targetRegistry;
        _scoreAdder = scoreAdder;
    }

    private void Awake()
    {
        // НОВЫЙ СИНТАКСИС (без NetworkVariableSettings):
        // Передаем: null (начальный список пуст), права на чтение (Everyone), права на запись (Server)
        bricksState = new NetworkList<bool>(
            null, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server
        );

        // На старте автоматически нумеруем все кирпичи на сцене
        for (int i = 0; i < allBricks.Count; i++)
        {
            Brick currBrick = allBricks[i];
            currBrick.Initialize(i, this);
            _targetRegistry.Register(currBrick); // Регистрируем в реестре
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Подписываемся на изменения в сетевом списке кирпичей
        bricksState.OnListChanged += OnBricksStateNetworkChanged;

        // ЛОГИКА ДЛЯ ХОСТА (СЕРВЕРА)
        if (IsServer)
        {
            // Заполняем сетевой список значениями true (все кирпичи живы)
            foreach (var brick in allBricks)
            {
                bricksState.Add(true);
            }
        }

        // ВАЖНЕЙШИЙ ШАГ ДЛЯ LATE JOIN (ЛОГИКА ДЛЯ КЛИЕНТА):
        // Как только поздний клиент зашел, он пробегается по синхронизированному 
        // списку и мгновенно тушит те кирпичи, которые сервер отметил как false!
        if (IsClient)
        {
            for (int i = 0; i < bricksState.Count; i++)
            {
                if (!bricksState[i])
                {
                    Brick currBrick = allBricks[i];
                    currBrick.DisableBrick();
                    _targetRegistry.Unregister(currBrick);
                }
            }
        }
    }

    // Этот метод вызывает кирпич при ударе мяча. Работает строго на Сервере.
    public void DestroyBrickServer(int brickId, int points)
    {
        if (!IsServer)
            return;
        
        if (brickId < 0 || brickId >= bricksState.Count || !bricksState[brickId])
            return;

        // Сервер отмечает в сетевом списке, что данный кирпич мертв
        bricksState[brickId] = false; 

        // Начисляем очки на сервере (они синхронизируются через ScoreManager)
        _scoreAdder.AddScore(points);
    }

    private void OnBricksStateNetworkChanged(NetworkListEvent<bool> changeEvent)
    {
        // Когда в сети меняется статус какого-то кирпича (с true на false)
        if (changeEvent.Type == NetworkListEvent<bool>.EventType.Value && !changeEvent.Value)
        {
            int brickId = changeEvent.Index;
            
            // Тушим кирпич на экране и у Хоста, и у Клиента
            Brick currBrick = allBricks[brickId];
            currBrick.DisableBrick();
            _targetRegistry?.Unregister(currBrick);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        bricksState.OnListChanged -= OnBricksStateNetworkChanged;
    }

    // Кнопка в инспекторе, чтобы быстро собрать все кирпичи со сцены в список
    [ContextMenu("Собрать все кирпичи со сцены")]
    private void FindAllBricks()
    {
        allBricks = new List<Brick>(FindObjectsByType<Brick>(FindObjectsSortMode.None));
    }
}