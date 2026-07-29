using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 6f;
    [SerializeField, Min(0f)] private float jumpSpeed = 11f;
    [SerializeField, Min(0f)] private float gravityScale = 4f;

    private Rigidbody2D body;
    private readonly HashSet<Collider2D> groundedColliders = new();
    private float horizontalInput;
    private bool jumpQueued;

    public bool IsGrounded => groundedColliders.Count > 0;

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

        if (jumpPressed && IsGrounded)
        {
            jumpQueued = true;
        }
    }

    private void FixedUpdate()
    {
        float verticalVelocity = jumpQueued ? jumpSpeed : body.linearVelocity.y;
        body.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalVelocity);

        if (jumpQueued)
        {
            jumpQueued = false;
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
    }

    private void UpdateGroundContact(Collision2D collision)
    {
        bool hasGroundContact = false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.5f)
            {
                hasGroundContact = true;
                break;
            }
        }

        if (hasGroundContact)
        {
            groundedColliders.Add(collision.collider);
        }
        else
        {
            groundedColliders.Remove(collision.collider);
        }
    }
}
