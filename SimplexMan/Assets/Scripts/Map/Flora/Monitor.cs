using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : InteractiveCollider {

    public enum State {On, Off};
    public State state;

    List<Renderer> electricity = new List<Renderer>();
    Renderer indicatorLight;
    GameObject noise;

    // Recorded initial state
    State initialState;

    void Awake() {
        foreach (Transform child in transform.Find("Cables")) {
            electricity.Add(child.GetComponent<Renderer>());
        }
        indicatorLight = transform.Find("Screen").Find("IndicatorLight").GetComponent<Renderer>();
        noise = transform.Find("Screen").Find("Noise").gameObject;
        SetState(state);
    }

    void SetState(State newState) {
        if (newState == State.On) {
            noise.SetActive(true);
            foreach (Renderer r in electricity) {
                r.material.EnableKeyword("_EMISSION");
            }
            indicatorLight.material.SetColor("_EmissionColor", Color.green);
        } else {
            noise.SetActive(false);
            foreach (Renderer r in electricity) {
                r.material.DisableKeyword("_EMISSION");
            }
            indicatorLight.material.SetColor("_EmissionColor", Color.red);
        }
        state = newState;
    }

    public override void PlayerInteraction() {
        if (state == State.On) {
            SetState(State.Off);
        } else {
            SetState(State.On);
        }
    }

    protected override void StartRecording() {
        initialState = state;
    }

    protected override void StopRecording() {
        SetState(initialState);
    }
}
