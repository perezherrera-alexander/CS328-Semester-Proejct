using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Suit of armor enemy that "hardens" every couple of seconds, where it stops moving and is invulnerable to attacks

public class ArmorAI : EnemyAI
{
    public enum ArmorState 
    {
        Walking,
        Hardening,
        Hardened
    }
    public ArmorState armorState = ArmorState.Walking;
    public float hardenTime = 3f;
    public float walkTime = 5f;
    private float timeSinceLastHarden = 0f;
    private float timeSpentHardened = 0f;
    protected override void Start()
    {
        base.Start();
        enemyName = "Armor";
    }

    protected override void Update()
    {
        base.Update();
        switch(armorState)
        {
            case ArmorState.Walking:
                // If the armor can see the player, chase them
                if (CanSeePlayer())
                {
                    ChasePlayer();
                }
                timeSinceLastHarden += Time.deltaTime;
                if(timeSinceLastHarden >= walkTime)
                {
                    armorState = ArmorState.Hardening;
                }
                break;
            case ArmorState.Hardening:
                rb.mass = 1000f;
                rb.velocity = Vector2.zero;
                rb.freezeRotation = true;
                timeSinceLastHarden = 0f;
                armorState = ArmorState.Hardened;
                break;
            case ArmorState.Hardened:
                timeSpentHardened += Time.deltaTime;
                rb.velocity = Vector2.zero;
                if(timeSpentHardened >= hardenTime)
                {
                    armorState = ArmorState.Walking;
                    rb.freezeRotation = false;
                    timeSpentHardened = 0f;
                    rb.mass = 1f;
                }
                break;
        }
    }

    protected override void FixedUpdate()
    {
        if(armorState == ArmorState.Walking)
        {
            rb.velocity = transform.up * speed;

            if (target != null && target.CompareTag("Player"))
            {
                RotateToTarget();
                AttackCooldown();
            }
        }
    }

    
    protected override void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the Skeleton collides with a damaging object (e.g., player's attack)
        if (other.gameObject.CompareTag("DamagingObject"))
        {
            if(armorState == ArmorState.Hardened)
            {
                // If the armor is hardened, do not take damage
                return;
            }
            else {
                TakeDamage();
            }
        }
        else if (other.gameObject.CompareTag("Player") && lastAttackTime >= attackCooldown && armorState == ArmorState.Walking)
        {
            playerController.TakeDamage(4);
        }
    }

   public void InflictDamage()
    {
        //Debug.Log("Armor took damage");
        if(armorState == ArmorState.Hardened)
        {
                // If the armor is hardened, do not take damage
            return;
        }
        else {
            TakeDamage();
        }
    }

    void ChasePlayer()
    {
        if (target != null && target.CompareTag("Player"))
        {
            Vector2 directionToPlayer = target.position - transform.position;
            MoveTowardsTarget(directionToPlayer);
        }
    }

    bool CanSeePlayer()
    {
        // Raycast to check if there's a clear line of sight to the player
        if (target != null && target.CompareTag("Player"))
        {
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

    protected override void Die()
    {
        base.Die();

        // Give player back mana
        playerController.currentMana += 30;
    }
}
