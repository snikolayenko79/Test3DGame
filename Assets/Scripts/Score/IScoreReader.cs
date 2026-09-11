using UnityEngine;
using System;

public interface IScoreReader
{
    int CurrentScore { get; }
    event Action<int> OnScoreChanged;
}
