using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Animator animator;
    public SpriteRenderer spriteRenderer;
    public float speed;
    public float gravity;
    public float jumpForce;
    float velocityY = 0;
    bool inSlam = false;
    bool inJump = false;
    bool inAction = false;
    float actionTimer = 0f;
    public float slashDuration = 0.5f;
    public float slamDuration = 2f;
    CharacterController controller;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        velocityY += gravity * Time.deltaTime;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.right * inputX + transform.forward * inputZ).normalized;
        Vector3 velocity = move * speed;
        velocity.y = velocityY;

        // Wait for current action to finish
        if (inAction)
        {
            actionTimer -= Time.deltaTime;
            if (actionTimer <= 0f)
            {
                inAction = false;
            }
        }
        // Finish slam if in air
        else if (inSlam)
        {
            if (!inJump && controller.isGrounded)
            {
                SlamDown();
            }
        }
        // If not waiting for action or slam, allow new inputs
        else
        {
            // Allow jump input at any time if not already jumping
            if (Input.GetKeyDown(KeyCode.Space) && !inJump)
            {
                Jump();
                
            }
            // Handle Click Input
            if (Input.GetMouseButtonDown(0))
            {
                if (!inJump)
                {
                    Slash();
                }
                else
                {
                    SlamUp();
                }
            }
            // Handle movement and animations
            if (move == Vector3.zero)
            {
                animator.Play("idle");
            }
            else
            {
                FlipSprite(inputX);
                animator.Play("walk");
            }
        }
        // Finally move character
        controller.Move(velocity * Time.deltaTime);
        GroundCheck();
    }
    void GroundCheck()
    {
        if (controller.isGrounded)
        {
            velocityY = 0;
            inJump = false;
        }
    }
    void Jump()
    {
        velocityY = jumpForce;
        inJump = true;
    }
    void Slash()
    {
        animator.Play("slash");
        inAction = true;
        actionTimer = slashDuration;
    }
    void SlamUp()
    {
        animator.Play("slamup");
        inSlam = true;
    }
    void SlamDown()
    {
        animator.Play("slamdown");
        inAction = true;
        actionTimer = slamDuration;
        inSlam = false;
    }

    void FlipSprite(float inputX)
    {
        if (inputX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (inputX < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}