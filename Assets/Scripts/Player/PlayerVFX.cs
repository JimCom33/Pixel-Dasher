using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(SpriteRenderer))]
public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private Vector2 slashOffset = new Vector2(1f, 0.05f);
    [SerializeField] private Vector2 dustOffset = new Vector2(0f, -0.9f);

    private PlayerMovement movement;
    private SpriteRenderer playerRenderer;
    private Rigidbody2D body;
    private bool wasGrounded;
    private int handledDoubleJumpSequence;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        playerRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        wasGrounded = movement.IsGrounded;
        handledDoubleJumpSequence = movement.DoubleJumpSequence;
    }

    private void Update()
    {
        bool isGrounded = movement.IsGrounded;
        if (wasGrounded && !isGrounded && body.linearVelocity.y > 0.1f)
        {
            PlayDustEffect();
        }
        else if (!wasGrounded && isGrounded)
        {
            PlayDustEffect();
        }

        if (movement.DoubleJumpSequence != handledDoubleJumpSequence)
        {
            handledDoubleJumpSequence = movement.DoubleJumpSequence;
            PlayDustEffect();
        }

        wasGrounded = isGrounded;
    }

    public void PlaySlashEffect()
    {
        if (slashPrefab == null)
        {
            return;
        }

        float direction = playerRenderer.flipX ? -1f : 1f;
        Vector3 localPosition = new Vector3(
            slashOffset.x * direction,
            slashOffset.y,
            0f);
        GameObject effect = Instantiate(slashPrefab, transform);
        effect.transform.localPosition = localPosition;
        SpriteRenderer effectRenderer = effect.GetComponent<SpriteRenderer>();
        if (effectRenderer != null)
        {
            effectRenderer.flipX = playerRenderer.flipX;
        }

        Destroy(effect, 0.55f);
    }

    private void PlayDustEffect()
    {
        if (dustPrefab == null)
        {
            return;
        }

        Vector3 position = transform.position + (Vector3)dustOffset;
        GameObject effect = Instantiate(dustPrefab, position, Quaternion.identity);
        Destroy(effect, 0.55f);
    }
}
