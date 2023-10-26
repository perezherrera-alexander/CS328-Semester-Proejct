using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public float movementSpeed;
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

    private int dashes = 0;

    // Start is called before the first frame update

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        movementSpeed /= 10;

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
        if(dashes > 0) {
            dashes--;
        }
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(inputX * movementSpeed, inputY * movementSpeed);

        rb.MovePosition(rb.position + movement);

        LookAtMouse();
        HandleShooting();
        HandleDash();
    }

    void LookAtMouse() {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - rb.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
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

    public void HandleDash() { // Dash in the direction of the mouse
        if(dashes > 0) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - rb.position;
            direction.Normalize();            
            rb.MovePosition(rb.position + (direction * 0.4f));
            //rb.AddForce(direction * 10, ForceMode2D.Force);
            // There's a better way to do this with AddForce, will revisit later
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if(dashes == 0) {
                Debug.Log("Dashing");
                dashes = 10;
            }
        }
    }
}
