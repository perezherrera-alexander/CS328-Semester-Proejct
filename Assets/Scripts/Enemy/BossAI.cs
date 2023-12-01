using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : EnemyAI
{
    public int maxHealth;

    protected override void Start()
    {
        base.Start();

        health = 100;

        maxHealth = health;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {

        if (target != null)
        {
            //RotateToTarget();
            AttackCooldown();
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DamagingObject"))
        {
            TakeDamage();
        } 
        else if (other.gameObject.CompareTag("Player") && lastAttackTime >= attackCooldown)
        {
            playerController.TakeDamage(3);
        }
    }

    protected bool CanSeePlayer()
    {
        // Raycast to check if there's a clear line of sight to the player
        if (target != null && target.CompareTag("Player"))
        {
            if (Vector2.Distance(transform.position, target.position) > 30)
            {
                return false;
            }
            Vector2 directionToPlayer = target.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, Mathf.Infinity, LayerMask.GetMask("Player"));

            if (hit.collider != null && hit.collider.CompareTag("Wall"))
            {
                return false;
            } else if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                // Player is in line of sight
                return true;
            }
        }

        // Player is not in line of sight
        return false;
    }
}