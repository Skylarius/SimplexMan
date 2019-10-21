using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordableClone : Recordable {

    public GameObject deathEffect;

    List<RecordablePlayer.RecordedInput> inputs;
    int index = 0;
    float horizontal;
    float vertical;
    float mouseX;
    bool jump;
    bool interactDown;
    bool interactUp;

    float speed;
    float rotationSpeed;
    float jumpForce;
    float airFriction;
    float stunnedTime;

    Rigidbody rb;
    Vector3 velocity;
    float rotation;
    Vector3 jumpStartVelocity;
    bool jumpInput;
    bool isStunned;

    Vector3 initialPosition;
    Quaternion initialRotation;
    int goBackToIndex = -1;
    bool isAlive = true;

    void Awake() {
        PlayerController player = FindObjectOfType<PlayerController>();

        speed = player.speed;
        rotationSpeed = player.rotationSpeed;
        jumpForce = player.jumpForce;
        airFriction = player.airFriction;
        stunnedTime = player.stunnedTime;
    }

    public override void Start() {
        rb = GetComponent<Rigidbody>();
        base.Start();
    }

    public override void Update() {
        if (index < inputs.Count) {
            if (!isAlive) {
                isAlive = true;
                SetVisibility();
            }

            horizontal = inputs[index].horizontal;
            vertical = inputs[index].vertical;
            mouseX = inputs[index].mouseX;
            jump = inputs[index].jump;
            interactDown = inputs[index].interactDown;
            interactUp = inputs[index].interactUp;
        } else {
            if (goBackToIndex != -1) {
                if (isAlive) {
                    horizontal = 0;
                    vertical = 0;
                    mouseX = 0;
                    jump = false;
                    interactDown = false;
                    interactUp = false;

                    isAlive = false;
                    SetVisibility();
                    Destroy(Instantiate(deathEffect, transform.position, Quaternion.identity), 2);
                }
            } else {
                FindObjectOfType<PlayerController>().StartRecording -= StartRecording;
                FindObjectOfType<PlayerController>().StopRecording -= StopRecording;

                Destroy(Instantiate(deathEffect, transform.position, Quaternion.identity), 2);
                Destroy(gameObject);
            }
        }

        // Movement Input  
        Vector3 moveHorizontal = transform.right * horizontal;
        Vector3 moveVertical = transform.forward * vertical;
        Vector3 direction = (moveHorizontal + moveVertical).normalized; 
        velocity = direction * speed;

        // Rotation Input
        rotation = mouseX;

        // Jump Input
        if (jump) {
            jumpInput = true;
        }

        // Interaction Input
        if (interactDown) {
            //PlayerInteraction();
        }
        if (interactUp) {
            //StopPlayerInteraction();
        }

        index++;
    }

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

    void SetVisibility() {
        GetComponent<CapsuleCollider>().enabled = isAlive;
        GetComponent<MeshRenderer>().enabled = isAlive;
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

    public override void StartRecording() {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        goBackToIndex = index;
        base.StartRecording();
    }

    public override void StopRecording() {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        index = goBackToIndex;
        goBackToIndex = -1;
        base.StopRecording();
    }

    public void SetInputs(List<RecordablePlayer.RecordedInput> _inputs) {
        inputs = _inputs;
    }
}
