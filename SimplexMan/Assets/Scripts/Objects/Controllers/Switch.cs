using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : InteractiveCollider {
    
    public enum State {On, Off};
    public State state;
    public MutableObject mutableObject;
    public Transform swicthLever;
    public Renderer greenLight;
    public Renderer redLight;
    public float speed;

    float onPositionZ = 0.3f;
    Material green;
    Material red;

    // Recorded initial state
    State initialState;

    public override void Start() {
        base.Start();

        green = greenLight.material;
        red = redLight.material;
        
        SetState(state);
    }

    void SetState(State s) {
        state = s;
        Vector3 pos = swicthLever.localPosition;
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
        swicthLever.localPosition = pos;
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

    IEnumerator On() {
        while (swicthLever.localPosition.z < onPositionZ) {
            swicthLever.localPosition += new Vector3(0, 0, Time.deltaTime * speed);
            yield return null;
        }
        SetState(State.On);
    }

    IEnumerator Off() {
        while (swicthLever.localPosition.z > -onPositionZ) {
            swicthLever.localPosition -= new Vector3(0, 0, Time.deltaTime * speed);
            yield return null;
        }
        SetState(State.Off);
    }
}
