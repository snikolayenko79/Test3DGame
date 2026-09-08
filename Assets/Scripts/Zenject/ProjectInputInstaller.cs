using UnityEngine;
using Zenject;

public class ProjectInputInstaller : MonoInstaller
{
    // Ссылка на префаб нашего продвинутого ввода (AdvancedInputSource)
    [SerializeField] private NewInputSystemSource InputPrefab;

    public override void InstallBindings()
    {
        // Биндим ввод как синглтон на уровне ВСЕГО проекта.
        // Используем BindInterfacesTo, чтобы объект автоматически привязался 
        // и к своему классу, и к интерфейсу IGameplayInputSource.
        Container.BindInterfacesTo<NewInputSystemSource>()
            .FromComponentInNewPrefab(InputPrefab)
            .UnderTransformGroup("GlobalManagers")
            .AsSingle();
            
        Debug.Log("Zenject: Глобальный ввод успешно зарегистрирован в ProjectContext!");
    }
}