using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField, Min(0)] private int damage = 10;

    private void OnTriggerStay2D(Collider2D other)
    {
        foreach (MonoBehaviour behaviour in other.GetComponentsInParent<MonoBehaviour>())
        {
            if (behaviour is IDamageable damageable
                && behaviour.transform.root != transform.root)
            {
                damageable.TakeDamage(new DamageData(damage, Vector2.zero, gameObject));
                return;
            }
        }
    }
}
