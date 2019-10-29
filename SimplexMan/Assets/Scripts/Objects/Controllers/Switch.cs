using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : InteractiveCollider {
    
    public enum State {On, Off};
    public State state;
    public MutableObject mutableObject;
    
    float speed = 5;
    float onPositionZ = 0.3f;
    Transform switchLever;
    Material green;
    Material red;

    // Recorded initial state
    State initialState;

    void Awake() {
        switchLever = transform.Find("Switch");
        green = transform.Find("Green light").GetComponent<Renderer>().material;
        red = transform.Find("Red light").GetComponent<Renderer>().material;
        SetState(state);
    }

    void SetState(State s) {
        state = s;
        Vector3 pos = switchLever.localPosition;
        if (s == State.On) {
            pos.z = onPositionZ;
            mutableObject.ChangeState(true);
            green.EnableKeyword("_EMISSION");
            red.DisableKeyword("_EMISSION");
        } else { // Off
            pos.z = -onPositionZ;
            mutableObject.ChangeState(false);
            red.EnableKeyword("_EMISSION");
            green.DisableKeyword("_EMISSION");
        }
        switchLever.localPosition = pos;
    }

    protected override void PlayerInteraction() {
        if (base.isEnabled) {
            if (state == State.On) {
                StopCoroutine("On");
                StartCoroutine("Off");
            } else {
                StopCoroutine("Off");
                StartCoroutine("On");
            }       
        }
    }

    protected override void StartRecording() {
        initialState = state;
        base.StartRecording();
    }

    protected override void StopRecording() {
        if (state != initialState) {
            state = initialState;
            SetState(state);
        }
        base.StopRecording();
    }

    IEnumerator On() {
        while (switchLever.localPosition.z < onPositionZ) {
            switchLever.localPosition += new Vector3(0, 0, Time.deltaTime * speed);
            yield return null;
        }
        SetState(State.On);
    }

    IEnumerator Off() {
        while (switchLever.localPosition.z > -onPositionZ) {
            switchLever.localPosition -= new Vector3(0, 0, Time.deltaTime * speed);
            yield return null;
        }
        SetState(State.Off);
    }
}
