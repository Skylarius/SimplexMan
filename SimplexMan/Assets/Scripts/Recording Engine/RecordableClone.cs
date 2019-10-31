using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordableClone : Controller {

    public GameObject deathEffect;

    List<RecordablePlayer.RecordedInput> inputs;
    int frameIndex = 0;

    int goBackToIndex = -1;
    bool isAlive = true;

    // Recorded initial state
    Vector3 initialPosition;
    Quaternion initialRotation;
    

    protected override void Start() {
        base.Start();
        FindObjectOfType<PlayerController>().StartRecording += StartRecording;
        FindObjectOfType<PlayerController>().StopRecording += StopRecording;
    }

    protected override void Update() {
        base.Update();
        frameIndex++;
    }

    protected override void GetInputs() {
        if (frameIndex < inputs.Count) {
            if (!isAlive) {
                isAlive = true;
                SetVisibility();
            }
            horizontal = inputs[frameIndex].horizontal;
            vertical = inputs[frameIndex].vertical;
            mouseX = inputs[frameIndex].mouseX;
            jump = inputs[frameIndex].jump;
            interactDown = inputs[frameIndex].interactDown;
            // Edit the last frame so that, if the clone is interacting, the
            // interaction ends after its death.
            if (frameIndex < inputs.Count - 1) {
                interactUp = inputs[frameIndex].interactUp;
            } else {
                interactUp = true;
            }
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
    }

    void SetVisibility() {
        GetComponent<CapsuleCollider>().enabled = isAlive;
        GetComponent<MeshRenderer>().enabled = isAlive;
    }

    void StartRecording() {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        goBackToIndex = frameIndex;
    }

    void StopRecording() {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        frameIndex = goBackToIndex;
        goBackToIndex = -1;
    }

    public void SetInputs(List<RecordablePlayer.RecordedInput> _inputs) {
        inputs = _inputs;
    }
}
