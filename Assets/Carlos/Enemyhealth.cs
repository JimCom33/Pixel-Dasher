using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Enemy health system.
/// Handles receiving damage, hit feedback, stomp detection, and death.
/// Attach to the same GameObject as EnemyAI.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Hit Feedback")]
    public float hitFlashDuration = 0.1f;
    public Color hitColor = Color.red;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDur = 0.15f;

    [Header("Death Drop")]
    public GameObject dropPrefab;   // coin, power-up, etc.
    public int scoreValue = 100;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private EnemyAI ai;

    private bool isKnockedBack;
    private float knockbackTimer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        ai = GetComponent<EnemyAI>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Count down the knockback lock so the AI resumes control afterwards
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
                isKnockedBack = false;
        }
    }

    // ──────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────

    /// <summary>
    /// Apply damage and push the enemy away from the damage source.
    /// </summary>
    /// <param name="damage">Amount of health to remove.</param>
    /// <param name="damageSourcePos">World position of the hit origin (used for knockback direction).</param>
    public void TakeDamage(int damage, Vector2 damageSourcePos)
    {
        if (ai != null && ai.currentState == EnemyAI.EnemyState.Dead) return;

        currentHealth -= damage;

        ApplyKnockback(damageSourcePos);
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
            Die();
    }

    /// <summary>
    /// Apply damage without knockback (e.g. lava, poison tiles).
    /// </summary>
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position);
    }

    // ──────────────────────────────────────────
    //  Private helpers
    // ──────────────────────────────────────────

    void ApplyKnockback(Vector2 source)
    {
        if (rb == null) return;

        Vector2 dir = ((Vector2)transform.position - source).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);

        isKnockedBack = true;
        knockbackTimer = knockbackDur;
    }

    System.Collections.IEnumerator FlashRed()
    {
        Color original = sr.color;
        sr.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        sr.color = original;
    }

    void Die()
    {
        // Award score
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
            scoreManager.AddScore(scoreValue);

        // Spawn drop item
        if (dropPrefab != null)
            Instantiate(dropPrefab, transform.position, Quaternion.identity);

        // Delegate death animation to EnemyAI
        if (ai != null)
            ai.Die();
        else
            Destroy(gameObject, 0.3f);
    }

    // ──────────────────────────────────────────
    //  Collision — stomp detection
    // ──────────────────────────────────────────

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        // Check whether the player is landing on top of the enemy (Mario stomp)
        float playerFoot = col.transform.position.y - col.collider.bounds.extents.y;
        float enemyTop = transform.position.y + GetComponent<Collider2D>().bounds.extents.y;

        bool stompedFromAbove = playerFoot > (enemyTop - 0.2f);

        if (stompedFromAbove)
        {
            // Player stomped the enemy — deal 1 damage and bounce the player upward
            TakeDamage(1, col.transform.position);

            Rigidbody2D playerRb = col.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 8f);
        }
        else
        {
            // Side collision — enemy damages the player
            PlayerHealth playerHealth = col.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(1);
        }
    }
}
