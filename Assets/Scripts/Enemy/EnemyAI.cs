using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public String enemyName;
    public float speed = 5f;
    public float rotateSpeed = 0.05f;
    protected Rigidbody2D rb;
    public float attackCooldown = 1f; // Add attack cooldown duration
    public float lastAttackTime; // Track the last attack time

    public int health = 10;

    public Movement playerController;

    public AudioClip enemyDeathSound;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            GetTarget();
        }
    }

    protected virtual void FixedUpdate()
    {
        rb.velocity = transform.up * speed;

        if (target != null)
        {
            RotateToTarget();
            AttackCooldown();
        }
    }

    protected virtual void GetTarget()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    protected virtual void RotateToTarget()
    {
        Vector2 targetDir = target.position - transform.position;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, rotateSpeed);
    }

    protected virtual void AttackCooldown()
    {
        if (Time.time - lastAttackTime >= attackCooldown) // Check if enough time has passed since the last attack
        {
            lastAttackTime = Time.time; // Update the last attack time
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DamagingObject"))
        {
            TakeDamage();
        }
        else if (other.gameObject.CompareTag("Player") && lastAttackTime >= attackCooldown)
        {
            playerController.TakeDamage(1);
        }
    }

    public virtual void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        playerController.increaseHealth(5);
        playerController.increaseMana(10);
        AudioSource.PlayClipAtPoint(enemyDeathSound, transform.position);
        Destroy(gameObject);
    }

    protected virtual void MoveTowardsTarget(Vector2 direction)
    {
        // Move towards the target
        rb.velocity = direction.normalized * speed;

        // Rotate towards the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, rotateSpeed);
    }
}
