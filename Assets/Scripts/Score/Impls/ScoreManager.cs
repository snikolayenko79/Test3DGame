using System;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour, IScoreAdder, IScoreReader
{
    // Сетевая переменная: читать могут все (Everyone), писать — только Сервер (Server)
    // Она автоматически пересылает актуальное значение любому ПОЗДНО подключившемуся клиенту!
    private readonly NetworkVariable<int> _netScore = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public int CurrentScore => _netScore.Value;
    public event Action<int> OnScoreChanged;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Когда клиент подключается, он сразу подписывается на обновление этой переменной
        _netScore.OnValueChanged += (oldVal, newVal) =>
        {
            OnScoreChanged?.Invoke(newVal);
        };
        
        // Сразу при входе обновляем UI актуальным значением, которое прилетело с сервера
        OnScoreChanged?.Invoke(_netScore.Value);
    }
    
    public void AddScore(int points)
    {
        if (points <= 0) return;

        // Начислять очки в сетевую переменную имеет право только Сервер
        if (!IsServer)
            return;

        _netScore.Value += points;
    }

    public void ResetScore()
    {
        // Начислять очки в сетевую переменную имеет право только Сервер
        if (!IsServer)
            return;

        _netScore.Value = 0;
    }
}