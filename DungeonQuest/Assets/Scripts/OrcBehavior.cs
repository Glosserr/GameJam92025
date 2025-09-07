
using UnityEngine;
using System;
public class OrcBehavior : MobBehavior
{
    private Transform target;
    public float attackDuration;
    public float attackRange;
    public float attackRadius;
    public float attackAngle;
    public float attackDamage;
    public float aggroDistance;
    public float stopDistance;

    protected override void Start()
    {
        base.Start();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (inAction) return;

        if (target != null)
        {
            if (Math.Abs(target.position.x - transform.position.x) <= aggroDistance ||
                Math.Abs(target.position.z - transform.position.z) <= aggroDistance)
            {
                MoveToAttack();
            }
        }
        else
        {
            Idle();
        }
    }

    void MoveToAttack()
    {
        if (Math.Abs(target.position.x - transform.position.x) > stopDistance ||
            Math.Abs(target.position.z - transform.position.z) > stopDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            move = new Vector3(direction.x, 0, direction.z);
            if (move != Vector3.zero) lastMoveDirection = move;
            base.FlipSprite(direction.x);
            animator.Play("walk");
        }
        else
        {
            move = Vector3.zero;
            Attack();
        }

    }

    void Attack()
    {
        StartAttack("attack", attackDuration,
            () => base.DamageInRange(attackRange, attackRadius, attackAngle, attackDamage, (target.position - transform.position).normalized),
            0.15f
        );
    }
    void Idle()
    {
        move = Vector3.zero;
        animator.Play("idle");
    }
}
