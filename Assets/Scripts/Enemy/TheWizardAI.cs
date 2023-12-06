using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TheWizardAI : BossAI
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float shootingCooldown = 0.05f;
    public bool beingUsedByBoss = false;
    public float randomAngleRange = 5f;
    public float randomSpeedRange = 2f;
    public float teleportCooldown = 10f;
    private float lastTeleportTime;
    private bool isTeleporting = false;

    private float startHealth;
    private float startSpeed;
    public float halfHealthSpeedBuff = 1.2f;
    private bool isDoingAttack = false;
    public float chanceToDeflectAttack = 0.05f;
    public float chanceToDodgeAttack = 0.1f;
    private bool isMoving = false;

    protected override void Start()
    {
        base.Start();

        health = 150;

        attackCooldown = 5f;

        startHealth = health;
        startSpeed = speed;
    }

    protected override void FixedUpdate()
    {
        if (target != null)
        {
            RotateToTarget();
            AttackCooldown();
        }
        if (isTeleporting)
        {
            lastTeleportTime = Time.time;
        }

        if (Time.time - lastTeleportTime >= teleportCooldown && playerController.playerIsShooting)
        {
            Teleport();
        }

        if (health <= startHealth / 2)
        {
            speed = startSpeed * halfHealthSpeedBuff;
            teleportCooldown = 7.5f;
        }
        else if (health <= startHealth / 4)
        {
            speed = startSpeed * halfHealthSpeedBuff * halfHealthSpeedBuff;
            teleportCooldown = 4f;
        }
    }

    protected override void RotateToTarget()
    {
        Vector2 targetDir = target.position - transform.position;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, rotateSpeed);

        float duration = Random.Range(0.5f, 1.5f);

        StartCoroutine(MoveLeftOrRight(duration));
    }

    private IEnumerator MoveLeftOrRight(float duration)
    {
        if (!isMoving)
        {
            isMoving = true;
            float randomFloat = Random.Range(0f, 1f);
            if (randomFloat <= 0.5f)
            {
                rb.velocity = transform.right * speed;
            }
            else
            {
                rb.velocity = -transform.right * speed;
            }
            yield return new WaitForSeconds(duration);
            //rb.velocity = Vector2.zero;
            isMoving = false;
        }
    }

    protected override void AttackCooldown()
    {
        if (Time.time - lastAttackTime >= attackCooldown && !isDoingAttack)
        {
            int randomAttack = Random.Range(1, 4);

            AllMighty(randomAttack);
        }
    }

    private void AllMighty(int randomAttack)
    {
        switch (randomAttack)
        {
            case 1:
                ShootProjectiles();

                beingUsedByBoss = false;
                isDoingAttack = false;
                break;
            case 2:
                //StartCoroutine(ShootBeam());
                break;
            case 3:
                //StartCoroutine(AOEAttack());
                break;
            case 4:
                //StartCoroutine(ShootBigProjectiles());
                break;
        }        
    }

    private void ShootProjectiles()
    {
        isDoingAttack = true;
        beingUsedByBoss = true;
        int randomNumberOfProjectiles = Random.Range(3, 12);
        
        StartCoroutine(SpawnProjectileLogic(randomNumberOfProjectiles));
    }

    private IEnumerator SpawnProjectileLogic(int randomNumberOfProjectiles)
    {
        for (int i = 0; i < randomNumberOfProjectiles; i++)
        {
            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            float randomAngle = Random.Range(-randomAngleRange, randomAngleRange);  // Get random angle deviation
            float adjustedBulletSpeed = projectileSpeed + Random.Range(-randomSpeedRange, randomSpeedRange);  // Get random speed deviation 

            Vector2 playerDirection = (target.transform.position - transform.position).normalized;  // Get direction towards the player
            Vector2 shootDirection = Quaternion.Euler(0, 0, randomAngle) * playerDirection;  // Apply random angle deviation

            bulletRb.velocity = shootDirection * adjustedBulletSpeed;
            
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator Waiter(float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);
    }

    private void ShootBeam()
    {
        isDoingAttack = true;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().velocity = transform.up * projectileSpeed;
        isDoingAttack = false;
    }

    private void AOEAttack()
    {
        isDoingAttack = true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                playerController.TakeDamage(5);
            }
        }
        isDoingAttack = false;
    }

    private void ShootBigProjectiles()
    {
        isDoingAttack = true;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().velocity = transform.up * projectileSpeed;
        isDoingAttack = false;
    }

    private void Teleport()
    {
        // Add logic to use Ability 2
        // Example: Teleport within a 6 unit radius (if not obstructed)
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * 6f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, randomPosition - (Vector2)transform.position, 6f);
        
        if (hit.collider == null || !hit.collider.CompareTag("Wall"))
        {
            transform.position = randomPosition;
            lastTeleportTime = Time.time; // Update the last teleport time
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DamagingObject"))
        {
            float randInt = Random.Range(0, 1);
            if (randInt == 0)
            {
                // Generate a random float between 0 and 1, and check if it's less than the chance to deflect an attack
                float randomFloat = Random.Range(0f, 1f);
                if (randomFloat <= chanceToDeflectAttack)
                {
                    // Deflect the attack
                    Vector2 direction = (transform.position - other.transform.position).normalized;
                    other.gameObject.GetComponent<Rigidbody2D>().velocity = direction * 10f;
                }
                else
                {
                    TakeDamage();
                }
            }
            else
            {
                // Generate a random float between 0 and 1, and check if it's less than the chance to dodge an attack
                float randomFloat = Random.Range(0f, 1f);
                if (randomFloat <= chanceToDodgeAttack)
                {
                    Teleport();
                }
                else
                {
                    TakeDamage();
                }
            }
        } 
        else if (other.gameObject.CompareTag("Player") && lastAttackTime >= attackCooldown)
        {
            playerController.TakeDamage(3);
        }
    }

    protected override void Die()
    {
        base.Die();
        // Additional logic for The Wizard's death behavior
        // Example: Drop special loot or trigger a cutscene
    }
}

