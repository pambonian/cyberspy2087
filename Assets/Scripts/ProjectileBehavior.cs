using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    // Time after which the projectile gets destroyed if it doesn't hit the player
    private float lifetime = 4f;

    private void Start()
    {
        // Destroy the projectile after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the projectile has collided with the player
        if (other.CompareTag("Player"))
        {
            // Here, handle what happens when the projectile hits the player.
            // For example, reduce the player's health.

            // Destroy the projectile after hitting the player
            Destroy(gameObject);
        }
    }
}
