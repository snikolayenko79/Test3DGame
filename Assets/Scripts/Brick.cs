using UnityEngine;

public interface IDamageable
{
    void TakeDamage();
}

public class Brick : MonoBehaviour, IDamageable
{
    public void TakeDamage()
    {
        // Здесь можно вызвать эффект частиц или звук перед смертью
        Destroy(gameObject);
    }
}