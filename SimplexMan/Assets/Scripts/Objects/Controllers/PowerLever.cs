using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLever : InteractivePoweredCollider {
    
    public List<GameObject> gameObjectsToPower;
    List<IPower> objectsToPower = new List<IPower>();
    
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
        foreach (GameObject o in gameObjectsToPower) {
            objectsToPower.Add(o.GetComponent<IPower>());
        }
        lever = transform.Find("Lever");
        base.electricity.Add(transform.Find("Electricity").GetComponent<Renderer>());
    }

    protected override void Start() {
        SetState(state);
        base.Start();
    }

    void SetState(State s) {
        state = s;
        Vector3 rot = lever.localRotation.eulerAngles;
        if (s == State.Up) {
            rot.z = upRotation;
            SetPowers(false);
        } else { // Down
            rot.z = -upRotation;
            if (base.hasPower) {
                SetPowers(true);
            }
        }
        lever.localRotation = Quaternion.Euler(rot);
    }

    void SetPowers(bool hasPower) {
        foreach (IPower objectToPower in objectsToPower) {
            objectToPower.SetPower(hasPower);
        }
    }

    public override void PlayerInteraction() {
        if (base.isEnabled) {
            isHolding = true;
            StopCoroutine("Up");
            StartCoroutine("Down");
        }
    }

    public override void StopPlayerInteraction() {
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
            if (state == State.Up) {
                SetPowers(false);
            } else {
                SetPowers(true);
            }
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
        SetPowers(false);
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
