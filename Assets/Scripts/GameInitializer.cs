using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private PaddleController paddle;
    
    private void Start()
    {
        // Ищем компонент ввода на сцене (или создаем его)
        IInputEventSource input = paddle.GetComponent<IInputEventSource>();
        if (input != null)
        {
            paddle.InitializeInput(input);
        }
    }
}