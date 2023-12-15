using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoblinAI : EnemyAI
{
    private bool isInPack = false;
    private bool isAttacking = false;
    private float goblinCheckRadius = 15f; // Adjust as needed
    private bool isCloseToGoblin = false;

    protected override void Start()
    {
        base.Start();
        enemyName = "Goblin";
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        rb.velocity = transform.up * speed;

        if (lastAttackTime > attackCooldown)
        {
            isAttacking = false;
        }
        else if (lastAttackTime < attackCooldown)
        {
            isAttacking = true;
        }

        if (target != null)
        {
            if (isAttacking)
            {
                // If the Goblin is attacking, do not perform other actions
                return;
            }

            if (CanSeePlayer())
            {
                // If the Goblin is in a pack, chase the player
                if (isInPack && isCloseToGoblin)
                {
                    RotateToTarget();
                }
                else if (isInPack && !isCloseToGoblin)
                {
                    MoveToGoblin();
                }
                else if (!isInPack)
                {
                    // If the Goblin is not in a pack, move away from the player
                    MoveAwayFromTarget(target.position);
                    // If the Goblin is not in a pack, check if there are other Goblins nearby
                    SenseOtherGoblins();
                }
            }
            else
            {
                // If the Goblin cannot see the player, check if there are other Goblins nearby
                SenseOtherGoblins();
            }
        }
    }

    protected override void RotateToTarget()
    {
        if (target.CompareTag("Player"))
        {
            base.RotateToTarget();
        }
    }

    private bool CanSeePlayer()
    {
        // Raycast to check if there's a clear line of sight to the player
        if (target != null && target.CompareTag("Player"))
        {
            Vector2 directionToPlayer = target.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                directionToPlayer,
                Mathf.Infinity,
                LayerMask.GetMask("Player")
            );

            if (hit.collider != null && hit.collider.CompareTag("Wall"))
            {
                return false;
            }
            else if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                // Player is in line of sight
                return true;
            }
        }

        return false;
    }

    private void SenseOtherGoblins()
    {
        Collider2D[] goblins = Physics2D.OverlapCircleAll(
            transform.position,
            goblinCheckRadius,
            LayerMask.GetMask("Goblin")
        );

        if (goblins.Length > 1)
        {
            // If there are other Goblins nearby, join the pack
            isInPack = true;
            MoveToGoblin();
        }
        else
        {
            // If there are no other Goblins nearby, move randomly
            isInPack = false;
        }
    }

    private void MoveToGoblin()
    {
        // Find the closest Goblin
        Collider2D[] goblins = Physics2D.OverlapCircleAll(
            transform.position,
            goblinCheckRadius,
            LayerMask.GetMask("Goblin")
        );
        float closestDistance = Mathf.Infinity;
        Transform closestGoblin = null;

        foreach (Collider2D goblin in goblins)
        {
            if (goblin.gameObject != gameObject)
            {
                float distance = Vector2.Distance(transform.position, goblin.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestGoblin = goblin.transform;
                }
            }
        }

        // Move towards the closest Goblin
        if (closestGoblin != null)
        {
            Vector2 directionToGoblin = closestGoblin.position - transform.position;
            MoveTowardsTarget(directionToGoblin);
        }

        // Check if the Goblin is close to the closest Goblin
        if (closestDistance <= 3f)
        {
            isCloseToGoblin = true;
        }
        else
        {
            isCloseToGoblin = false;
        }
    }

    private void MoveAwayFromTarget(Vector2 direction)
    {
        // Move away from the target
        rb.velocity = -direction.normalized * speed;

        // Rotate away from the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, rotateSpeed);
    }

    protected override void Die()
    {
        base.Die();

        // Give player back mana
        //playerController.currentMana += 10;
        playerController.increaseMana(10);
    }
}
