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

    // Start is called before the first frame update

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentMana = maxMana;
        manaBar.SetMaxMana(maxMana);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // For Testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            UseMana(1);
        }
        */

    }

    void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(inputX * movementSpeed, inputY * movementSpeed);
        //Debug.Log(movement);

        rb.MovePosition(rb.position + movement);
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }

    public void UseMana(int mana) {
        currentMana -= mana;

        manaBar.SetMana(currentMana);
    }
}
