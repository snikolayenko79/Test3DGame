using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [Header("Платформы игроков")]
    [SerializeField] private PaddleController paddle1;
    [SerializeField] private PaddleController paddle2;

    [Header("Один общий Мяч")]
    [SerializeField] private BallController generalBall;

    public override void InstallBindings()
    {
        Container.Bind<TargetRegistry>().AsSingle();
        Container.BindInterfacesTo<ScoreManager>().AsSingle();

        // Регистрируем платформы по очереди (Zenject сам соберет их в List<IHorizontalMovable>)
        Container.Bind<IHorizontalMovable>().FromInstance(paddle1).AsCached();
        Container.Bind<IHorizontalMovable>().FromInstance(paddle2).AsCached();

        // Регистрируем ОДИН общий мяч для всей сцены
        Container.Bind<IActionTriggerable>().FromInstance(generalBall).AsSingle();

        // Запускаем роутер
        Container.BindInterfacesAndSelfTo<InputRouter>().AsSingle().NonLazy();
    }
}