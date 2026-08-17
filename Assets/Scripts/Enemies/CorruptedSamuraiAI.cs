using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
[RequireComponent(typeof(Health))]
public class CorruptedSamuraiAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float patrolDistance = 3f;
    [SerializeField, Min(0f)] private float patrolSpeed = 1.5f;
    [SerializeField, Min(0f)] private float chaseSpeed = 2.5f;
    [SerializeField, Min(0f)] private float detectionRange = 6f;

    [Header("Attack")]
    [SerializeField, Min(0f)] private float attackRange = 1.5f;
    [SerializeField, Min(0)] private int attackDamage = 15;
    [SerializeField, Min(0f)] private float attackWindup = 0.25f;
    [SerializeField, Min(0f)] private float attackCooldown = 1f;

    [Header("Reactions")]
    [SerializeField, Min(0f)] private float hitStunDuration = 0.25f;
    [SerializeField, Min(0f)] private float deathDelay = 0.85f;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Health health;
    private Collider2D bodyCollider;
    private Transform target;
    private Vector2 spawnPosition;
    private float patrolDirection = 1f;
    private float previousHealth;
    private float hitStunRemaining;
    private bool isAttacking;
    private bool isDead;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        bodyCollider = GetComponent<Collider2D>();
        spawnPosition = body.position;
    }

    private void OnEnable()
    {
        health.onHealthChanged.AddListener(OnHealthChanged);
        health.onDeath.AddListener(OnDeath);
    }

    private void Start()
    {
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        target = player != null ? player.transform : null;
        previousHealth = health.GetCurrentHealth();
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.onHealthChanged.RemoveListener(OnHealthChanged);
            health.onDeath.RemoveListener(OnDeath);
        }
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        hitStunRemaining = Mathf.Max(0f, hitStunRemaining - Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (isDead || isAttacking || hitStunRemaining > 0f)
        {
            animator.SetBool(IsMoving, false);
            return;
        }

        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(body.position, target.position);
            if (distanceToTarget <= attackRange)
            {
                animator.SetBool(IsMoving, false);
                StartCoroutine(AttackRoutine());
                return;
            }

            if (distanceToTarget <= detectionRange)
            {
                MoveTowards(target.position.x, chaseSpeed);
                return;
            }
        }

        Patrol();
    }

    private void Patrol()
    {
        float destinationX = spawnPosition.x + patrolDistance * patrolDirection;
        if (Mathf.Abs(body.position.x - destinationX) < 0.1f)
        {
            patrolDirection *= -1f;
            destinationX = spawnPosition.x + patrolDistance * patrolDirection;
        }

        MoveTowards(destinationX, patrolSpeed);
    }

    private void MoveTowards(float destinationX, float speed)
    {
        float direction = Mathf.Sign(destinationX - body.position.x);
        Vector2 nextPosition = body.position + Vector2.right * direction * speed * Time.fixedDeltaTime;
        body.MovePosition(nextPosition);
        spriteRenderer.flipX = direction < 0f;
        animator.SetBool(IsMoving, true);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool(IsMoving, false);
        animator.SetTrigger(Attack);

        if (target != null)
        {
            spriteRenderer.flipX = target.position.x < transform.position.x;
        }

        yield return new WaitForSeconds(attackWindup);

        if (!isDead && target != null
            && Vector2.Distance(body.position, target.position) <= attackRange + 0.25f)
        {
            foreach (MonoBehaviour behaviour in target.GetComponentsInParent<MonoBehaviour>())
            {
                if (behaviour is IDamageable damageable)
                {
                    damageable.TakeDamage(new DamageData(attackDamage, Vector2.zero, gameObject));
                    break;
                }
            }
        }

        yield return new WaitForSeconds(Mathf.Max(0f, attackCooldown - attackWindup));
        isAttacking = false;
    }

    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        if (currentHealth < previousHealth && currentHealth > 0f)
        {
            animator.SetTrigger(Hit);
            hitStunRemaining = hitStunDuration;
        }

        previousHealth = currentHealth;
    }

    private void OnDeath()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        StopAllCoroutines();
        animator.SetBool(IsMoving, false);
        animator.SetBool(IsDead, true);
        body.linearVelocity = Vector2.zero;
        if (bodyCollider != null)
        {
            bodyCollider.enabled = false;
        }

        Destroy(gameObject, deathDelay);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? (Vector3)spawnPosition : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            center + Vector3.left * patrolDistance,
            center + Vector3.right * patrolDistance);
    }
}
