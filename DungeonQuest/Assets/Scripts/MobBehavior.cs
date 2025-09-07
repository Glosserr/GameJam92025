using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobBehavior : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Vector3 lastMoveDirection = Vector3.forward;
    public LayerMask groundLayers = -2;
    public float groundCheckDistance = 0.1f;
    public float groundCheckRadius = 0.1f;
    public float health = 3;
    public float speed = 2f;
    public float gravity = -5f;
    protected Rigidbody rb;
    protected Vector3 move;
    protected float actionTimer = 0f;
    public float jumpForce = 2.5f;
    float velocityY = 0;
    protected bool inJump = false;
    protected bool inAction = false;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }

    protected virtual void Update()
    {
        if (health <= 0)
        {
            StartCoroutine(HandleDeath());
        }

        // handle lock timer
        if (inAction)
        {
            actionTimer -= Time.deltaTime;
            if (actionTimer <= 0f)
            {
                inAction = false;
            }
        }

        // movement
        Vector3 velocity = move * speed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

        GroundCheck();
    }

    // ------------------------
    // ATTACK HANDLING
    // ------------------------
    protected void StartAttack(string animationName, float duration, System.Action onHit, float hitDelay = 0f)
    {
        if (!inAction)
        {
            animator.Play(animationName);
            inAction = true;
            actionTimer = duration;
            StartCoroutine(DoAttack(onHit, hitDelay));
        }
    }

    private IEnumerator DoAttack(System.Action onHit, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        onHit?.Invoke();
    }

    // ------------------------
    // UTILS
    // ------------------------
    public void FlipSprite(float inputX)
    {
        if (inputX > 0) spriteRenderer.flipX = false;
        else if (inputX < 0) spriteRenderer.flipX = true;
    }

    public void GroundCheck()
    {
        if (IsGrounded())
        {
            velocityY = 0;
            inJump = false;
        }
    }

    public bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.down * 0.5f;
        bool grounded = Physics.CheckSphere(origin, groundCheckRadius, groundLayers);
        Debug.DrawRay(origin, Vector3.down * groundCheckDistance, grounded ? Color.green : Color.red);
        return grounded;
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            inJump = true;
        }
    }

    public void DamageInRadius(float range, float radius, float damage)
    {
        Vector3 center = transform.position + transform.forward * range;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            MobBehavior mob = hitCollider.GetComponentInParent<MobBehavior>();
            if (mob != null && mob != this)
            {
                Debug.Log($"Hit mob: {mob.name} with damage {damage}");
                StartCoroutine(mob.TakeDamage(damage));
            }
        }
    }

    public void DamageInRange(float range, float radius, float coneAngle, float damage, Vector3 direction)
    {
        Vector3 center = transform.position + direction.normalized * range;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log($"Checking {hitCollider.name}");
            MobBehavior mob = hitCollider.GetComponentInParent<MobBehavior>();
            if (mob != null && mob != this)
            {
                Vector3 toTarget = (mob.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(direction, toTarget);
                if (angle <= coneAngle)
                {
                    Debug.Log($"Hit mob in cone: {mob.name} with damage {damage}");
                    StartCoroutine(mob.TakeDamage(damage));
                }
            }
        }
    }

    public IEnumerator TakeDamage(float amount)
    {
        health -= amount;
        animator.Play("hurt");
        yield return null;
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);
        Debug.Log($"Mob took {amount} damage!");
    }

    public IEnumerator HandleDeath()
    {
        Debug.Log($"{name} died!");
        animator.Play("death");
        yield return null;
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);
        Destroy(gameObject);
    }
}
