using UnityEngine;
using UnityEngine.SceneManagement; // Для загрузки игровой сцены
using UnityEngine.UI;
using Zenject;
using Unity.Netcode;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button leftPaddleButton;
    [SerializeField] private Button rightPaddleButton;

    private GameSettings _settings;

    [Inject]
    public void Construct(GameSettings settings)
    {
        _settings = settings;
    }

    private void OnEnable()
    {
        leftPaddleButton.onClick.AddListener(StartAsPlayerOne);
        rightPaddleButton.onClick.AddListener(StartAsPlayerTwo);
    }
    
    private void StartAsPlayerOne()
    {
        // 1. Записываем выбор в глобальные настройки
        _settings.SelectedMode = PlayerMode.Player1;

        if (NetworkManager.Singleton != null)
        {
            // 2. Запускаем ХОСТ (Сервер + Игрок 1)
            NetworkManager.Singleton.StartHost();
            Debug.Log("NGO: Хост запущен за Игрока 1!");

            // 3. СТРОГО НА ХОСТЕ: Приказываем сетевому менеджеру загрузить игровую сцену.
            // NGO сам загрузит эту сцену у Хоста и автоматически подтянет сюда второго игрока, когда тот подключится!
            // Замените "GameScene" на точное имя вашей сцены геймплея
            NetworkManager.Singleton.SceneManager.LoadScene("Arkanoid", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    private void StartAsPlayerTwo()
    {
        // 1. Записываем выбор в глобальные настройки
        _settings.SelectedMode = PlayerMode.Player2;

        if (NetworkManager.Singleton != null)
        {
            // 2. Запускаем КЛИЕНТ (Игрок 2)
            // Клиент просто подключается к серверу. Метод LoadScene ему вызывать НЕ НАДО!
            // Как только связь установится, NGO сам переключит экран Клиента на нужную сцену.
            NetworkManager.Singleton.StartClient();
            Debug.Log("NGO: Клиент пытается подключиться за Игрока 2...");
        }
    }
}