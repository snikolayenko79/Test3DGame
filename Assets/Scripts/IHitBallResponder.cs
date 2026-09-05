using UnityEngine;

public interface IBallHitResponder
{
    // Каждый объект сам решает, как реагировать на мяч
    void HandleBallHit(Rigidbody ballRigidbody, ContactPoint contactPoint);
}