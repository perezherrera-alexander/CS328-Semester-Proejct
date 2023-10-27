using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHandler : MonoBehaviour {
    public Animator anim;

    void Update () {
        if (Input.GetKeyDown(KeyCode.Q)) {
            anim.GetComponent<Animator>().Play("InvisibilityAffect");
        }
    }
}
