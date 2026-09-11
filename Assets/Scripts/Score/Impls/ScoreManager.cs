using System;

public class ScoreManager : IScoreAdder, IScoreReader
{
    private int _currentScore = 0;

    public event Action<int> OnScoreChanged;
    public int CurrentScore => _currentScore;

    public void AddScore(int points)
    {
        if (points <= 0) return;

        _currentScore += points;
        OnScoreChanged?.Invoke(_currentScore);
    }

    public void ResetScore()
    {
        _currentScore = 0;
        OnScoreChanged?.Invoke(_currentScore);
    }
}