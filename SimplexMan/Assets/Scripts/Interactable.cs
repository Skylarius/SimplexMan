// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Interactable : Recordable {
    
//     public float speed;

//     // Interaction variables
//     bool isEnabled = false;
//     Animator animator;
//     int setActiveHash = Animator.StringToHash("setActive");
//     bool setActive = false;

//     // Recording variables
//     bool initialActivatedState;

//     public override void Start() {
//         base.Start();
//         FindObjectOfType<PlayerController>().PlayerInteraction += PlayerInteraction;
//         animator = GetComponent<Animator>();
//         animator.speed = speed;
//     }

//     //
//     // Interaction functions
//     //

//     void OnTriggerEnter(Collider collider) {
//         if (collider.tag == "Player" || collider.tag == "Clone") {
//             isEnabled = true;
//         }
//     }

//     void OnTriggerExit(Collider collider) {
//         if (collider.tag == "Player" || collider.tag == "Clone") {
//             isEnabled = false;
//         }
//     }

//     void PlayerInteraction() {
//         if (isEnabled) {
//             setActive = !setActive;
//             animator.SetBool(setActiveHash, setActive);
//         }
//     }

//     //
//     // Recording functions
//     //

//     public override void StartRecording() {
//         initialActivatedState = setActive;
//         base.StartRecording();
//     }

//     public override void StopRecording() {
//         if (setActive != initialActivatedState) {
//             setActive = !setActive;
//             animator.SetBool(setActiveHash, setActive);
//         }
//         base.StopRecording();
//     }

// }
