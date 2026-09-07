using UnityEngine;

public interface IBallHitResponder
{
    // Каждый объект сам решает, как реагировать на мяч
    void HandleBallHit(ContactPoint contactPoint);
}