using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    public bool isAlive = true;
    public float movementSpeed;
    public float dashSpeed = 200f;
    public int dashDuration = 5; // In frames (0.2s) (1s)
    public int dashCooldownTimer = 25; // In frames (0.2s) (5s)
    public DashBar dashBar;
    private Rigidbody2D rb;

    public int maxHealth = 20;
    public float currentHealth;
    public HealthBar healthBar;

    public float maxMana = 100;
    public float currentMana;
    public ManaBar manaBar;
    public float manaRegenRate = 0.02f;
    public float manaEmptyCooldown = 250f;
    public float manaCooldownTimer = 0;

    public GameObject bulletPrefab;
    public WeaponType weaponType;
    public float bulletSpeed = 10f;
    public float shootingCooldown = 0.05f;
    private float nextShotTime = 0f;
    public float beamDamageTime = 0f;
    private float timeSinceLastBeam = 0f;
    public float randomAngleRange = 5f;
    public float randomSpeedRange = 2f;
    public int bulletDamage = 2;
    public bool playerIsShooting = false;
    #pragma warning disable 0414
    bool isPaused;
    #pragma warning restore 0414

    private int dashCountdown = 0;
    private int dashCooldown = 0;
    private bool dashing = false;

    private float maxBeamLength = 100f;

    public AudioClip shootSound;
    public AudioClip dashSound;
    public AudioClip playerDeathSound;

    public ParticleSystem explosionEffect;
    public Renderer playerRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        movementSpeed /= 10;

        dashBar.SetMaxDash(dashCooldownTimer);

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentMana = maxMana;
        manaBar.SetMaxMana(maxMana);

        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAlive){
            // set tag to something that can't be hit
            gameObject.tag = "Untagged";
        }

        if(weaponType == WeaponType.Basic) {
            shootingCooldown = 0.05f;
        }
        else if(weaponType == WeaponType.Beam) {
            shootingCooldown = 0f;
        }
        else if(weaponType == WeaponType.Charge) {
            shootingCooldown = 0.1f;
        }
        timeSinceLastBeam += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DamagingObjectFromEnemy"))
        {
            
            TakeDamage(0.1f);
        }
    }

    // Called once per 0.2 seconds
    void FixedUpdate()
    {
        if(isAlive){
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");
            Vector2 movement = new Vector2(inputX * movementSpeed, inputY * movementSpeed); // Movement vector
    
            rb.MovePosition(rb.position + movement); // Move the player
    
            // Get the player to look at the mouse
            Vector2 aimDirection = GetDirectionToMouse(); // Saving this for later so that we don't compute it twice (in LookAtMouse() and in HandleDash())
            
            LookAtMouse(aimDirection);
            HandleShooting(aimDirection);
    
            // Dash Cooldown Handling
            if(dashing) {
                dashCountdown--;
            }
            if(dashCooldown > 0) {
                dashCooldown--;
            }
            HandleDash(aimDirection);
    
            if (currentMana >= maxMana)
            {
                currentMana = maxMana;
            }
            else 
            {
                currentMana += manaRegenRate;
    
                manaBar.SetMana(currentMana);
            }
        }
    }

    Vector2 GetMousePosition() { // Returns the mouse position in world coordinates
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    Vector2 GetDirectionToMouse() { // Returns the direction from the player to the mouse
        Vector2 mousePosition = GetMousePosition();
        Vector2 direction = mousePosition - rb.position;
        direction.Normalize();
        return direction;
    }

    void LookAtMouse(UnityEngine.Vector2 mouseDirection) { // Moved this functionality into FixedUpdate() since it's trivial
        float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void HandleShooting(UnityEngine.Vector2 mouseDirection) {
        if (!isAlive) return;
        if (Input.GetMouseButton(0) && Time.time > nextShotTime)  // Left mouse button
        {
            if(weaponType == WeaponType.Basic) {
                Shoot();
                nextShotTime = Time.time + shootingCooldown;
            }
            else if(weaponType == WeaponType.Beam) {
                FireBeam(mouseDirection);
                nextShotTime = Time.time + shootingCooldown;
            }
            else if(weaponType == WeaponType.Charge) {
                // TODO: Charge weapon
                Debug.Log("Charge weapon");
            }
        }
        else {
            Destroy(GameObject.Find("Beam"));
            playerIsShooting = false;
        }
    }

    void FireBeam(UnityEngine.Vector2 mouseDirection) {
        playerIsShooting = true;

        // Create a new GameObject for the beam if it doesn't exist yet
        GameObject beam = GameObject.Find("Beam");
        LineRenderer lineRenderer;
        if(beam == null) {
            beam = new GameObject("Beam");
            lineRenderer = beam.AddComponent<LineRenderer>();
        }
        else {
            lineRenderer = beam.GetComponent<LineRenderer>();
        }

        // Set other properties of the LineRenderer, such as color, width, etc.
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startColor = Color.blue;  
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(lineRenderer.startColor, 0.0f), new GradientColorKey(lineRenderer.endColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

        
        

        // Offset the beam's start position so that it doesn't collide with the player
        Vector3 beamOffset = new Vector3(0f, 0.5f, 0f);
        beamOffset = Quaternion.Euler(0, 0, rb.rotation) * beamOffset;
        lineRenderer.SetPosition(0, transform.position + beamOffset);


        LayerMask mask = LayerMask.GetMask("Wall", "Enemy", "Goblin", "Boss"); // Layer 8 & 9
        // This mask wouldn't hit the suit of armor for some reason so I removed it.
        // Raycast to the nearest object in the direction of the mouse
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.up, maxBeamLength);

        bool hitWall = false;
        float closestHitDistance = maxBeamLength;
        List<GameObject> enemiesHit = new List<GameObject>();

        // Find if the raycast hit a wall or an enemy. Set the beam to the closest wall. Add enemies to a list to damage them later
        foreach(RaycastHit2D h in hit) {
            if(h.collider.gameObject.CompareTag("Wall")) {
                if(h.fraction < closestHitDistance) {
                    closestHitDistance = h.fraction;
                    lineRenderer.SetPosition(1, h.point);
                    hitWall = true;
                }
            }
            else if(h.collider.gameObject.CompareTag("Enemy") || h.collider.gameObject.CompareTag("Goblin")) {
                enemiesHit.Add(h.collider.gameObject);
            }
        }
        if(!hitWall) {
            lineRenderer.SetPosition(1, transform.position + transform.up * maxBeamLength);
        }
        // Damage enemies hit by the beam (if any) but only once every interval (beamDamageTime)
        if(enemiesHit.Count > 0 && timeSinceLastBeam > beamDamageTime) {
            foreach(GameObject enemy in enemiesHit) {
                if(enemy != null) {
                    CreateBeamEffect(enemy.transform.position);
                    if(enemy.GetComponent<EnemyAI>().enemyName == "Armor") {
                        enemy.GetComponent<ArmorAI>().InflictDamage();
                    }
                    else if(enemy.GetComponent<EnemyAI>().enemyName == "Ghost") {
                        enemy.GetComponent<GhostAI>().InflictDamage();
                    }
                    else if(enemy.GetComponent<EnemyAI>().enemyName == "Goblin") {
                        enemy.GetComponent<GoblinAI>().TakeDamage();
                    }
                    else if(enemy.GetComponent<EnemyAI>().enemyName == "Minion") {
                        enemy.GetComponent<MinionAI>().TakeDamage();
                    }
                    else if(enemy.GetComponent<EnemyAI>().enemyName == "Skeleton") {
                        enemy.GetComponent<SkeletonAI>().InflictDamage();
                    }
                    else if(enemy.GetComponent<EnemyAI>().enemyName == "Skeleton King")
                    {
                        enemy.GetComponent<SkeletonKingAI>().TakeDamage();
                    }
                    else if(enemy.GetComponent<EnemyAI>().enemyName == "Wizard"){
                        enemy.GetComponent<TheWizardAI>().TakeDamage();
                    }
                    else {
                        Debug.Log("Uh oh, someone didn't add a case for this enemy (Alexander's fault)");
                        //Debug.Log(enemy.GetComponent<EnemyAI>().enemyName);
                    }
                }
            }
            timeSinceLastBeam = 0;
        }

        // No need to destroy the beam, we'll reuse it as long as the player is holding down click. We'll destroy it later on if the player stop's clicking
    }

    public void Shoot() {
        if (currentMana - bulletDamage <= 0) {
            currentMana = 0;
            return;
        }
        else
        {
            playerIsShooting = true;
        
            // Play shooting sound
            AudioSource.PlayClipAtPoint(shootSound, transform.position);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        
            float randomAngle = Random.Range(-randomAngleRange, randomAngleRange);  // Get random angle deviation
            float adjustedBulletSpeed = bulletSpeed + Random.Range(-randomSpeedRange, randomSpeedRange);  // Get random speed deviation 

            Vector2 shootDirection = Quaternion.Euler(0, 0, randomAngle) * transform.up;  // Apply random angle deviation

            bulletRb.AddForce(shootDirection * adjustedBulletSpeed, ForceMode2D.Impulse);

            playerIsShooting = true;
        
            /* For mana cooldown, not currently working
            if (currentMana == 0 && Input.GetMouseButton(0) && manaCooldownTimer < manaEmptyCooldown)
            {
                manaCooldownTimer += Time.deltaTime;
            }
            else if (currentMana == 0 && Input.GetMouseButton(0) && manaCooldownTimer >= manaEmptyCooldown)
            {
                UseMana(bulletDamage);

                manaCooldownTimer = 0;
            } 
            else if (currentMana > 0)
            {
                UseMana(bulletDamage);
            }
            */
            UseMana(bulletDamage);
        }
    }

    public void TakeDamage(float damage) {
        if (!isAlive) return;
        currentHealth -= damage;
    
        if (currentHealth <= 0) {
            currentHealth = 0;
            isAlive = false;
            
            // Play the death sound
            AudioSource.PlayClipAtPoint(playerDeathSound, transform.position);
    
            // Play explosion effect and change color to red
            CreateExplosionEffect();
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null) {
                renderer.enabled = false;
            }
    
            // Disable colliders
            Collider collider = GetComponent<Collider>();
            if (collider != null) {
                collider.enabled = false;
            }

            // Disable the Rigidbody to stop all physics interactions
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true; // Stops the Rigidbody from reacting to physics
                rb.velocity = Vector3.zero; // Optionally, immediately stop any movement
            }

            // Start the coroutine to wait and then reload the scene
            StartCoroutine(ReloadSceneAfterDelay());
        }
    
        healthBar.SetHealth((int)currentHealth);
    }

    private void CreateExplosionEffect() {
        GameObject explosionEffect = new GameObject("ExplosionEffect");
        ParticleSystem particleSystem = explosionEffect.AddComponent<ParticleSystem>();
    
        // Set the position of the explosion to the player's position
        explosionEffect.transform.position = transform.position;
    
        var main = particleSystem.main;
        main.startSize = new ParticleSystem.MinMaxCurve(0.10f, 0.3f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startLifetime = 0.2f;

        var emission = particleSystem.emission;
        emission.rateOverTime = 200;

    
        // Create a simple red material for the particles
        Material redMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        redMaterial.color = Color.red;
        particleSystem.GetComponent<ParticleSystemRenderer>().material = redMaterial;

        particleSystem.Play();
    }


    private IEnumerator ReloadSceneAfterDelay() {
        // Wait for 1 second
        yield return new WaitForSeconds(2);
    
        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UseMana(int mana) {
        if (currentMana - mana <= 0) {
            currentMana = 0;
        } else {
            currentMana -= mana;
        }

        manaBar.SetMana(currentMana);
    }

    public void HandleDash(UnityEngine.Vector2 direction) { // Dash in the direction of the mouse
        if (!isAlive) return;
        dashBar.SetDash(dashCooldown);
        if(dashCountdown <= 0) {
            dashing = false;
        }
        if(Input.GetKey("space")) {
            if(dashCooldown > 0)
            {
                // Debug.Log("Dash on cooldown");
            }
            else
            {
                // Play dash sound
                AudioSource.PlayClipAtPoint(dashSound, transform.position);
                if(!dashing) {
                    dashCountdown = dashDuration;
                    dashCooldown = dashCooldownTimer;
                    dashing = true;
                    //Debug.Log("Dashing");
                    direction.Normalize(); // Otherwise the dash is faster when the mouse is further away from the player          
                    rb.AddForce(direction * dashSpeed, ForceMode2D.Force);
                }
            }
            
        }
        if(dashing){
            direction.Normalize(); // Otherwise the dash is faster when the mouse is further away from the player          
            rb.AddForce(direction * dashSpeed, ForceMode2D.Force);
        }
    }

    
    private void CreateBeamEffect(Vector3 hitLocation) {
        GameObject beamEffect = new GameObject("BeamEffect");
        ParticleSystem particleSystem = beamEffect.AddComponent<ParticleSystem>();
    
        // Set the position of the effect to the hit location
        beamEffect.transform.position = hitLocation;
    
        var main = particleSystem.main;
        main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.09f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startColor = new Color(0.676f, 0.844f, 0.898f, 0.3f);
        //main.remain
        main.startLifetime = 0.08f;
        //main.duration = 0.05f;

        var emission = particleSystem.emission;
        emission.rateOverTime = 200;

    
        // Create a simple red material for the particles
        Material blueMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        blueMaterial.color = new Color(0.676f, 0.844f, 0.898f, 0.3f);

        particleSystem.GetComponent<ParticleSystemRenderer>().material = blueMaterial;

        particleSystem.Play();
        // Stop the particle system after 0.05 seconds
        StartCoroutine(StopParticleSystemAfterDelay(particleSystem, 0.08f));
    }

    private IEnumerator StopParticleSystemAfterDelay(ParticleSystem particleSystem, float delay) {
        yield return new WaitForSeconds(delay);
    
        // Stop the particle system
        particleSystem.Stop();
    }
}
