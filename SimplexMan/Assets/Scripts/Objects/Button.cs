using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : RecordablePress {
    
    public ElectricWall objectToChange;

    public float speed;
    Animator animator;
    int setActiveHash = Animator.StringToHash("setActive");
    bool isEnabled = false;

    public override void Start() {
        base.Start();

        animator = GetComponent<Animator>();
        animator.speed = speed;
        animator.SetBool(setActiveHash, isActive);

        FindObjectOfType<PlayerController>().PlayerInteraction += PlayerInteraction;
    }

    public override void Update() {
        base.Update();
    }

    // Interaction functions
    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            isEnabled = true;
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            isEnabled = false;
        }
    }

    void PlayerInteraction() {
        if (isEnabled) {
            isActive = !isActive;
            animator.SetBool(setActiveHash, isActive);
            objectToChange.ChangeState(isActive);
        }
    }

    protected override void ResetState(bool _isActive) {
        animator.SetBool(setActiveHash, _isActive);
        objectToChange.ChangeState(_isActive);
    }
}
