using UnityEngine; // Обязательно для Resources.Load
using Zenject;

public class ProjectInputInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameSettings>().AsSingle().NonLazy();
        
        // 1. Принудительно загружаем физические префабы из папки Resources методами самой Unity
        GameObject inputPrefab1 = Resources.Load<GameObject>("InputP1");
        GameObject inputPrefab2 = Resources.Load<GameObject>("InputP2");

        // Защита от дурака: если вы ошиблись в имени файла, консоль сразу об этом скажет!
        if (inputPrefab1 == null) Debug.LogError("Zenject: Не удалось найти префаб InputP1 в папке Resources!");
        if (inputPrefab2 == null) Debug.LogError("Zenject: Не удалось найти префаб InputP2 в папке Resources!");

        // Регистрируем с ID, но БЕЗ .NonLazy(). 
        // Объект создастся на сцене DontDestroyOnLoad только тогда, когда мы его попросим.
        Container.Bind<IInputEventSource>().WithId("P1").To<AdvancedInputSourceWithActionsName>().FromComponentInNewPrefab(inputPrefab1).AsCached();
        Container.Bind<IInputEventSource>().WithId("P2").To<AdvancedInputSourceWithActionsName>().FromComponentInNewPrefab(inputPrefab2).AsCached();
    }
}