using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehavior : MobBehavior
{
    bool inSlam = false;

    [Header("Slash Attack")]
    public float slashDuration;
    public float slashRange;
    public int coins;
    public float slashRadius;
    public float slashAngle;
    public float slashDamage;

    [Header("Slam Attack")]
    public float slamDuration;
    public float slamRadius;
    public float slamDamage;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        
        
        // Always check ground state first
        base.Update();
        GroundCheck();

        PickupInRange();

        if (!hittable)
        {
            hittableTimer -= Time.deltaTime;
            if (hittableTimer <= 0f)
            {
                hittable = true;
            }
        }

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        move = (transform.right * inputX + transform.forward * inputZ).normalized;

        if (move != Vector3.zero)
            lastMoveDirection = move;

        // Block input while locked in attack
        if (inAction)
            return;

        // Slam "waiting to land" state
        if (inSlam)
        {
            if (IsGrounded())
                SlamDown();
            return;
        }

        // Handle Jump input
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Debug.Log($"Jump pressed . inJump={IsGrounded()}, IsGrounded={IsGrounded()}");
            Jump();
        }

        // Handle Attack input
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(IsGrounded());
            if (IsGrounded()) Slash();
            else SlamUp();
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

    void Slash()
    {
        StartAttack("slash", slashDuration,
            () => DamageInRange(slashRange, slashRadius, slashAngle, slashDamage, lastMoveDirection),
            0.1f
        );
    }

    void SlamUp()
    {
        animator.Play("slamup");
        inSlam = true;
    }

    void SlamDown()
    {
        StartAttack("slamdown", slamDuration,
            () => DamageInRadius(slamRadius, slamDamage),
            0.1f
        );
        inSlam = false;
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        hittable = false;
        hittableTimer = 4f;
    }

    void PickupInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.75f);
        foreach (var hitCollider in hitColliders)
        {
            PickupBehavior item = hitCollider.GetComponentInParent<PickupBehavior>();
            if (item != null)
            {
                if (item.type == "potion")
                {
                    health += 2;
                }
                else if (item.type == "gem")
                {
                    coins += 1;
                }
                item.IsPickedUp();
            }

        }
    }
}
