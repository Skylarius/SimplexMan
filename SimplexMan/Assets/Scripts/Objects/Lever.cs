using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : InteractiveCollider {
    
    public ElectricWall wall;
    public float speed;
    public Transform lever;

    enum State {Down, Up};
    State state = State.Up;

    bool isHolding = false;
    float initialAngle = -50;

    // Recorded initial state
    State initialState;

    public override void Start() {
        base.Start();
        
        SetState(state);
    }

    void SetState(State s) {
        state = s;
        Vector3 rot = lever.rotation.eulerAngles;
        if (s == State.Up) {
            rot.z = initialAngle;
            wall.ChangeState(true);
        } else { // Down
            rot.z = -initialAngle;
            wall.ChangeState(false);
        }
        lever.rotation = Quaternion.Euler(rot);
    }

    protected override void PlayerInteraction() {
        if (base.isEnabled) {
            isHolding = !isHolding;
            StopCoroutine("Up");
            StartCoroutine("Down");
        }
    }

    protected override void StopPlayerInteraction() {
        if (isHolding) {
            isHolding = !isHolding;
            StopCoroutine("Down");
            StartCoroutine("Up");
        }
    }

    public override void StartRecording() {
        initialState = state;
        base.StartRecording();
    }

    public override void StopRecording() {
        if (state != initialState) {
            state = initialState;
            SetState(state);
        }
        base.StopRecording();
    }

    IEnumerator Down() {
        Vector3 currentRot = lever.rotation.eulerAngles;
        float startRotZ = currentRot.z;
        if (startRotZ > 180) {
            startRotZ -= 360;
        }
        float percentage = 0;
        while (percentage <= 1) {
            currentRot.z = Mathf.Lerp(startRotZ, -initialAngle, percentage);
            lever.rotation = Quaternion.Euler(currentRot);

            percentage += Time.deltaTime * speed;
            yield return null;
        }
        SetState(State.Down);
    }

    IEnumerator Up() {
        state = State.Up;
        wall.ChangeState(true);
        Vector3 currentRot = lever.rotation.eulerAngles;
        float startRotZ = currentRot.z;
        if (startRotZ > 180) {
            startRotZ -= 360;
        }
        float percentage = 0;
        while (percentage <= 1) {
            currentRot.z = Mathf.Lerp(startRotZ, initialAngle, percentage);
            lever.rotation = Quaternion.Euler(currentRot);

            percentage += Time.deltaTime * speed;
            yield return null;
        }
        currentRot.z = initialAngle;
        lever.rotation = Quaternion.Euler(currentRot);
    }
}
