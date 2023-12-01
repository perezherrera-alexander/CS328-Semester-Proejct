using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public float movementSpeed;
    public float dashSpeed = 200f;
    public int dashDuration = 5; // In frames (0.2s) (1s)
    public int dashCooldownTimer = 25; // In frames (0.2s) (5s)
    public DashBar dashBar;
    private Rigidbody2D rb;

    public int maxHealth = 20;
    public int currentHealth;
    public HealthBar healthBar;

    public float maxMana = 100;
    public float currentMana;
    public ManaBar manaBar;
    public float manaRegenRate = 0.02f;

    public GameObject bulletPrefab;
    public WeaponType weaponType;
    public float bulletSpeed = 10f;
    public float shootingCooldown = 0.05f;
    private float nextShotTime = 0f;
    public float randomAngleRange = 5f;
    public float randomSpeedRange = 2f;
    public int bulletDamage = 2;

    bool isPaused;

    private int dashCountdown = 0;
    private int dashCooldown = 0;
    private bool dashing = false;

    private float maxBeamLength = 100f;

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
        if(isPaused) {
            // This is just here to get rid of the warning
        }

        /*
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            UseMana(1);
        }
        */

        if(weaponType == WeaponType.Basic) {
            shootingCooldown = 0.05f;
        }
        else if(weaponType == WeaponType.Beam) {
            shootingCooldown = 0f;
        }
        else if(weaponType == WeaponType.Charge) {
            shootingCooldown = 0.1f;
        }
    }

    // Called once per 0.2 seconds
    void FixedUpdate()
    {
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
        }
    }

    void FireBeam(UnityEngine.Vector2 mouseDirection) {
        // Create a new GameObject for the beam if it doesn't exist yet
        GameObject beam = GameObject.Find("Beam");
        LineRenderer lineRenderer;
        BoxCollider2D collider; // Colider is here so that later on we can check if the beam hit anything
        if(beam == null) {
            beam = new GameObject("Beam");
            lineRenderer = beam.AddComponent<LineRenderer>();
            collider = beam.AddComponent<BoxCollider2D>();
        }
        else {
            lineRenderer = beam.GetComponent<LineRenderer>();
            collider = beam.GetComponent<BoxCollider2D>();
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

        // Ignore collisions between the beam and the player
        Physics2D.IgnoreCollision(collider, GetComponent<CircleCollider2D>());
        // Raycast to the nearest object in the direction of the mouse and only look for walls and enemies
        LayerMask mask = LayerMask.GetMask("Wall", "Enemy"); // Layer 8 & 9
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, maxBeamLength, mask);

        if (hit.collider != null) { // If the raycast hit something, update the beam's end position and the collider's size
            lineRenderer.SetPosition(1, hit.point);
            collider.size = new Vector2(0.2f, hit.distance);
        } else { // In case we don't hit anything, just make the beam (and collider) really long
            lineRenderer.SetPosition(1, transform.position + transform.up * maxBeamLength);
            collider.size = new Vector2(0.2f, maxBeamLength);
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
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        
            float randomAngle = Random.Range(-randomAngleRange, randomAngleRange);  // Get random angle deviation
            float adjustedBulletSpeed = bulletSpeed + Random.Range(-randomSpeedRange, randomSpeedRange);  // Get random speed deviation 

            Vector2 shootDirection = Quaternion.Euler(0, 0, randomAngle) * transform.up;  // Apply random angle deviation

            bulletRb.AddForce(shootDirection * adjustedBulletSpeed, ForceMode2D.Impulse);

            UseMana(bulletDamage);
        }
    }

    public void TakeDamage(int damage) {
        if (currentHealth - damage <= 0) {
            currentHealth = 0;
        } else {
            currentHealth -= damage;
        }

        healthBar.SetHealth(currentHealth);
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
}
