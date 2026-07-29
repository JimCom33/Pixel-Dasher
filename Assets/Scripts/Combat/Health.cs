using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField, Min(0f)] private float invulnerabilityDuration;
    [SerializeField, Min(0.02f)] private float flashInterval = 0.1f;

    private float currentHealth;
    private bool isInvulnerable;
    private SpriteRenderer spriteRenderer;
    private Color normalColor;

    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent onDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            normalColor = spriteRenderer.color;
        }

        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(DamageData damageData)
    {
        if (isInvulnerable || currentHealth <= 0f)
        {
            return;
        }

        currentHealth -= damageData.damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (invulnerabilityDuration > 0f)
        {
            StartCoroutine(InvulnerabilityRoutine(damageData.source));
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Die() 
    {
        onDeath?.Invoke();
        gameObject.SetActive(false);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    private IEnumerator InvulnerabilityRoutine(GameObject damageSource)
    {
        isInvulnerable = true;
        List<ColliderPair> ignoredCollisions = IgnoreCollisionsWith(damageSource);
        float elapsedTime = 0f;
        bool faded = false;

        while (elapsedTime < invulnerabilityDuration)
        {
            faded = !faded;
            if (spriteRenderer != null)
            {
                Color color = normalColor;
                color.a = faded ? 0.3f : 1f;
                spriteRenderer.color = color;
            }

            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }

        RestoreCollisions(ignoredCollisions);
        isInvulnerable = false;
    }

    private List<ColliderPair> IgnoreCollisionsWith(GameObject damageSource)
    {
        List<ColliderPair> ignoredCollisions = new List<ColliderPair>();
        if (damageSource == null)
        {
            return ignoredCollisions;
        }

        Collider2D[] ownColliders = GetComponentsInChildren<Collider2D>();
        Collider2D[] sourceColliders = damageSource.transform.root.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D ownCollider in ownColliders)
        {
            foreach (Collider2D sourceCollider in sourceColliders)
            {
                Physics2D.IgnoreCollision(ownCollider, sourceCollider, true);
                ignoredCollisions.Add(new ColliderPair(ownCollider, sourceCollider));
            }
        }

        return ignoredCollisions;
    }

    private void RestoreCollisions(List<ColliderPair> ignoredCollisions)
    {
        foreach (ColliderPair pair in ignoredCollisions)
        {
            if (pair.first != null && pair.second != null)
            {
                Physics2D.IgnoreCollision(pair.first, pair.second, false);
            }
        }
    }

    private readonly struct ColliderPair
    {
        public readonly Collider2D first;
        public readonly Collider2D second;

        public ColliderPair(Collider2D first, Collider2D second)
        {
            this.first = first;
            this.second = second;
        }
    }
}
