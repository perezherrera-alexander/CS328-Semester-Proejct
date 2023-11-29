using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float bulletLifetime = 2.0f;  // Bullet will exist for 2 seconds by default
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, bulletLifetime);  // Destroy the bullet after the specified lifetime
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall") // If the bullet collides with anything tagged as "Wall"
        {
            Destroy(gameObject); // Then destroy it
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            
            Destroy(gameObject);
        }
    }
}
