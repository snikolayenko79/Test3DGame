using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseZone : MonoBehaviour, IBallHitResponder
{
    public void HandleBallHit(Rigidbody ballRigidbody, ContactPoint contactPoint)
    {
        // Мяч упал мимо платформы — перезапуск
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}