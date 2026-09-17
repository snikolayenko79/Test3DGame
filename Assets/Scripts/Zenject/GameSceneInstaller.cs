using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [Header("Платформы игроков")]
    [SerializeField] private PaddleController paddle1;
    [SerializeField] private PaddleController paddle2;

    [Header("Один общий Мяч")]
    [SerializeField] private BallController generalBall;
    
    [SerializeField] private ScoreManager scoreManager;
    
    public override void InstallBindings()
    {
        Container.Bind<TargetRegistry>().AsSingle();
        Container.Bind<IScoreAdder>().FromInstance(scoreManager).AsCached();
        Container.Bind<IScoreReader>().FromInstance(scoreManager).AsCached();

        // МАГИЯ ZENJECT: Вместо Construct мы достаем настройки прямо из контейнера!
        // Zenject заглянет в ProjectContext и найдет там наш сохраненный GameSettings
        GameSettings gameSettings = Container.Resolve<GameSettings>();
        
        // Независимо от режима, мы всегда регистрируем ОБЕ платформы как PaddleController,
        // чтобы общий мяч знал их обе и правильно рассчитывал углы отскока!
        Container.Bind<PaddleController>().FromInstance(paddle1).AsCached();
        Container.Bind<PaddleController>().FromInstance(paddle2).AsCached();

        // Теперь switch работает на 100% стабильно и без сбоев
        switch (gameSettings.SelectedMode)
        {
            case PlayerMode.Player1:
                Container.Bind<IInputEventSource>().FromResolve("P1").AsCached();
                Container.Bind<IHorizontalMovable>().FromInstance(paddle1).AsCached();
                
                break;

            case PlayerMode.Player2:
                Container.Bind<IInputEventSource>().FromResolve("P2").AsCached();
                Container.Bind<IHorizontalMovable>().FromInstance(paddle2).AsCached();
                
                break;
        }

        // Регистрируем ОДИН общий мяч для всей сцены
        Container.Bind<IActionTriggerable>().FromInstance(generalBall).AsSingle();

        // Запускаем роутер
        Container.BindInterfacesAndSelfTo<InputRouter>().AsSingle().NonLazy();
    }
}