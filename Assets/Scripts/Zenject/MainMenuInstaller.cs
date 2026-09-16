using UnityEngine;
using Zenject;

public class MainMenuInstaller : MonoInstaller
{
    [SerializeField] private MainMenuController mainMenuController;

    public override void InstallBindings()
    {
        Container.Bind<MainMenuController>().FromInstance(mainMenuController).AsSingle().NonLazy();
    }
}