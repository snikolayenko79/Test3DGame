using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [Header("Scene Instances")]
    [SerializeField] private PaddleController paddleInstance;
    [SerializeField] private BallMovement ballInstance; // Ссылка на мяч, который стоит на сцене

    public override void InstallBindings()
    {
        Container.Bind<TargetRegistry>().AsSingle();
        
        // Магия Zenject: один менеджер закроет потребности и IScoreAdder, и IScoreReader
        Container.BindInterfacesTo<ScoreManager>().AsSingle();
        
        // Привязываем платформу со сцены
        Container.Bind<PaddleController>()
            .FromInstance(paddleInstance)
            .AsSingle();

        // Важнейшая строчка, которую ругает ошибка:
        // Регистрируем компонент движения мяча со сцены
        Container.Bind<BallMovement>()
            .FromInstance(ballInstance)
            .AsSingle();
    }
}