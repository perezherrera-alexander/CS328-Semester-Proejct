using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpells : MonoBehaviour {
    ManaBar manaBar;
    Movement movement;

    // This will be so you cant spam spells
    float castDelayInvis = 0f;
    float castDelayHaste = 0f;

    bool canBeTargeted = true;

    void Start() {
        manaBar = GetComponent<ManaBar>();
        movement = GetComponent<Movement>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q) && movement.currentMana > 0 && castDelayInvis <= 0) {
            Invisibility();
            movement.UseMana(1);
            castDelayInvis = 15f;
            canBeTargeted = false;
        } else if (Input.GetKeyDown(KeyCode.E) && movement.currentMana > 0 && castDelayHaste <= 0) {
            Haste();
            movement.UseMana(1);
            castDelayHaste = 15f;
        } else {
            castDelayInvis -= Time.deltaTime;
            castDelayHaste -= Time.deltaTime;
        }
    }

    void Invisibility() {
        // Make the player invisible for a short time

        StartCoroutine(InvisibilityTimer());
    }

    IEnumerator InvisibilityTimer() {

        yield return new WaitForSeconds(5);
        canBeTargeted = true;
    }

    void Haste() {
        // Make the player faster for a short time
        movement.movementSpeed *= 2;
        StartCoroutine(HasteTimer());
    }

    IEnumerator HasteTimer() {
        yield return new WaitForSeconds(5);
        movement.movementSpeed /= 2;
    }
}
