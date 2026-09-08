using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Глобальная точка доступа (Service Locator / Singleton)
    public static InputManager Instance { get; private set; }

    // Предоставляем наружу только чистый интерфейс
    public IInputEventSource GameplayInput => InputSource;

    [SerializeField] private NewInputSystemSource InputSource;

    private void Awake()
    {
        // Настройка синглтона для простого доступа из любой точки игры
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Менеджер не уничтожится при смене уровней
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Менеджер может централизованно отключать ввод (для пауз, катсцен, экранов проигрыша)
    public void EnableGameplayInput() => InputSource.enabled = true;
    public void DisableGameplayInput() => InputSource.enabled = false;
}