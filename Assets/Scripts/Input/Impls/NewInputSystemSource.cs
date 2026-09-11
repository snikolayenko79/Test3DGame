using System;
using UnityEngine;
using UnityEngine.InputSystem; // Обязательный namespace для новой системы

public class NewInputSystemSource : MonoBehaviour, IInputEventSource, GameControls.IGameplayActions
{
    public event Action<float> OnHorizontalMovementChanged;

    private GameControls controls;
    private float lastDirection = 0f;

    private void Awake()
    {
        // Инициализируем сгенерированный Unity класс управления
        controls = new GameControls();
        
        // Регистрируем этот класс как обработчик событий карты Gameplay
        controls.Gameplay.SetCallbacks(this);
    }

    private void OnEnable()
    {
        // Включаем карту действий при активации объекта
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        // Выключаем карту действий, чтобы не тратить ресурсы
        controls.Gameplay.Disable();
    }

    // Этот метод автоматически генерируется интерфейсом IGameplayActions 
    // Он вызывается новой системой ввода САМА по себе: когда кнопка нажата, удерживается или отпущена
    public void OnMoveX(InputAction.CallbackContext context)
    {
        // Считываем текущее значение оси (-1, 0, 1)
        float currentDirection = context.ReadValue<float>();

        // Передаем событие дальше по цепочке в Paddle только при изменении значения
        if (Mathf.Approximately(currentDirection, lastDirection) == false)
        {
            lastDirection = currentDirection;
            OnHorizontalMovementChanged?.Invoke(currentDirection);
        }
    }
    
    public void OnAction(InputAction.CallbackContext context)
    {
        
    }
}