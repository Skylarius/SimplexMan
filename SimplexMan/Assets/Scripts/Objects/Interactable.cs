using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : RecordableInteraction {
    
    public float speed;
    Animator animator;
    int setActiveHash = Animator.StringToHash("setActive");

    public override void Start() {
        base.Start();
        animator = GetComponent<Animator>();
        animator.speed = speed;
    }

    public override void Update() {
        base.Update();
    }

    protected override void InteractionFunction(bool _isActive) {
        animator.SetBool(setActiveHash, _isActive);
    }
}
