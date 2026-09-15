using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdvancedInputSourceWithActionsName : MonoBehaviour, IInputEventSource
{
    public event Action<float> OnHorizontalMovementChanged;
    public event Action OnActionTriggered;

    [Header("Input Setup")]
    [SerializeField] private PlayerInput playerInput; 
    
    // В эти поля в инспекторе мы просто впишем "MoveP1"/"ActionP1" или "MoveP2"/"ActionP2"
    [SerializeField] private string moveActionName = "MoveP1";
    [SerializeField] private string actionActionName = "ActionP1";

    private InputAction moveAction;
    private InputAction actionAction;
    private float lastDirection = 0f;

    private void Awake()
    {
        // Динамически находим нужные экшены по их именам
        moveAction = playerInput.actions.FindAction(moveActionName);
        actionAction = playerInput.actions.FindAction(actionActionName);
        
        //Debug.Log("Input awake");
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.performed += OnMovePerformed;
            moveAction.canceled += OnMoveCanceled;
        }
        if (actionAction != null)
        {
            actionAction.started += OnActionStarted;
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
        }
        if (actionAction != null)
        {
            actionAction.started -= OnActionStarted;
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        float dir = context.ReadValue<float>();
        if (!Mathf.Approximately(dir, lastDirection))
        {
            lastDirection = dir;
            OnHorizontalMovementChanged?.Invoke(dir);
            //Debug.Log("Input OnMove");
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        lastDirection = 0f;
        OnHorizontalMovementChanged?.Invoke(0f);
    }

    private void OnActionStarted(InputAction.CallbackContext context)
    {
        //Debug.Log("Input OnAction");
        OnActionTriggered?.Invoke();
    }
}