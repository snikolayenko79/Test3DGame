using UnityEngine;

public class Ball : MonoBehaviour
{
    public float constantSpeed = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Запускаем мяч вверх и немного вбок под углом
        Vector3 launchDirection = new Vector3(0.5f, 0f, -1f).normalized;
        rb.linearVelocity = launchDirection * constantSpeed; // В старых версиях Unity: rb.velocity
    }

    void FixedUpdate()
    {
        // Физика 3D может замедлять мяч из-за погрешностей. 
        // Этот код поддерживает скорость мяча строго постоянной.
        rb.linearVelocity = rb.linearVelocity.normalized * constantSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Если столкнулись с кирпичом — уничтожаем его
        if (collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
        }
        
        // Хитрый трюк для изменения угла отскока от платформы
        // if (collision.gameObject.GetComponent<PaddleController>() != null)
        // {
        //     // Считаем расстояние от центра платформы до точки удара мяча
        //     float hitPoint = collision.transform.position.x - transform.position.x;
        //     // Корректируем направление движения мяча по оси X
        //     Vector3 currentVelocity = rb.linearVelocity;
        //     currentVelocity.x = -hitPoint * 2f; 
        //     rb.linearVelocity = currentVelocity;
        // }
    }
}