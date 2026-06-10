using UnityEngine;

/// <summary>
/// Ranged enemy that fires projectiles in an arc (Lakitu / Hammer Bro style).
/// Can be combined with EnemyAI on the same GameObject, or used standalone.
/// </summary>
public class EnemyShooter : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;           // empty child Transform at the enemy's "mouth"
    public float fireInterval = 2f;  // seconds between shots
    public float projectileSpeed = 6f;

    [Header("Sinusoidal Movement (Lakitu)")]
    public bool sinusoidalMovement = false;
    public float sinAmplitude = 1.5f;
    public float sinFrequency = 1f;

    private float fireTimer;
    private float startY;
    private Transform player;

    void Start()
    {
        startY = transform.position.y;
        fireTimer = fireInterval;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (sinusoidalMovement)
            MoveSinusoidal();

        // Count down and fire on interval
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            Shoot();
            fireTimer = fireInterval;
        }
    }

    /// Bobs the enemy up and down on the Y axis
    void MoveSinusoidal()
    {
        float newY = startY + Mathf.Sin(Time.time * sinFrequency) * sinAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    /// Spawns a projectile aimed toward the player in a downward arc
    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Default direction: straight down
        Vector2 dir = Vector2.down;

        if (player != null)
        {
            // Arc toward the player: horizontal bias + downward component
            float deltaX = player.position.x - firePoint.position.x;
            dir = new Vector2(Mathf.Sign(deltaX) * 0.5f, -1f).normalized;
        }

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();

        if (projRb != null)
            projRb.linearVelocity = dir * projectileSpeed;

        // Safety destroy in case the projectile misses everything
        Destroy(proj, 4f);
    }
}
