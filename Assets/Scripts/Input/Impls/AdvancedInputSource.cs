using System;
using UnityEngine;
using UnityEngine.InputSystem; // Обязательный namespace для новой системы

public class AdvancedInputSource : MonoBehaviour, IInputEventSource, GameControls.IGameplayActions
{
    public event Action<float> OnHorizontalMovementChanged;
    public event Action OnActionTriggered;

    private GameControls controls;
    private float lastDirection = 0f;

    private void Awake()
    {
        // Инициализируем сгенерированный Unity класс управления
        controls = new GameControls();
        
        // Регистрируем этот скрипт как обработчик событий для карты "Gameplay"
        controls.Gameplay.SetCallbacks(this);
    }

    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();

    // Этот метод вызывается новой системой ввода автоматически при нажатии кнопок движения
    public void OnMoveX(InputAction.CallbackContext context)
    {
        // Считываем значение оси (-1, 0, 1)
        float currentDirection = context.ReadValue<float>();

        // Оповещаем платформу только если значение реально изменилось
        if (Mathf.Approximately(currentDirection, lastDirection) == false)
        {
            lastDirection = currentDirection;
            OnHorizontalMovementChanged?.Invoke(currentDirection);
        }
    }

    // Этот метод будет вызываться автоматически при клике мыши или нажатии Пробела
    public void OnAction(InputAction.CallbackContext context)
    {
        // context.started означает, что кнопка была только что НАЖАТА (а не отпущена)
        if (context.started)
        {
            // Вызываем событие клика (мяч его услышит и полетит)
            OnActionTriggered?.Invoke();
        }
    }
}