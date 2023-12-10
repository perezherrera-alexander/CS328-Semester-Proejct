using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelWall : MonoBehaviour
{
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
        {
            GetTarget();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            GetTarget();
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObject.FindGameObjectWithTag("Boss") == null && other.gameObject.CompareTag("Player"))
        {
            // If so, load the next level
            SceneManager.LoadScene("Level02");
        }
    }

    void GetTarget() {
        if (GameObject.FindGameObjectWithTag("Player")) 
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
