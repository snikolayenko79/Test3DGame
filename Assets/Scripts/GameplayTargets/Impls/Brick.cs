using UnityEngine;
using Zenject;
using Unity.Netcode;

public class Brick : MonoBehaviour, IBallHitResponder, IGameplayTarget
{
    [SerializeField] private int scorePoints = 10; 
    
    // Уникальный номер кирпича на сцене (0, 1, 2, 3...)
    public int BrickId { get; set; } 

    private LevelManager _levelManager;

    // Передаем ссылку на менеджер уровня
    public void Initialize(int id, LevelManager levelManager)
    {
        BrickId = id;
        _levelManager = levelManager;
    }

    // Этот метод вызывается общим мячом строго на Сервере/Хосте
    public void HandleBallHit(ContactPoint contactPoint)
    {
        Hit();
    }

    public void DisableBrick()
    {
        gameObject.SetActive(false);
    }
    
    // Реализация интерфейса IGameplayTarget
    public void Hit()
    {
        // Просто сообщаем менеджеру: "Кирпич с моим ID был уничтожен!"
        _levelManager?.DestroyBrickServer(BrickId, scorePoints);
    }
}