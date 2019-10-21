using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordableClone : Controller {

    public GameObject deathEffect;

    List<RecordablePlayer.RecordedInput> inputs;
    int index = 0;

    Vector3 initialPosition;
    Quaternion initialRotation;
    int goBackToIndex = -1;
    bool isAlive = true;

    protected override void Start() {
        base.Start();
        FindObjectOfType<PlayerController>().StartRecording += StartRecording;
        FindObjectOfType<PlayerController>().StopRecording += StopRecording;
    }

    protected override void Update() {
        base.Update();
        index++;
    }

    protected override void GetInputs() {
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
    }

    public override void Interact() {
        FindObjectOfType<PlayerController>().Interact();
    }

    public override void StopInteract() {
        FindObjectOfType<PlayerController>().StopInteract();
    }

    void SetVisibility() {
        GetComponent<CapsuleCollider>().enabled = isAlive;
        GetComponent<MeshRenderer>().enabled = isAlive;
    }

    void StartRecording() {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        goBackToIndex = index;
    }

    void StopRecording() {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        index = goBackToIndex;
        goBackToIndex = -1;
    }

    public void SetInputs(List<RecordablePlayer.RecordedInput> _inputs) {
        inputs = _inputs;
    }
}
