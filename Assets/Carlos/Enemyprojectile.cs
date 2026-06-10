using UnityEngine;

/// <summary>
/// Behaviour for projectiles fired by EnemyShooter.
/// Attach this to the projectile prefab.
/// Requires: Rigidbody2D
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;

    [Header("Effects")]
    public GameObject hitEffectPrefab;  // optional particle/sprite on impact

    void OnCollisionEnter2D(Collision2D col)
    {
        // Deal damage to the player on contact
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerHealth ph = col.gameObject.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }

        // Spawn impact effect if assigned
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // Do not destroy if it hits another enemy
        if (col.gameObject.CompareTag("Enemy")) return;

        Destroy(gameObject);
    }
}