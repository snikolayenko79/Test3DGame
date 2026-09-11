using UnityEngine;
using TMPro; // Обязательно для работы с TextMeshPro
using Zenject; // Подключаем Zenject

[RequireComponent(typeof(TextMeshProUGUI))]
public class BricksCounterUI : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    private TargetRegistry _targetRegistry;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Zenject автоматически внедрит сюда наш синглтон реестра мишеней
    [Inject]
    public void Construct(TargetRegistry registry)
    {
        _targetRegistry = registry;
    }

    private void OnEnable()
    {
        if (_targetRegistry != null)
        {
            // Подписываемся на изменение количества блоков
            _targetRegistry.OnTargetsCountChanged += UpdateVisualText;
            
            // Сразу же принудительно обновляем текст текущим стартовым значением
            UpdateVisualText(_targetRegistry.GetRemainingTargetsCount());
        }
    }

    private void OnDisable()
    {
        if (_targetRegistry != null)
        {
            _targetRegistry.OnTargetsCountChanged -= UpdateVisualText;
        }
    }

    private void UpdateVisualText(int remainingBricks)
    {
        // Обновляем строчку на экране
        _textMesh.text = $"Блоков осталось: {remainingBricks}";
    }
}