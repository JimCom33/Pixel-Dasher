using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 6f;
    [SerializeField, Min(0f)] private float jumpSpeed = 11f;
    [SerializeField, Min(0f)] private float gravityScale = 4f;
    [SerializeField, Min(1)] private int maximumJumps = 2;
    [SerializeField, Min(0f)] private float wallJumpHorizontalSpeed = 10f;
    [SerializeField, Min(0f)] private float wallJumpControlLockDuration = 0.15f;
    [SerializeField, Min(0f)] private float wallSlideSpeed = 3f;

    private Rigidbody2D body;
    private readonly HashSet<Collider2D> groundedColliders = new();
    private readonly Dictionary<Collider2D, float> wallContacts = new();
    private float horizontalInput;
    private bool jumpQueued;
    private bool wallJumpQueued;
    private float wallJumpDirection;
    private float wallJumpControlLockRemaining;
    private int jumpsUsed;
    private int doubleJumpSequence;

    public bool IsGrounded => groundedColliders.Count > 0;
    public bool IsWallSliding { get; private set; }
    public int DoubleJumpSequence => doubleJumpSequence;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = gravityScale;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            horizontalInput = 0f;
            return;
        }

        bool left = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
        bool right = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;
        horizontalInput = (right ? 1f : 0f) - (left ? 1f : 0f);

        bool jumpPressed = keyboard.spaceKey.wasPressedThisFrame
            || keyboard.wKey.wasPressedThisFrame
            || keyboard.upArrowKey.wasPressedThisFrame;

        float wallNormal = GetWallNormal();
        if (jumpPressed && !IsGrounded && Mathf.Abs(wallNormal) > 0.5f)
        {
            wallJumpDirection = wallNormal;
            wallJumpQueued = true;
            jumpQueued = true;
            jumpsUsed = 1;
        }
        else if (jumpPressed && (IsGrounded || jumpsUsed < maximumJumps))
        {
            if (!IsGrounded && jumpsUsed == 0)
            {
                jumpsUsed = 1;
            }

            jumpsUsed++;
            jumpQueued = true;

            if (jumpsUsed > 1)
            {
                doubleJumpSequence++;
            }
        }
    }

    private void FixedUpdate()
    {
        float wallNormal = GetWallNormal();
        bool holdingTowardWall = horizontalInput * wallNormal < -0.5f;
        IsWallSliding = !IsGrounded
            && !jumpQueued
            && wallJumpControlLockRemaining <= 0f
            && body.linearVelocity.y < 0f
            && Mathf.Abs(wallNormal) > 0.5f
            && holdingTowardWall;

        float verticalVelocity = jumpQueued
            ? jumpSpeed
            : IsWallSliding
                ? Mathf.Max(body.linearVelocity.y, -wallSlideSpeed)
                : body.linearVelocity.y;
        float horizontalVelocity;

        if (wallJumpQueued)
        {
            horizontalVelocity = wallJumpDirection * wallJumpHorizontalSpeed;
            wallJumpControlLockRemaining = wallJumpControlLockDuration;
        }
        else if (wallJumpControlLockRemaining > 0f)
        {
            wallJumpControlLockRemaining -= Time.fixedDeltaTime;
            horizontalVelocity = body.linearVelocity.x;
        }
        else
        {
            horizontalVelocity = horizontalInput * moveSpeed;
        }

        body.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);

        if (jumpQueued)
        {
            jumpQueued = false;
            wallJumpQueued = false;
            groundedColliders.Clear();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UpdateGroundContact(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateGroundContact(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        groundedColliders.Remove(collision.collider);
        wallContacts.Remove(collision.collider);
    }

    private void UpdateGroundContact(Collision2D collision)
    {
        bool hasGroundContact = false;
        float wallNormal = 0f;
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            if (normal.y > 0.5f)
            {
                hasGroundContact = true;
            }

            if (Mathf.Abs(normal.x) > 0.5f)
            {
                wallNormal = normal.x;
            }
        }

        if (hasGroundContact)
        {
            groundedColliders.Add(collision.collider);
            jumpsUsed = 0;
        }
        else
        {
            groundedColliders.Remove(collision.collider);
        }

        if (Mathf.Abs(wallNormal) > 0.5f)
        {
            wallContacts[collision.collider] = wallNormal;
        }
        else
        {
            wallContacts.Remove(collision.collider);
        }
    }

    private float GetWallNormal()
    {
        foreach (float normal in wallContacts.Values)
        {
            if (Mathf.Abs(normal) > 0.5f)
            {
                return normal;
            }
        }

        return 0f;
    }
}
