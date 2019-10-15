using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    [Header("Input")]
	public string Horizontal = "Horizontal";
	public string Vertical = "Vertical";
	public string mouseX = "Mouse X";
    public string mouseY = "Mouse Y";
    [Header("Movement")]
    public float speed = 20.0F;
    public float rotationSpeed = 2;
    public float jumpForce = 200;
    [Header("Camera")]
    public float cameraRotSpeed = 1;
    public Vector2 cameraRotationXRange = new Vector2(10, 35);
    [Header("Symplex Man")]
    public GameObject clonePrefab;
    public ParticleSystem deathEffect;

    public event System.Action PlayerInteraction;

    bool isDownInteract = false;
    bool isRecording = false;
    Recordings recordings;
    
    float currentCameraRotX = 0f;
    Camera myCamera;
    Rigidbody rb;
    

    void Start(){
        rb = GetComponent<Rigidbody>();
        myCamera = GetComponentInChildren<Camera>();
	}

	void FixedUpdate() {

        if (IsGrounded()) {
            // Jump Input
            if (Input.GetButtonDown("Jump")) {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            // Rotation Input
            float yRot = Input.GetAxisRaw(mouseX);              
            Vector3 rotation = new Vector3(0f, yRot, 0f) * rotationSpeed;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        }

        // Movement Input
        float xMov = 0;
        if (IsGrounded()) {
            xMov = Input.GetAxisRaw(Horizontal);
        }    
        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * Input.GetAxisRaw(Vertical);
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

        // Camera Input
        float xRot = Input.GetAxisRaw(mouseY);
        currentCameraRotX -= xRot * cameraRotSpeed;
        currentCameraRotX = Mathf.Clamp(currentCameraRotX, cameraRotationXRange.x, cameraRotationXRange.y);
        myCamera.transform.localEulerAngles = new Vector3(currentCameraRotX, 0f, 0f);

        // Interaction Input
        if (Input.GetButtonDown("Interact") && !isDownInteract) {
            isDownInteract = true;
            PlayerInteraction();
        }
        if (Input.GetButtonUp("Interact")) {
            isDownInteract = false;
        }

        // Cloning Input
        if (Input.GetButtonDown("Clone") && !isRecording) {
            isRecording = true;
            recordings = new Recordings();
            StartCoroutine("Record", recordings);
        }
        if (Input.GetButtonUp("Clone") && isRecording) {
            isRecording = false;
            StopCoroutine("Record");
            StartCoroutine("SymplexMan", recordings);
        }
    }

    bool IsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, transform.localScale.y + 0.5f);
    }

    IEnumerator Record(Recordings recordings) {
        int index = 0;
        while (true) {
            recordings.position.Add(transform.position);
            recordings.rotation.Add(transform.rotation);
            recordings.scale.Add(transform.localScale);
            recordings.velocity.Add(rb.velocity);
            if (isDownInteract) {
                recordings.isInteracting.Add(true);
            } else {
                recordings.isInteracting.Add(false);
            }
            index++;
            yield return null;
        }
        
    }

    IEnumerator SymplexMan(Recordings recordings) {
        recordings.isInteracting = recordings.CleanList(recordings.isInteracting);
        
        transform.position = recordings.position[0];
        transform.rotation = recordings.rotation[0];
        transform.localScale = recordings.scale[0];
        rb.velocity = recordings.velocity[0];
        if (recordings.isInteracting[0]) {
            PlayerInteraction();
        }

        GameObject clone = Instantiate(clonePrefab, 
                                       recordings.position[0], 
                                       recordings.rotation[0]);

        Rigidbody cloneRB = clone.GetComponent<Rigidbody>();
        Transform cloneT = clone.GetComponent<Transform>();

        for (int i = 0; i < recordings.velocity.Count; i++) {
            cloneT.position = recordings.position[i];
            cloneT.rotation = recordings.rotation[i];
            cloneT.localScale = recordings.scale[i];
            cloneRB.velocity = recordings.velocity[i];
            if (recordings.isInteracting[i]) {
                PlayerInteraction();
            }
            yield return null;
        }
        Vector3 deathPosition = cloneT.position;
        Destroy(clone);
        Destroy(Instantiate(deathEffect.gameObject, deathPosition, Quaternion.identity), 2);
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
