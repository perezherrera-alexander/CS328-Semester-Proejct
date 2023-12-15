using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAI : EnemyAI
{
    public enum GhostState
    {
        Floating,
        Invisible
    }

    public GhostState ghostState = GhostState.Floating;

#pragma warning disable 0414
    private bool isInvisible = false;
#pragma warning restore 0414
    private float floatSpeed;
    private float invisSpeed;

    protected override void Start()
    {
        base.Start();
        enemyName = "Ghost";

        floatSpeed = speed * 1.15f;
        invisSpeed = speed / 1.15f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        switch (ghostState)
        {
            case GhostState.Floating:
                rb.velocity = transform.up * floatSpeed;

                // Enable circle collider
                GetComponent<CircleCollider2D>().enabled = true;
                break;
            case GhostState.Invisible:
                isInvisible = true;
                rb.velocity = transform.up * invisSpeed;

                // Disable circle collider
                GetComponent<CircleCollider2D>().enabled = false;
                break;
        }

        if (!isInWall())
        {
            ghostState = GhostState.Floating;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DamagingObject"))
        {
            TakeDamage();
        }
        else if (other.gameObject.CompareTag("Player") && lastAttackTime >= attackCooldown)
        {
            other.gameObject.GetComponent<Movement>().TakeDamage(0.75f);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            // If the Ghost collides with a wall, change its state to invisible
            ghostState = GhostState.Invisible;
        }
    }

    public void InflictDamage()
    {
        TakeDamage();
    }

    private bool isInWall()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Wall"));
    }
}
