using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 15f;
    public float movementLimit = 7f; // Ограничение, чтобы не выехать за стены

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector3 newPosition = transform.position + Vector3.right * moveInput * speed * Time.deltaTime;
        
        // Ограничиваем движение по оси X
        newPosition.x = Mathf.Clamp(newPosition.x, -movementLimit, movementLimit);
        transform.position = newPosition;
    }
}