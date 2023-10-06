using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public float movementSpeed = 10f;

    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(movementSpeed * inputX, movementSpeed * inputY, 0);

        movement *= Time.deltaTime;

        transform.Translate(movement);
    }

    void FixedUpdate()
    {
        
    }
}
