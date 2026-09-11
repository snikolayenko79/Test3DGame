using UnityEngine;
using System;

public interface IInputEventSource
{
    // Событие передает направление движения (-1, 0, 1) каждый раз, когда оно меняется
    event Action<float> OnHorizontalMovementChanged;
    event Action OnActionTriggered;
}