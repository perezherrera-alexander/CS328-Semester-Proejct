using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWall : MonoBehaviour
{
    void Update()
    {
        if(GameObject.FindGameObjectWithTag("Boss") == null)
        {
            Destroy(gameObject);
        }
    }
}
