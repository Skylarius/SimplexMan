using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    [Header("Movement")]
    public float speed = 15.0F;
    public float rotationSpeed = 5;
    public float jumpForce = 300;
    [Range(0, 1)]
    public float airFriction;
    public float stunnedTime;
    [Header("Camera")]
    public float cameraRotSpeed = 1;
    public Vector2 cameraRotationXRange = new Vector2(25, 35);
    [Header("Symplex Man")]
    public GameObject deathEffect;

    public event System.Action PlayerInteraction;
    public event System.Action StopPlayerInteraction;
    public event System.Action StartRecording;
    public event System.Action StopRecording;

    Recordings recordings;

    bool isStunned = false;
    
    float currentCameraRotX = 0f;
    Vector3 jumpStartVelocity;
    Camera myCamera;
    Rigidbody rb;
    Vector3 velocity;
    float rotation;
    bool jumpInput;

    Vector3 initialPosition;
    Quaternion initialRotation;

    void Start(){
        rb = GetComponent<Rigidbody>();
        myCamera = GetComponentInChildren<Camera>();
	}

    // Input should be taken in Update
    // Non physical movement should be done here as well
    void Update() {
        
        // Movement Input  
        Vector3 moveHorizontal = transform.right * Input.GetAxisRaw("Horizontal");
        Vector3 moveVertical = transform.forward * Input.GetAxisRaw("Vertical");
        Vector3 direction = (moveHorizontal + moveVertical).normalized; 
        velocity = direction * speed;

        // Rotation Input
        rotation = Input.GetAxisRaw("Mouse X");

        // Jump Input
        if (Input.GetButtonDown("Jump")) {
            jumpInput = true;
        }

        // Camera Input
        float xRot = Input.GetAxisRaw("Mouse Y");
        currentCameraRotX -= xRot * cameraRotSpeed;
        currentCameraRotX = Mathf.Clamp(currentCameraRotX, cameraRotationXRange.x, cameraRotationXRange.y);
        myCamera.transform.localEulerAngles = new Vector3(currentCameraRotX, 0f, 0f); 

        // Interaction Input
        if (Input.GetButtonDown("Interact")) {
            PlayerInteraction();
        }
        if (Input.GetButtonUp("Interact")) {
            StopPlayerInteraction();
        }

        // Cloning Input
        if (Input.GetButtonDown("Record")){
            //recordings = new Recordings();
            //Instantiate(clonePrefab, transform.position, transform.rotation);
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            StartRecording();         
        }
        if (Input.GetButtonUp("Record")){
            StopRecording();
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }
    }

    // Physics should be applied in FixedUpdate
	void FixedUpdate() {

        if (!isStunned) {
            if (IsGrounded()) {
                // Movement
                rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

                // Rotation
                Vector3 newRotation = new Vector3(0f, rotation, 0f) * rotationSpeed;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(newRotation));

                // Jump
                if (jumpInput) {
                    jumpInput = false;
                    jumpStartVelocity = velocity;
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            } else {
                // Jump Movement
                Vector3 airVelocity = jumpStartVelocity + new Vector3(velocity.x, 0, velocity.z) * (1 - airFriction);
                airVelocity = Vector3.ClampMagnitude(airVelocity, speed);
                rb.velocity = airVelocity + Vector3.up * rb.velocity.y;
            }
        }
    }

    bool IsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, transform.localScale.y + 0.5f);
    }

    public void Stun() {
        isStunned = true;
        StopCoroutine("RecoverFromStunned");
        StartCoroutine("RecoverFromStunned");
    }

    IEnumerator RecoverFromStunned() {
        float recoveryTime = 0;
        while (recoveryTime <= stunnedTime) {
            recoveryTime += Time.deltaTime;
            yield return null;
        }
        isStunned = false;
    }

    IEnumerator Record(Recordings recordings) {
        while (true) {
            recordings.position.Add(transform.position);
            recordings.rotation.Add(transform.rotation);
            recordings.scale.Add(transform.localScale);
            recordings.velocity.Add(rb.velocity);
            // if (isDownInteract) {
            //     recordings.isInteracting.Add(true);
            // } else {
            //     recordings.isInteracting.Add(false);
            // }
            yield return null;
        }
        
    }

    IEnumerator SymplexMan(Recordings recordings) {
        // recordings.isInteracting = recordings.CleanList(recordings.isInteracting);
        
        // transform.position = recordings.position[0];
        // transform.rotation = recordings.rotation[0];
        // transform.localScale = recordings.scale[0];
        // rb.velocity = recordings.velocity[0];
        // if (recordings.isInteracting[0]) {
        //     PlayerInteraction();
        // }

        // GameObject clone = Instantiate(clonePrefab, 
        //                                recordings.position[0], 
        //                                recordings.rotation[0]);

        // Rigidbody cloneRB = clone.GetComponent<Rigidbody>();
        // Transform cloneT = clone.GetComponent<Transform>();

        // for (int i = 0; i < recordings.velocity.Count; i++) {
        //     cloneT.position = recordings.position[i];
        //     cloneT.rotation = recordings.rotation[i];
        //     cloneT.localScale = recordings.scale[i];
        //     cloneRB.velocity = recordings.velocity[i];
        //     if (recordings.isInteracting[i]) {
        //         PlayerInteraction();
        //     }
        //     yield return null;
        // }
        // Vector3 deathPosition = cloneT.position;
        // Destroy(clone);
        // Destroy(Instantiate(deathEffect, deathPosition, Quaternion.identity), 2);
        yield return null;
    }

    

    class Recordings {
        public List<Vector3> position = new List<Vector3>();
        public List<Quaternion> rotation = new List<Quaternion>();
        public List<Vector3> scale = new List<Vector3>();
        public List<Vector3> velocity = new List<Vector3>();
        public List<bool> isInteracting = new List<bool>();

        public List<bool> CleanList(List<bool> dirtyList) {
            List<bool> list = dirtyList;
            bool isCleaning = false;
            for (int i = 0; i < list.Count; i++) {
                if (list[i] == false) {
                    isCleaning = false;
                } else if (list[i] == true && !isCleaning) {
                    isCleaning = true;
                } else {
                    list[i] = false;
                }
            }
            return list;
        }
    }
}
