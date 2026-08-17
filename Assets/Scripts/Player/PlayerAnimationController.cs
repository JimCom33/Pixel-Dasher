using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class PlayerAnimationController : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsWallSliding = Animator.StringToHash("IsWallSliding");
    private static readonly int DoubleJump = Animator.StringToHash("DoubleJump");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int AttackState = Animator.StringToHash("Ninja_Attack1");

    private Animator animator;
    private PlayerMovement movement;
    private SpriteRenderer spriteRenderer;
    private int handledDoubleJumpSequence;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        bool movingLeft = keyboard != null
            && (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed);
        bool movingRight = keyboard != null
            && (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed);
        bool isMoving = movingLeft || movingRight;
        bool isJumping = !movement.IsGrounded;

        animator.SetBool(IsMoving, isMoving);
        animator.SetBool(IsJumping, isJumping);
        animator.SetBool(IsWallSliding, movement.IsWallSliding);

        if (movement.DoubleJumpSequence != handledDoubleJumpSequence)
        {
            handledDoubleJumpSequence = movement.DoubleJumpSequence;
            animator.SetTrigger(DoubleJump);
        }

        Mouse mouse = Mouse.current;
        if (mouse != null
            && mouse.leftButton.wasPressedThisFrame
            && !IsAttackInProgress())
        {
            animator.SetTrigger(Attack);
        }

        if (movingLeft && !movingRight)
        {
            spriteRenderer.flipX = true;
        }
        else if (movingRight && !movingLeft)
        {
            spriteRenderer.flipX = false;
        }
    }

    private bool IsAttackInProgress()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.shortNameHash == AttackState)
        {
            return true;
        }

        return animator.IsInTransition(0)
            && animator.GetNextAnimatorStateInfo(0).shortNameHash == AttackState;
    }
}
