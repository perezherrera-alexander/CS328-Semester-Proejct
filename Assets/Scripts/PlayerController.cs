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

    public int maxHealth = 10;
    public int currentHealth;
    public HealthBar healthBar;

    public int maxMana = 10;
    public int currentMana;
    public ManaBar manaBar;

    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float shootingCooldown = 0.05f;
    private float nextShotTime = 0f;
    public float randomAngleRange = 5f;
    public float randomSpeedRange = 2f;

    bool isPaused;

    private int dashCountdown = 0;
    private int dashCooldown = 0;
    private bool dashing = false;

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

        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            UseMana(1);
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
        rb.rotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        
        HandleShooting();
        if(dashing) {
            dashCountdown--;
        }
        if(dashCooldown > 0) {
            dashCooldown--;
        }
        HandleDash(aimDirection);
    }

    Vector2 GetMousePosition() { // Returns the mouse position in world coordinates
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    Vector2 GetDirectionToMouse() { // Returns the direction from the player to the mouse
        Vector2 mousePosition = GetMousePosition();
        Vector2 direction = mousePosition - rb.position;
        return direction;
    }

    void LookAtMouse() { // Moved this functionality into FixedUpdate() since it's trivial
        Vector2 MouseDirection = GetDirectionToMouse();
        float angle = Mathf.Atan2(MouseDirection.y, MouseDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void HandleShooting() {
        if (Input.GetMouseButton(0) && Time.time > nextShotTime)  // Left mouse button
        {
            Shoot();
            nextShotTime = Time.time + shootingCooldown;
        }
    }

    void Shoot() {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
    
        float randomAngle = Random.Range(-randomAngleRange, randomAngleRange);  // Get random angle deviation
        float adjustedBulletSpeed = bulletSpeed + Random.Range(-randomSpeedRange, randomSpeedRange);  // Get random speed deviation 

        Vector2 shootDirection = Quaternion.Euler(0, 0, randomAngle) * transform.up;  // Apply random angle deviation

        bulletRb.AddForce(shootDirection * adjustedBulletSpeed, ForceMode2D.Impulse);
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
