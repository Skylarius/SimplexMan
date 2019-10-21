using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    [Header("Movement")]
    public float speed = 15.0F;
    public float rotationSpeed = 5;
    public float jumpForce = 300;
    [Range(0, 1)]
    public float airFriction = 0;
    public float stunnedTime = 1;
    
    protected Vector3 velocity;
    protected float rotation;
    protected bool jumpInput;

    protected float horizontal;
    protected float vertical;
    protected float mouseX;
    protected bool jump;
    protected bool interactDown;
    protected bool interactUp;

    Rigidbody rb;
    Vector3 jumpStartVelocity;
    bool isStunned;

    protected virtual void Start() {
        rb = GetComponent<Rigidbody>();
    }

    // Input should be taken in Update
    protected virtual void Update() {
        GetInputs();

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
    }

    // Physics should be applied in FixedUpdate
    protected virtual void FixedUpdate() {
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

    protected virtual void GetInputs() {}
}
