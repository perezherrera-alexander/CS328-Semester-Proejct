using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float chanceToDodgeAttack = 0.4f;
    private bool isMoving = false;

    public float beamDuration = 0.5f;

    public float beamLength = 4f;

    //private bool beamExists = false;

    protected override void Start()
    {
        base.Start();

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

        /*
        if (Time.deltaTime - lastTeleportTime >= teleportCooldown && playerController.playerIsShooting)
        {
            Teleport();
        }
        */

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
                rb.velocity = transform.right * speed * 0.5f;
            }
            else
            {
                rb.velocity = -transform.right * speed * 0.5f;
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
            int randomAttack = Random.Range(1, 3); // This 3 is here so that he isn't always attacking.
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
                StartCoroutine(ShootBeam());
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
        if (!isShooting)
            StartCoroutine(ShootProjectilesCoroutine());
    }

    private void SpawnProjectileLogic()
    {
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<BossBulletBehavior>().setPlayerController(playerController.gameObject);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        // disable bullet collision with boss
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        float randomAngle = Random.Range(-randomAngleRange, randomAngleRange); // Get random angle deviation
        float adjustedBulletSpeed =
            projectileSpeed + Random.Range(-randomSpeedRange, randomSpeedRange); // Get random speed deviation

        Vector2 playerDirection = (target.transform.position - transform.position).normalized; // Get direction towards the player
        Vector2 shootDirection = Quaternion.Euler(0, 0, randomAngle) * playerDirection; // Apply random angle deviation

        bulletRb.velocity = shootDirection * adjustedBulletSpeed;

        if (target.CompareTag("Player"))
            return;
    }

    private IEnumerator ShootBeam()
    {
        if (target.CompareTag("Player"))
        {
            isDoingAttack = true;
            beingUsedByBoss = true;

            // Create visual effect to warn player
            CreateBeamEffect(transform.position);

            yield return new WaitForSeconds(1f);

            GameObject beam = GameObject.Find("EnemyBeam");
            LineRenderer lineRenderer;
            if (beam == null)
            {
                beam = new GameObject("EnemyBeam");
                lineRenderer = beam.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(
                    Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply")
                );
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.yellow;
                lineRenderer.colorGradient = new Gradient
                {
                    colorKeys = new[]
                    {
                        new GradientColorKey(lineRenderer.startColor, 0.0f),
                        new GradientColorKey(lineRenderer.endColor, 1.0f)
                    },
                    alphaKeys = new[]
                    {
                        new GradientAlphaKey(1.0f, 0.0f),
                        new GradientAlphaKey(1.0f, 1.0f)
                    }
                };
                lineRenderer.startWidth = 0.2f;
                lineRenderer.endWidth = 0.2f;
            }
            else
            {
                lineRenderer = beam.GetComponent<LineRenderer>();
            }

            bool foundPlayer = false;
            Vector3 beamOffset = new Vector3(0f, 0.5f, 0f);
            lineRenderer.SetPosition(0, transform.position + beamOffset);
            
            LayerMask mask = LayerMask.GetMask("Wall", "Player");
            RaycastHit2D[] hit = Physics2D.RaycastAll(
                transform.position,
                (target.transform.position - transform.position).normalized,
                beamLength,
                mask
            );
            foreach (RaycastHit2D h in hit)
            {
                if (h.collider != null)
                {
                    if (h.collider.gameObject.CompareTag("Player"))
                        h.collider.gameObject.GetComponent<Movement>().TakeDamage(2f);
                    lineRenderer.SetPosition(1, h.point);
                    foundPlayer = true;
                    break;
                }
            }
            if (!foundPlayer)
                lineRenderer.SetPosition(
                    1,
                    transform.position
                        + (target.transform.position - transform.position).normalized * beamLength
                );

            StartCoroutine(DoAThingOverTime(lineRenderer, beamDuration));
            StartCoroutine(DeactivateBeam(beam, beamDuration));
        }
        yield return null;
    }

    private IEnumerator DeactivateBeam(GameObject beam, float duration)
    {
        yield return new WaitForSeconds(duration);

        // After the duration, deactivate the beam
        if (beam != null)
        {
            LineRenderer lineRenderer = beam.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.zero);
            }
            beam.SetActive(false);
        }
        isDoingAttack = false;
        beingUsedByBoss = false;
        Destroy(beam);
    }

    private void Teleport()
    {
        // Add logic to use Ability 2
        // Example: Teleport within a 6 unit radius (if not obstructed)
        float randomDistance = Random.Range(2f, 9f);
        Vector2 randomPosition =
            (Vector2)transform.position + Random.insideUnitCircle * randomDistance;
        LayerMask mask = LayerMask.GetMask("Wall", "Player");
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            randomPosition - (Vector2)transform.position,
            randomDistance,
            mask
        );

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
            Debug.Log("Health: " + health);
            TakeDamage();
            /*
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
            */
        }
        else if (other.gameObject.CompareTag("Player") && lastAttackTime >= attackCooldown)
        {
            playerController.TakeDamage(2);
            Debug.Log("Player Health: " + playerController.currentHealth);
        }
    }

    protected override void Die()
    {
        base.Die();
        // Additional logic for The Wizard's death behavior
        // Example: Drop special loot or trigger a cutscene
        SceneManager.LoadScene("EndScreen");
    }

    private void CreateBeamEffect(Vector3 hitLocation)
    {
        GameObject beamEffect = new GameObject("BeamEffect");
        ParticleSystem particleSystem = beamEffect.AddComponent<ParticleSystem>();

        // Set the position of the effect to the hit location
        beamEffect.transform.position = hitLocation;

        var main = particleSystem.main;
        main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.09f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startColor = Color.red;
        //main.remain
        main.startLifetime = 0.08f;
        //main.duration = 0.05f;

        var emission = particleSystem.emission;
        emission.rateOverTime = 200;

        // Create a simple red material for the particles
        Material redMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        redMaterial.color = Color.red;

        particleSystem.GetComponent<ParticleSystemRenderer>().material = redMaterial;

        particleSystem.Play();
        // Stop the particle system after 0.05 seconds
        StartCoroutine(StopParticleSystemAfterDelay(particleSystem, 0.08f));
    }

    private IEnumerator StopParticleSystemAfterDelay(ParticleSystem particleSystem, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Stop the particle system
        particleSystem.Stop();
        // Delte the particle system after 1 second
        Destroy(particleSystem.gameObject, 1f);
    }

    IEnumerator DoAThingOverTime(LineRenderer lineRenderer, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            //someColorValue = Color.Lerp(start, end, normalizedTime);
            lineRenderer.colorGradient = new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(lineRenderer.startColor, 0.0f),
                    new GradientColorKey(lineRenderer.endColor, 1.0f)
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(1 - normalizedTime, 0.0f),
                    new GradientAlphaKey(1 - normalizedTime, 1.0f)
                }
            };
            yield return null;
        }
        //someColorValue = end; //without this, the value will end at something like 0.9992367
    }
}
