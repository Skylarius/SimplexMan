using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : Controller {

    [Header("Camera")]
    public float cameraRotSpeed = 1;
    public Vector2 cameraRotationXRange = new Vector2(25, 35);

    public event System.Action PlayerInteraction;
    public event System.Action StopPlayerInteraction;
    public event System.Action StartRecording;
    public event System.Action StopRecording;

    bool recordDown;
    bool recordUp;
    
    float currentCameraRotX = 0f;
    Camera myCamera;

    Vector3 initialPosition;
    Quaternion initialRotation;

    protected override void Start(){
        base.Start();
        myCamera = GetComponentInChildren<Camera>();
	}

    // Input should be taken in Update
    // Non physical movement should be done here as well
    protected override void Update() {
        base.Update();

        // Camera Input
        float xRot = Input.GetAxisRaw("Mouse Y");
        currentCameraRotX -= xRot * cameraRotSpeed;
        currentCameraRotX = Mathf.Clamp(currentCameraRotX, cameraRotationXRange.x, cameraRotationXRange.y);
        myCamera.transform.localEulerAngles = new Vector3(currentCameraRotX, 0f, 0f); 

        // Interaction Input
        if (interactDown) {
            PlayerInteraction();
        }
        if (interactUp) {
            StopPlayerInteraction();
        }

        // Cloning Input
        if (recordDown){
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            StartRecording();         
        }
        if (recordUp){
            StopRecording();
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }
    }

    protected override void GetInputs() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxisRaw("Mouse X");
        jump = Input.GetButtonDown("Jump");
        interactDown = Input.GetButtonDown("Interact");
        interactUp = Input.GetButtonUp("Interact");

        recordDown = Input.GetButtonDown("Record");
        recordUp = Input.GetButtonUp("Record");
    }
}

    // IEnumerator Record(Recordings recordings) {
    //     while (true) {
    //         recordings.position.Add(transform.position);
    //         recordings.rotation.Add(transform.rotation);
    //         recordings.scale.Add(transform.localScale);
    //         recordings.velocity.Add(rb.velocity);
    //         // if (isDownInteract) {
    //         //     recordings.isInteracting.Add(true);
    //         // } else {
    //         //     recordings.isInteracting.Add(false);
    //         // }
    //         yield return null;
    //     }
        
    // }

    // IEnumerator SymplexMan(Recordings recordings) {
    //     // recordings.isInteracting = recordings.CleanList(recordings.isInteracting);
        
    //     // transform.position = recordings.position[0];
    //     // transform.rotation = recordings.rotation[0];
    //     // transform.localScale = recordings.scale[0];
    //     // rb.velocity = recordings.velocity[0];
    //     // if (recordings.isInteracting[0]) {
    //     //     PlayerInteraction();
    //     // }

    //     // GameObject clone = Instantiate(clonePrefab, 
    //     //                                recordings.position[0], 
    //     //                                recordings.rotation[0]);

    //     // Rigidbody cloneRB = clone.GetComponent<Rigidbody>();
    //     // Transform cloneT = clone.GetComponent<Transform>();

    //     // for (int i = 0; i < recordings.velocity.Count; i++) {
    //     //     cloneT.position = recordings.position[i];
    //     //     cloneT.rotation = recordings.rotation[i];
    //     //     cloneT.localScale = recordings.scale[i];
    //     //     cloneRB.velocity = recordings.velocity[i];
    //     //     if (recordings.isInteracting[i]) {
    //     //         PlayerInteraction();
    //     //     }
    //     //     yield return null;
    //     // }
    //     // Vector3 deathPosition = cloneT.position;
    //     // Destroy(clone);
    //     // Destroy(Instantiate(deathEffect, deathPosition, Quaternion.identity), 2);
    //     yield return null;
    // }

    

    // class Recordings {
    //     public List<Vector3> position = new List<Vector3>();
    //     public List<Quaternion> rotation = new List<Quaternion>();
    //     public List<Vector3> scale = new List<Vector3>();
    //     public List<Vector3> velocity = new List<Vector3>();
    //     public List<bool> isInteracting = new List<bool>();

    //     public List<bool> CleanList(List<bool> dirtyList) {
    //         List<bool> list = dirtyList;
    //         bool isCleaning = false;
    //         for (int i = 0; i < list.Count; i++) {
    //             if (list[i] == false) {
    //                 isCleaning = false;
    //             } else if (list[i] == true && !isCleaning) {
    //                 isCleaning = true;
    //             } else {
    //                 list[i] = false;
    //             }
    //         }
    //         return list;
    //     }
    // }
