using UnityEngine;

/// <summary>
/// Main enemy AI script — Mario-style 2D platformer.
/// Attach this to the enemy GameObject.
/// Requires: Rigidbody2D, Collider2D, SpriteRenderer
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAI : MonoBehaviour
{
    // ──────────────────────────────────────────
    //  States
    // ──────────────────────────────────────────
    public enum EnemyState { Patrol, Chase, Attack, Dead }

    [Header("Initial State")]
    public EnemyState currentState = EnemyState.Patrol;

    // ──────────────────────────────────────────
    //  Movement
    // ──────────────────────────────────────────
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float jumpForce = 8f;
    public bool canJump = false;   // enable for enemies that jump over walls

    // ──────────────────────────────────────────
    //  Detection
    // ──────────────────────────────────────────
    [Header("Detection")]
    public float detectionRange = 5f;   // radius to detect the player
    public float loseRange = 8f;   // range at which the enemy loses sight
    public float attackRange = 1f;   // distance required to trigger an attack
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    // ──────────────────────────────────────────
    //  Patrol raycasts
    // ──────────────────────────────────────────
    [Header("Patrol Raycasts")]
    public float groundCheckDist = 0.6f;  // how far ahead to check for a ledge
    public float wallCheckDist = 0.4f;

    // ──────────────────────────────────────────
    //  Internal components
    // ──────────────────────────────────────────
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;         // optional
    private Transform player;

    private float direction = 1f;       // 1 = right, -1 = left
    private bool isGrounded;
    private bool isDead;

    // Collider dimensions (calculated automatically at runtime)
    private Vector2 colliderSize;
    private Vector2 colliderOffset;

    // ──────────────────────────────────────────
    //  Unity Lifecycle
    // ──────────────────────────────────────────
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();   // may be null — that's fine

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            colliderSize = col.bounds.size;
            colliderOffset = col.offset;
        }
    }

    void Start()
    {
        // Find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("[EnemyAI] No GameObject found with tag 'Player'.");
    }

    void Update()
    {
        if (isDead) return;

        CheckGrounded();
        UpdateState();
        ExecuteState();
        FlipSprite();
    }

    // ──────────────────────────────────────────
    //  State machine
    // ──────────────────────────────────────────

    /// Evaluates transition conditions and switches states accordingly
    void UpdateState()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                if (dist < detectionRange && CanSeePlayer())
                    ChangeState(EnemyState.Chase);
                break;

            case EnemyState.Chase:
                if (dist > loseRange)
                    ChangeState(EnemyState.Patrol);
                else if (dist <= attackRange)
                    ChangeState(EnemyState.Attack);
                break;

            case EnemyState.Attack:
                if (dist > attackRange)
                    ChangeState(EnemyState.Chase);
                break;
        }
    }

    /// Runs the behaviour for the current state every frame
    void ExecuteState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol: DoPatrol(); break;
            case EnemyState.Chase: DoChase(); break;
            case EnemyState.Attack: DoAttack(); break;
        }
    }

    void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        // Trigger animation if an Animator is present
        if (anim != null)
            anim.SetTrigger(newState.ToString());
    }

    // ──────────────────────────────────────────
    //  Behaviours
    // ──────────────────────────────────────────

    /// Walks back and forth; turns around at walls and ledges
    void DoPatrol()
    {
        rb.linearVelocity = new Vector2(patrolSpeed * direction, rb.linearVelocity.y);

        bool wallAhead = CheckWallAhead();
        bool groundAhead = CheckGroundAhead();

        if (wallAhead || !groundAhead)
            direction *= -1f;
    }

    /// Moves toward the player; optionally jumps over walls
    void DoChase()
    {
        if (player == null) return;

        direction = (player.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(chaseSpeed * direction, rb.linearVelocity.y);

        // Jump over wall if the flag is enabled and the enemy is grounded
        if (canJump && CheckWallAhead() && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    /// Stops moving and plays the attack animation
    void DoAttack()
    {
        // Halt horizontal movement during the attack
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (anim != null)
            anim.SetTrigger("Attack");

        // Deal damage via an AnimationEvent, or call:
        // player.GetComponent<PlayerHealth>().TakeDamage(1);
    }

    // ──────────────────────────────────────────
    //  Detection
    // ──────────────────────────────────────────

    /// Returns true if there is an unobstructed line of sight to the player
    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector2 origin = transform.position;
        Vector2 targetDir = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.position);

        // If the ray hits ground/wall geometry before reaching the player, sight is blocked
        RaycastHit2D hit = Physics2D.Raycast(origin, targetDir, dist, groundLayer);
        return hit.collider == null;
    }

    /// Returns true if a wall is present directly in front of the enemy
    bool CheckWallAhead()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(0, colliderOffset.y);
        Vector2 dir = new Vector2(direction, 0);

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, wallCheckDist, groundLayer);

        Debug.DrawRay(origin, dir * wallCheckDist, Color.red); // visible in Scene view
        return hit.collider != null;
    }

    /// Returns true if there is solid ground ahead of the enemy's feet (ledge detection)
    bool CheckGroundAhead()
    {
        // Cast from just in front of the enemy's foot
        float footY = transform.position.y - (colliderSize.y * 0.5f);
        float checkX = transform.position.x + (direction * (colliderSize.x * 0.5f + 0.1f));

        Vector2 origin = new Vector2(checkX, footY);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDist, groundLayer);

        Debug.DrawRay(origin, Vector2.down * groundCheckDist, Color.yellow);
        return hit.collider != null;
    }

    /// Updates the isGrounded flag each frame
    void CheckGrounded()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(0, -(colliderSize.y * 0.5f));
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, groundLayer);
        isGrounded = hit.collider != null;

        if (anim != null)
            anim.SetBool("IsGrounded", isGrounded);
    }

    // ──────────────────────────────────────────
    //  Utilities
    // ──────────────────────────────────────────

    /// Flips the sprite to match the current movement direction
    void FlipSprite()
    {
        sr.flipX = (direction < 0);
    }

    /// Called externally (e.g. from EnemyHealth) when the enemy receives lethal damage
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        currentState = EnemyState.Dead;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        if (anim != null)
            anim.SetTrigger("Die");
        else
            Destroy(gameObject, 0.5f);

        // Disable collision with the player layer after death
        Physics2D.IgnoreLayerCollision(
            gameObject.layer,
            LayerMask.NameToLayer("Player"),
            true
        );
    }

    /// Called via AnimationEvent on the last frame of the death animation
    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    // ──────────────────────────────────────────
    //  Editor Gizmos
    // ──────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Lose-sight range
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, loseRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
