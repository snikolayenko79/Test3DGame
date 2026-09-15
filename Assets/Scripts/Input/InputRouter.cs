using System;
using System.Collections.Generic;
using Zenject;
using UnityEngine;

public class InputRouter : IInitializable, IDisposable
{
    private readonly List<IInputEventSource> _inputs;
    private readonly List<IHorizontalMovable> _movables;
    private readonly IActionTriggerable _action;

    private bool _isPaused = false;

    // Zenject автоматически соберет все зарегистрированные объекты в эти списки!
    // Инпуты прилетят из ProjectContext, а платформы и мячи — из SceneContext.
    public InputRouter(
        List<IInputEventSource> inputs,
        List<IHorizontalMovable> movables,
        IActionTriggerable action)
    {
        _inputs = inputs;
        _movables = movables;
        _action = action;
    }

    public void Initialize()
    {
        // Находим реальное количество игроков по минимальному размеру списков
        int playersCount = Mathf.Min(_inputs.Count, _movables.Count);
        
        Debug.Log(playersCount);
        
        for (int i = 0; i < playersCount; i++)
        {
            var input = _inputs[i];
            var movable = _movables[i];

            // Динамически связываем их через анонимные методы (лямбды)
            input.OnHorizontalMovementChanged += (dir) => 
            {
                if (!_isPaused) movable?.SetMoveDirection(dir);
            };

            input.OnActionTriggered += () => 
            {
                if (!_isPaused) _action?.ExecuteAction();
            };
        }

        //Debug.Log($"InputRouter: Успешно запущена динамическая шина для {playersCount} игроков!");
    }

    public void SetPause(bool pauseState)
    {
        _isPaused = pauseState;
        if (_isPaused)
        {
            foreach (var movable in _movables)
            {
                movable?.SetMoveDirection(0f);
            }
        }
    }

    public void Dispose()
    {
        // Логика отписки в цикле аналогично Initialize (опущена для краткости)
    }
}