using UnityEngine;
using Zenject;

public interface IDamageable
{
    void TakeDamage();
}

public class Brick : MonoBehaviour, IBallHitResponder, IGameplayTarget
{
    [SerializeField] private int scoreValue = 100; // Сколько очков дает этот кирпич
    
    private TargetRegistry _registry;
    private IScoreAdder _scoreAdder;

    [Inject]
    public void Construct(TargetRegistry registry, IScoreAdder  scoreAdder)
    {
        _registry = registry;
        _scoreAdder = scoreAdder;
    }

    private void Start()
    {
        // Кирпич появился на сцене (или заспавнился) -> сам записался в реестр
        _registry.Register(this);
    }
    
    // Реализация интерфейса IGameplayTarget
    public void Hit()
    {
        // Здесь можно уменьшать жизни кирпича, если он прочный. 
        // Но пока что ломаем с одного удара:
        OnBrickDestroyed();
    }

    private void OnBrickDestroyed()
    {
        // Перед уничтожением выписываем себя из реестра мишеней
        _registry.Unregister(this);
        
        Debug.Log($"Кирпич {gameObject.name} уничтожен!");
        Destroy(gameObject);
        
        _scoreAdder.AddScore(scoreValue);
    }

    // Реализация интерфейса отскока мяча из прошлых шагов.
    // Когда мяч бьется о кирпич, кирпич сам реагирует на удар!
    public void HandleBallHit(ContactPoint contactPoint)
    {
        Hit();
    }
}