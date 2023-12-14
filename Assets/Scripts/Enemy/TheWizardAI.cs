using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting;

// TODO:
// Speed isnt working! set it to zero, and he still moves
// Beam shouldnt be instant, maybe some warning somehow, or maybe the beam "sweeps" from one direction to another
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
    private float attackCooldownTime = 0.5f;
    private float lastAttackTimer = 0f;

    private float startHealth;
    private float startSpeed;
    public float halfHealthSpeedBuff = 1.2f;
    private bool isDoingAttack = false;
    public float chanceToDeflectAttack = 0.05f;
    public float chanceToDodgeAttack = 0.1f;
    private bool isMoving = false;

    public float beamDuration = 0.5f;

    protected override void Start()
    {
        base.Start();

        health = 150;

        attackCooldown = 5f;
        attackCooldownTime = Random.Range(0.5f, 2.5f);

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
            lastTeleportTime = Time.deltaTime;
        }

        if (Time.deltaTime - lastTeleportTime >= teleportCooldown && playerController.playerIsShooting)
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

        if (!isDoingAttack && !beingUsedByBoss)
        {
            lastAttackTimer += Time.deltaTime;
        }
        else
        {
            lastAttackTimer = 0f;
        }
    }

    protected override void RotateToTarget()
    {
        if (target.CompareTag("Player"))
        {
            Vector2 targetDir = target.position - transform.position;
            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90f;
            Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, q, rotateSpeed);

        }

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
        if (!isDoingAttack && lastAttackTimer >= attackCooldownTime)
        {
            int randomAttack = Random.Range(1, 3);
            AllMighty(randomAttack);
        }
    }

    private void AllMighty(int randomAttack)
    {
        switch (randomAttack)
        {
            case 1:
                ShootProjectiles();
                break;
            case 2:
                ShootBeam();
                break;
        }    
        
        beingUsedByBoss = false;    
    }

private bool isShooting = false;

private IEnumerator ShootProjectilesCoroutine()
{
    isShooting = true;
    isDoingAttack = true;
    beingUsedByBoss = true;

    int randomNumberOfProjectiles = Random.Range(3, 12);
    for (int i = 0; i < randomNumberOfProjectiles; i++)
    {
        SpawnProjectileLogic();
        yield return new WaitForSeconds(shootingCooldown);
    }

    lastAttackTimer = 0f;
    isDoingAttack = false;
    beingUsedByBoss = false;
    isShooting = false;
}

private void ShootProjectiles()
{
    if (!isShooting) StartCoroutine(ShootProjectilesCoroutine());
}

    private void SpawnProjectileLogic()
    {
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        float randomAngle = Random.Range(-randomAngleRange, randomAngleRange);  // Get random angle deviation
        float adjustedBulletSpeed = projectileSpeed + Random.Range(-randomSpeedRange, randomSpeedRange);  // Get random speed deviation 

        Vector2 playerDirection = (target.transform.position - transform.position).normalized;  // Get direction towards the player
        Vector2 shootDirection = Quaternion.Euler(0, 0, randomAngle) * playerDirection;  // Apply random angle deviation

        bulletRb.velocity = shootDirection * adjustedBulletSpeed;

        if (target.CompareTag("Player")) return;
    }

private void ShootBeam()
{
    if (target.CompareTag("Player"))
    {
        isDoingAttack = true;
        beingUsedByBoss = true;

        GameObject beam = GameObject.Find("EnemyBeam");
        LineRenderer lineRenderer;
        if (beam == null) {
            beam = new GameObject("EnemyBeam");
            lineRenderer = beam.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.colorGradient = new Gradient {
                colorKeys = new [] { new GradientColorKey(lineRenderer.startColor, 0.0f), new GradientColorKey(lineRenderer.endColor, 1.0f) },
                alphaKeys = new [] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            };
        } else {
            lineRenderer = beam.GetComponent<LineRenderer>();
        }

        Vector3 beamOffset = new Vector3(0f, 0.5f, 0f);
        lineRenderer.SetPosition(0, transform.position + beamOffset);
        float beamLength = 5f;
        LayerMask mask = LayerMask.GetMask("Wall", "Player");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.transform.position - transform.position).normalized, beamLength, mask);
        if (hit.collider != null) {
            if (hit.collider.gameObject.CompareTag("Player")) hit.collider.gameObject.GetComponent<Movement>().TakeDamage(2f);
            lineRenderer.SetPosition(1, hit.point);
        }

        StartCoroutine(DeactivateBeam(beam, beamDuration));
    }
}

private IEnumerator DeactivateBeam(GameObject beam, float duration)
{
    yield return new WaitForSeconds(duration);

    // After the duration, deactivate the beam
    if (beam != null) {
        LineRenderer lineRenderer = beam.GetComponent<LineRenderer>();
        if (lineRenderer != null) {
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
        }
        beam.SetActive(false);
    }
    isDoingAttack = false;
    beingUsedByBoss = false;
}

    private void Teleport()
    {
        // Add logic to use Ability 2
        // Example: Teleport within a 6 unit radius (if not obstructed)
        float randomDistance = Random.Range(1f, 4f);
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * randomDistance;
        LayerMask mask = LayerMask.GetMask("Wall", "Player");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, randomPosition - (Vector2)transform.position, randomDistance, mask);
        
        if (hit.collider == null || !hit.collider.gameObject.CompareTag("Wall"))
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
            playerController.TakeDamage(1);
        }
    }

    protected override void Die()
    {
        base.Die();
        // Additional logic for The Wizard's death behavior
        // Example: Drop special loot or trigger a cutscene
    }
}

