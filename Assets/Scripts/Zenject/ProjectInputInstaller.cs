using UnityEngine; // Обязательно для Resources.Load
using Zenject;

public class ProjectInputInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // 1. Принудительно загружаем физические префабы из папки Resources методами самой Unity
        GameObject inputPrefab1 = Resources.Load<GameObject>("InputP1");
        GameObject inputPrefab2 = Resources.Load<GameObject>("InputP2");

        // Защита от дурака: если вы ошиблись в имени файла, консоль сразу об этом скажет!
        if (inputPrefab1 == null) Debug.LogError("Zenject: Не удалось найти префаб InputP1 в папке Resources!");
        if (inputPrefab2 == null) Debug.LogError("Zenject: Не удалось найти префаб InputP2 в папке Resources!");

        // 2. Биндим Игрока 1 через явный приказ создать Prefab в памяти
        Container.Bind<IInputEventSource>()
            .To<AdvancedInputSourceWithActionsName>()
            .FromComponentInNewPrefab(inputPrefab1) // Явно отдаем загруженный Unity-объект
            .UnderTransformGroup("GlobalManagers")   // Упаковываем в красивую папку в иерархии
            .AsCached()
            .NonLazy(); // Теперь этот NonLazy сработает на 100%, так как Zenject видит физический префаб

        // 3. Биндим Игрока 2
        Container.Bind<IInputEventSource>()
            .To<AdvancedInputSourceWithActionsName>()
            .FromComponentInNewPrefab(inputPrefab2)
            .UnderTransformGroup("GlobalManagers")
            .AsCached()
            .NonLazy();
    }
}