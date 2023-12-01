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

    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component


    void Start() {
        if(canBeTargeted) // This is just here to get rid of the warning

        manaBar = GetComponent<ManaBar>();
        movement = GetComponent<Movement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q) && movement.currentMana > 0 && castDelayInvis <= 0) {
            Invisibility();
            movement.UseMana(25);
            castDelayInvis = 15f;
        } else if (Input.GetKeyDown(KeyCode.E) && movement.currentMana > 0 && castDelayHaste <= 0) {
            Haste();
            movement.UseMana(20);
            castDelayHaste = 15f;
        } else {
            castDelayInvis -= Time.deltaTime;
            castDelayHaste -= Time.deltaTime;
        }
    }
    
    void Invisibility() {
        // Make the player invisible for a short time
        StartCoroutine(InvisibilityTimer());
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.2f);

    }

    IEnumerator InvisibilityTimer() {
        canBeTargeted = false;
        // Change the players tag temporarily
        gameObject.tag = "Invisible";
        yield return new WaitForSeconds(5);
        canBeTargeted = true;
        gameObject.tag = "Player";
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
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
