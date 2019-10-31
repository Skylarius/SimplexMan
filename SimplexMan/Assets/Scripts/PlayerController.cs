using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : Controller {

    [Header("Camera")]
    public float cameraRotSpeed = 1;
    public Vector2 cameraRotationXRange = new Vector2(0, 15);

    public event System.Action StartRecording;
    public event System.Action StopRecording;

    bool recordDown;
    bool isRecordEnabled = true;
    bool isRecording = false;
    
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

        // Cloning Input
        if (recordDown){
            if (!isRecording && isRecordEnabled) {
                isRecording = true;
                initialPosition = transform.position;
                initialRotation = transform.rotation;
                StartRecording();
            } else if (isRecording) {
                isRecording = false;
                StopRecording();
                transform.position = initialPosition;
                transform.rotation = initialRotation;
            }
            
        }
    }

    public void EnableRecording() {
        isRecordEnabled = true;
    }

    public void DisableRecording() {
        isRecordEnabled = false;
    }
    

    protected override void GetInputs() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxisRaw("Mouse X");
        jump = Input.GetButtonDown("Jump");
        interactDown = Input.GetButtonDown("Interact");
        interactUp = Input.GetButtonUp("Interact");

        recordDown = Input.GetButtonDown("Record");
    }
}
