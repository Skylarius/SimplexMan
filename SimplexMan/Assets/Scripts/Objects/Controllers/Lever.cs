using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : InteractiveCollider {
    
    public MutableObject mutableObject;
    
    float speed = 100;
    Transform lever;

    enum State {Down, Up};
    State state = State.Up;

    bool isHolding = false;
    float upRotation = -50;

    // Recorded initial state
    Quaternion initialRotation;
    State initialState;

    void Awake() {
        lever = transform.Find("Lever");
        SetState(state);
    }

    void SetState(State s) {
        state = s;
        Vector3 rot = lever.localRotation.eulerAngles;
        if (s == State.Up) {
            rot.z = upRotation;
            mutableObject.ChangeState(true);
        } else { // Down
            rot.z = -upRotation;
            mutableObject.ChangeState(false);
        }
        lever.localRotation = Quaternion.Euler(rot);
    }

    protected override void PlayerInteraction() {
        if (base.isEnabled) {
            isHolding = true;
            StopCoroutine("Up");
            StartCoroutine("Down");
        }
    }

    protected override void StopPlayerInteraction() {
        if (isHolding) {
            isHolding = false;
            StopCoroutine("Down");
            StartCoroutine("Up");
        }
    }

    protected override void StartRecording() {
        initialState = state;
        initialRotation = lever.localRotation;
        base.StartRecording();
    }

    protected override void StopRecording() {
        if (state != initialState) {
            state = initialState;
            mutableObject.ChangeState(true);
        }
        lever.localRotation = initialRotation;
        base.StopRecording();
    }

    IEnumerator Down() {
        Vector3 currentRot = lever.localRotation.eulerAngles;
        if (currentRot.z > 180) {
            currentRot.z -= 360;
        }
        while (currentRot.z < -upRotation) {
            currentRot.z += Time.deltaTime * speed;
            lever.localRotation = Quaternion.Euler(currentRot);

            yield return null;
        }
        SetState(State.Down);
    }

    IEnumerator Up() {
        state = State.Up;
        mutableObject.ChangeState(true);
        Vector3 currentRot = lever.localRotation.eulerAngles;
        if (currentRot.z > 180) {
            currentRot.z -= 360;
        }
        while (currentRot.z > upRotation) {
            currentRot.z -= Time.deltaTime * speed;
            lever.localRotation = Quaternion.Euler(currentRot);

            yield return null;
        }
        currentRot.z = upRotation;
        lever.localRotation = Quaternion.Euler(currentRot);
    }
}
