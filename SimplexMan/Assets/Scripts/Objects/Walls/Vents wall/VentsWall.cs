using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentsWall : MutableObject {

    List<Vent> vents = new List<Vent>();

    bool state;

    // Recorded initial state
    bool initialState;

    void Awake() {
        foreach (Transform vent in transform.Find("Vents")) {
            vents.Add(vent.GetComponent<Vent>());
        }
    }

    public override bool ChangeState(bool _state) {
        state = _state;
        foreach (Vent vent in vents) {
            vent.ChangeState(_state);
        }
        return true;
    }

    protected override void StartRecording() {
        initialState = state;
        base.StartRecording();
    }

    protected override void StopRecording() {
        ChangeState(initialState);
        base.StopRecording();
    }
}
