using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAI : EnemyAI
{

    public enum SkeletonState
    {
        Walking,
        Dropping,
        PileOfBones
    }
    public SkeletonState skeletonState = SkeletonState.Walking;
    private int deathCount = 0;
    public float reviveTime = 5f;
    private float reviveTimer = 0f;
    private int maxHealth;
    private int pileOfBonesHealth;

    protected override void Start()
    {
        base.Start();

        maxHealth = health;
        pileOfBonesHealth = maxHealth / 2;
    }

    protected override void Update()
    {
        base.Update();
        switch(skeletonState){
            case SkeletonState.Walking:
                if (CanSeePlayer())
                {
                    ChasePlayer();
                }
                break;
            case SkeletonState.Dropping:
                rb.velocity = Vector2.zero;
                reviveTimer = 0;
                skeletonState = SkeletonState.PileOfBones;
                rb.freezeRotation = true;
                break;
            case SkeletonState.PileOfBones:
                rb.velocity = Vector2.zero;
                reviveTimer += Time.deltaTime;
                if(reviveTimer >= reviveTime){
                    skeletonState = SkeletonState.Walking;
                    health = maxHealth;
                    rb.freezeRotation = false;
                }
                break;
        }
    }

    protected override void FixedUpdate()
    {
        if (skeletonState == SkeletonState.Walking)
        {
            base.FixedUpdate();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DamagingObject"))
        {
            health--;
            if (health <= 0)
            {
                Die();
            }
        }
        else if (other.gameObject.CompareTag("Player") && lastAttackTime >= attackCooldown && skeletonState == SkeletonState.Walking)
        {
            playerController.TakeDamage(1);
        }
    }

    protected override void Die()
    {
        //Debug.Log("Skeleton died");
        if(skeletonState == SkeletonState.Walking){
            skeletonState = SkeletonState.Dropping;
            health = pileOfBonesHealth;
        }
        else if(skeletonState == SkeletonState.PileOfBones){
            deathCount++;
            //Debug.Log("Skeleton death count: " + deathCount);
            if (deathCount >= 2)
            {
                //Debug.Log("Died for real");
                Destroy(gameObject);
            }
            else
            {
                //health = pileOfBonesHealth;
                //skeletonState = SkeletonState.Walking;
            }
        }
    }

    void ChasePlayer()
    {
        if (target != null)
        {
            Vector2 directionToPlayer = target.position - transform.position;
            MoveTowardsTarget(directionToPlayer);
        }
    }

    bool CanSeePlayer()
    {
        // Raycast to check if there's a clear line of sight to the player
        if (target != null)
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
}