using System;
using System.Collections.Generic;

public class TargetRegistry
{
    private readonly List<IGameplayTarget> _targets = new();

    // Событие, которое оповестит игру о том, что все блоки уничтожены
    public event Action OnAllTargetsDestroyed;
    
    public event Action<int> OnTargetsCountChanged;

    public void Register(IGameplayTarget target)
    {
        if (!_targets.Contains(target))
        {
            _targets.Add(target);
            
            OnTargetsCountChanged?.Invoke(_targets.Count);
        }
    }

    public void Unregister(IGameplayTarget target)
    {
        if (_targets.Contains(target))
        {
            _targets.Remove(target);
            
            OnTargetsCountChanged?.Invoke(_targets.Count);
                
            // Если блоков на сцене больше нет — вызываем победу
            if (_targets.Count == 0)
            {
                OnAllTargetsDestroyed?.Invoke();
            }
        }
    }

    public int GetRemainingTargetsCount() => _targets.Count;
}