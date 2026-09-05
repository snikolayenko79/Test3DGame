using UnityEngine;

public interface IDamageable
{
    void TakeDamage();
}

public class Brick : MonoBehaviour, IBallHitResponder
{
    public void HandleBallHit(Rigidbody ballRigidbody, ContactPoint contactPoint)
    {
        TakeDamage();
    }
    
    private void TakeDamage()
    {
        // Здесь можно вызвать эффект частиц или звук перед смертью
        Destroy(gameObject);
    }
}