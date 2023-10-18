using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player"); // Camera follows the player
        position = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        position = player.transform.position;
        position.z = -10;
        transform.position = position;
    }
}
