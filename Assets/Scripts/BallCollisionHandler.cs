using UnityEngine;
using System;

public class BallCollisionHandler : MonoBehaviour
{
    // Событие: сообщаем всем вокруг, обо что мы ударились
    public event Action<Collision> OnBallCollision;

    private void OnCollisionEnter(Collision collision)
    {
        OnBallCollision?.Invoke(collision);
    }
}