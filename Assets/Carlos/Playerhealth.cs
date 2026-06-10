using UnityEngine;

/// <summary>
/// Basic player health system.
/// Referenced by EnemyHealth to apply contact damage.
/// Expand this class to fit your game's needs.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth = 3;

    [Header("Invincibility Frames")]
    public float invincibleDuration = 1.5f;
    private bool isInvincible;

    // Subscribe to these events from your UI or GameManager
    public event System.Action<int> OnHealthChanged;
    public event System.Action OnDeath;

    /// <summary>
    /// Reduce the player's health by <paramref name="amount"/>.
    /// Ignored during invincibility frames.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Debug.Log("[PlayerHealth] Player died.");
            // Notify your GameManager or reload the scene here
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    /// <summary>
    /// Restore health, clamped to maxHealth.
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// Blink the sprite for the duration of the invincibility window
    System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float elapsed = 0f;

        while (elapsed < invincibleDuration)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        if (sr != null) sr.enabled = true;
        isInvincible = false;
    }
}