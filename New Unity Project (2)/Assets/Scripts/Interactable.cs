using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float speed;

    bool isEnabled = false;
    Animator animator;
    int setActiveHash = Animator.StringToHash("setActive");
    bool setActive = false;

    void Start() {
        FindObjectOfType<PlayerController>().PlayerInteraction += PlayerInteraction;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.speed = speed;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player") {
            isEnabled = true;
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.tag == "Player") {
            isEnabled = false;
        }
    }

    void PlayerInteraction() {
        if (isEnabled) {
            setActive = !setActive;
            animator.SetBool(setActiveHash, setActive);
        }
    }

}
