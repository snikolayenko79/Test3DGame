using UnityEngine;
using System;

public class PaddleController : MonoBehaviour, IBallHitResponder
{
    public float speed = 15f;
    public float movementLimit = 7f; // Ограничение, чтобы не выехать за стены
    [SerializeField] private float RedirectInfluence = 2f;
    
    // Событие передает координату X точки удара относительно центра платформы
    public event Action<float> OnBallHitPaddle;
    
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector3 newPosition = transform.position + Vector3.right * moveInput * speed * Time.deltaTime;
        
        // Ограничиваем движение по оси X
        newPosition.x = Mathf.Clamp(newPosition.x, -movementLimit, movementLimit);
        transform.position = newPosition;
    }

    public void HandleBallHit(ContactPoint contactPoint)
    {
        float hitPoint = transform.position.x - contactPoint.point.x;

        // Просто сообщаем миру: "В нас ударились вот в этой точке"
        OnBallHitPaddle?.Invoke(hitPoint);
    }
}