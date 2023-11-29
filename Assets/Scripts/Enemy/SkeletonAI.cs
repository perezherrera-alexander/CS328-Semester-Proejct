using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonAI : EnemyAI
{
    private bool isDead = false;
    private bool isOnGround = false;
    private float reanimationTime = 5f; // Adjust as needed
    private float timeSinceDeath = 0f;

    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {
        base.Update();

        if (!isDead && !isOnGround)
        {
            // Check if the Skeleton can see the player
            if (CanSeePlayer())
            {
                ChasePlayer();
            } 
            else
            {
                RandomMovement();
            }
        }

        if (isDead && !isOnGround)
        {
            // Check if the Skeleton has been on the ground for the set reanimation time
            timeSinceDeath += Time.deltaTime;
            if (timeSinceDeath >= reanimationTime)
            {
                Reanimate();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the Skeleton collides with a damaging object (e.g., player's attack)
        if (other.CompareTag("DamagingObject"))
        {
            if (!isDead)
            {
                Die();
            }
            else if (isOnGround)
            {
                // If the Skeleton is on the ground and gets hit again, reanimate
                Reanimate();
            }
        }
    }

    private void RandomMovement()
    {
        if (!isDead && !isOnGround)
        {
            // Generate a random direction
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            MoveTowardsTarget(randomDirection);
        }
    }

    private void ChasePlayer()
    {
        // If the Skeleton can see the player, move towards the player
        if (target != null)
        {
            Vector2 directionToPlayer = target.position - transform.position;
            MoveTowardsTarget(directionToPlayer);
        }
    }

    private bool CanSeePlayer()
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

    protected override void Die()
    {
        isDead = true;
        // Collapse the Skeleton to the ground (play animation, set appropriate state, etc.)
        // For demonstration purposes, we're just deactivating the GameObject after a delay
        StartCoroutine(CollapseAndDeactivate());
    }

    private void Reanimate()
    {
        // Reanimate the Skeleton (play animation, set appropriate state, etc.)
        // For demonstration purposes, we're just resetting the state and reactivating the GameObject
        isDead = false;
        isOnGround = false;
        timeSinceDeath = 0f;
        gameObject.SetActive(true);
    }

    private IEnumerator CollapseAndDeactivate()
    {
        // Play the collapse animation or perform any other necessary actions
        // For demonstration purposes, we're just deactivating the GameObject after a delay
        yield return new WaitForSeconds(1f); // Adjust the delay as needed

        // Set the Skeleton as on the ground
        isOnGround = true;

        // Deactivate the GameObject
        gameObject.SetActive(false);
    }
}
