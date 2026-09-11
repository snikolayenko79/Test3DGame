using UnityEngine;
using TMPro;
using Zenject;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    private IScoreReader _scoreReader;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    [Inject]
    public void Construct(IScoreReader scoreReader)
    {
        _scoreReader = scoreReader;
    }

    private void OnEnable()
    {
        if (_scoreReader != null)
        {
            // Подписываемся на изменение счета
            _scoreReader.OnScoreChanged += UpdateScoreText;
            
            // Сразу выставляем стартовое значение (0)
            UpdateScoreText(_scoreReader.CurrentScore);
        }
    }

    private void OnDisable()
    {
        if (_scoreReader != null)
        {
            _scoreReader.OnScoreChanged -= UpdateScoreText;
        }
    }

    private void UpdateScoreText(int currentScore)
    {
        Debug.Log("Updating score on display");
        _textMesh.text = $"Очки: {currentScore}";
    }
}