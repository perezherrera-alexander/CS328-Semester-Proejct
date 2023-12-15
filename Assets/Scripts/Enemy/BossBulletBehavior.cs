using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletBehavior : MonoBehaviour
{
    public float bulletLifetime = 2.0f; // Bullet will exist for 2 seconds by default
    public GameObject playerController;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, bulletLifetime); // Destroy the bullet after the specified lifetime
    }

    public void setPlayerController(GameObject playerController)
    {
        this.playerController = playerController;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bullet Collision detected");
        if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject); // Then destroy it
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit");
            playerController.GetComponent<Movement>().TakeDamage(1);
            Debug.Log("Player health: " + playerController.GetComponent<Movement>().currentHealth);
            Destroy(gameObject);
        }
    }
}
