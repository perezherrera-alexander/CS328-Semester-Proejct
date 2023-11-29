using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAI : EnemyAI
{
    /*
    private bool isInPack = false;
    private bool isAttacking = false;
    private float obstacleAvoidanceRadius = 2f; // Adjust as needed
    private bool isRandomMovementScheduled = false;

    protected override void Start()
    {
        base.Start();

        speed = 3f;
        checkRadius = 20f;
        InvokeRepeating("RandomMovement", 0f, 3f); // Invoke RandomMovement every 3 seconds
    }

    void Update()
    {
        Collider2D[] enemies = CheckForEnemies();

        if (enemies.Length > 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.CompareTag("Player"))
                {
                    RunAway();
                }
                else if (enemy.CompareTag("Goblin") && !isInPack)
                {
                    GroupUp();
                }
            }
        }
        else
        {
            isAttacking = false;
        }
    }

    void FixedUpdate()
    {
        // Update the target (player) reference
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }

        // Check for enemies and perform appropriate actions
        Collider2D[] enemies = CheckForEnemies();

        if (enemies.Length > 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.CompareTag("Player"))
                {
                    RunAway();
                }
                else if (enemy.CompareTag("Goblin") && !isInPack)
                {
                    GroupUp();
                }
            }
        }
        else
        {
            isAttacking = false;
        }

        // If in a pack and the player is in attack radius, attack the player
        if (isInPack && target != null && target.CompareTag("Player"))
        {
            float distanceToPlayer = Vector2.Distance(transform.position, target.position);

            if (distanceToPlayer <= attackRadius)
            {
                AttackPlayer();
            }
        }
        else
        {
            // If not in a pack or not targeting the player, perform random movement
            if (!isRandomMovementScheduled)
            {
                StartCoroutine(ScheduleRandomMovement());
            }
        }
    }

    IEnumerator ScheduleRandomMovement()
    {
        isRandomMovementScheduled = true;
        yield return new WaitForSeconds(Random.Range(1f, 3f)); // Adjust the time interval as needed
        RandomMovement();
        isRandomMovementScheduled = false;
    }

    void RandomMovement()
    {
        if (!isInPack)
        {
            // Generate a random direction
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // Cast a ray to check for obstacles
            RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDirection, obstacleAvoidanceRadius, LayerMask.GetMask("Wall"));

            if (hit.collider == null)
            {
                // No obstacle, move in the random direction
                MoveTowardsTarget(randomDirection);
            }
            else
            {
                // Obstacle detected, do not perform another random movement immediately
                // Adjust the interval and schedule the next random movement
                StartCoroutine(ScheduleRandomMovement());
            }
        }
    }

    void GroupUp()
    {
        Collider2D[] nearbyGoblins = Physics2D.OverlapCircleAll(transform.position, checkRadius, LayerMask.GetMask("Goblin"));

        if (nearbyGoblins.Length > 1)
        {
            // Find the nearest goblin
            Transform nearestGoblin = null;
            float shortestDistance = float.MaxValue;

            foreach (var goblin in nearbyGoblins)
            {
                if (goblin.transform != transform)
                {
                    float distance = Vector2.Distance(transform.position, goblin.transform.position);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestGoblin = goblin.transform;
                    }
                }
            }

            // Move towards the nearest goblin
            if (nearestGoblin != null)
            {
                Vector2 directionToNearestGoblin = nearestGoblin.position - transform.position;

                // Cast a ray to check for obstacles
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToNearestGoblin.normalized, obstacleAvoidanceRadius, LayerMask.GetMask("Wall"));

                if (hit.collider == null)
                {
                    // No obstacle, move towards the nearest goblin
                    MoveTowardsTarget(directionToNearestGoblin);
                }
                else
                {
                    // Obstacle detected, do not perform another random movement immediately
                    // Adjust the interval and schedule the next random movement
                    StartCoroutine(ScheduleRandomMovement());
                }
            }
        }

        // For simplicity, add a delay before the goblin starts moving randomly again
        Invoke("ResumeRandomMovement", 5f);
    }

    void ResumeRandomMovement()
    {
        isInPack = true;
        InvokeRepeating("RandomMovement", 0f, 3f);
    }

    void RunAway()
    {
        if (isInPack)
        {
            Vector2 directionToPlayer = transform.position - target.position;

            // Cast a ray to check for obstacles
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, obstacleAvoidanceRadius, LayerMask.GetMask("Wall"));

            if (hit.collider == null)
            {
                // No obstacle, move away from the player
                MoveTowardsTarget(directionToPlayer);
            }
            else
            {
                // Obstacle detected, do not perform another random movement immediately
                // Adjust the interval and schedule the next random movement
                StartCoroutine(ScheduleRandomMovement());
            }
        }
        else
        {
            // If not in a pack, run away from the player
            if (target != null && target.CompareTag("Player"))
            {
                Vector2 directionToPlayer = transform.position - target.position;

                // Cast a ray to check for obstacles
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, obstacleAvoidanceRadius, LayerMask.GetMask("Wall"));

                if (hit.collider == null)
                {
                    // No obstacle, run away from the player
                    MoveTowardsTarget(directionToPlayer);
                }
                else
                {
                    // Obstacle detected, do not perform another random movement immediately
                    // Adjust the interval and schedule the next random movement
                    StartCoroutine(ScheduleRandomMovement());
                }
            }
            else
            {
                // If the target is not the player, perform random movement
                StartCoroutine(ScheduleRandomMovement());
            }
        }
    }

    void AttackPlayer()
    {
        // Implement logic for attacking the player
        isAttacking = true;
    }
    */
}
