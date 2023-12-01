using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectWithTag("Boss") == null)
        {
            Destroy(gameObject);
        }
    }
}
