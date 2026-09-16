using UnityEngine;
using UnityEngine.SceneManagement; // Для загрузки игровой сцены
using UnityEngine.UI;
using Zenject;

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
        leftPaddleButton.onClick.AddListener(() => StartGame(PlayerMode.Player1));
        rightPaddleButton.onClick.AddListener(() => StartGame(PlayerMode.Player2));
    }

    private void StartGame(PlayerMode mode)
    {
        // 1. Записываем выбор игрока в глобальные настройки
        _settings.SelectedMode = mode;

        // 2. Загружаем игровую сцену (замените "GameScene" на имя вашей сцены)
        SceneManager.LoadScene("Arkanoid");
    }
}