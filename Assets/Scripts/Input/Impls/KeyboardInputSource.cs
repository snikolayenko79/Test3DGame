using UnityEngine;
using System;

public class KeyboardInputSource : MonoBehaviour, IInputEventSource
{
    // Событие передает направление движения (-1, 0, 1) каждый раз, когда оно меняется
    public event Action<float> OnHorizontalMovementChanged;

    private float lastInput = 0f;
    
    private void Update()
    {
        float currentInput = Input.GetAxisRaw("Horizontal");

        // Генерируем событие только если значение изменилось (например, нажали или отпустили кнопку)
        if (Mathf.Approximately(currentInput, lastInput) == false)
        {
            lastInput = currentInput;
            OnHorizontalMovementChanged?.Invoke(currentInput);
        }
    }
}