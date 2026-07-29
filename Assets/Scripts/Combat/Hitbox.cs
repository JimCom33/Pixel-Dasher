using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Hitbox : MonoBehaviour
{
    [SerializeField, Min(0)] private int damage = 25;
    [SerializeField] private Vector2 offset = new Vector2(1f, 0f);
    [SerializeField] private Vector2 size = new Vector2(1.5f, 1f);
    [SerializeField] private Vector2 knockback = new Vector2(6f, 2f);
    [SerializeField] private LayerMask targetLayers = ~0;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Called by an animation event on the active sword-swing frame.
    public void PerformAttack()
    {
        float facingDirection = spriteRenderer.flipX ? -1f : 1f;
        Vector2 center = (Vector2)transform.position
            + new Vector2(offset.x * facingDirection, offset.y);
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(center, size, 0f, targetLayers);
        HashSet<IDamageable> damagedTargets = new();

        foreach (Collider2D overlap in overlaps)
        {
            foreach (MonoBehaviour behaviour in overlap.GetComponentsInParent<MonoBehaviour>())
            {
                if (behaviour is not IDamageable damageable
                    || behaviour.transform.root == transform.root
                    || !damagedTargets.Add(damageable))
                {
                    continue;
                }

                Vector2 directedKnockback = new Vector2(
                    knockback.x * facingDirection,
                    knockback.y);
                damageable.TakeDamage(new DamageData(damage, directedKnockback, gameObject));
                Vector3 popupPosition = new Vector3(
                    overlap.bounds.center.x,
                    overlap.bounds.max.y + 0.25f,
                    overlap.transform.position.z);
                DamagePopup.Spawn(popupPosition, damage);

                Rigidbody2D targetBody = overlap.attachedRigidbody;
                if (targetBody != null && targetBody.bodyType == RigidbodyType2D.Dynamic)
                {
                    targetBody.AddForce(directedKnockback, ForceMode2D.Impulse);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        SpriteRenderer currentRenderer = spriteRenderer != null
            ? spriteRenderer
            : GetComponent<SpriteRenderer>();
        float facingDirection = currentRenderer != null && currentRenderer.flipX ? -1f : 1f;
        Vector2 center = (Vector2)transform.position
            + new Vector2(offset.x * facingDirection, offset.y);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
