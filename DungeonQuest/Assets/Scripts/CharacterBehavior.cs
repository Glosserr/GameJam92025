using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public AudioClip heal;
    public AudioClip gem;
    public GameObject munnyObj;
    public GameObject healthObj;
    public TMP_Text munnyText;
    public TMP_Text healthText;


    protected override void Start()
    {
        base.Start();
        munnyObj = GameObject.FindGameObjectWithTag("munny");
        if (munnyObj != null)
            munnyText = munnyObj.GetComponent<TMP_Text>();

        healthObj = GameObject.FindGameObjectWithTag("health");
        if (healthObj != null)
            healthText = healthObj.GetComponent<TMP_Text>();


    }

    protected override void Update()
    {
        base.Update();
        if (munnyText != null)
            munnyText.text = "Munny: " + coins.ToString();

        if (healthText != null)
            healthText.text = "Health: " + health.ToString();
        // Always check ground state first

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
            bool atTable = hitCollider.CompareTag("table");
            if (item != null)
            {
                if (item.type == "potion")
                {
                    if (health < 8)
                    {
                        health += 2;
                        src.PlayOneShot(heal);
                    }
                    else if (health == 9)
                    {
                        health += 1;
                        src.PlayOneShot(heal);
                    }
                }
                else if (item.type == "gem")
                {
                    coins += 1;
                    src.PlayOneShot(gem);
                }
                item.IsPickedUp();
            }
            if (atTable && Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene(2);
            }

        }
    }
}
