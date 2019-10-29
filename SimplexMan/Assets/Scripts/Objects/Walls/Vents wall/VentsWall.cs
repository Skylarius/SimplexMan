using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentsWall : MutableObject {

    public Transform wall;
    public List<Vent> vents;

    Vector3 wallRotation;
    bool state;
    float speed = 10;

    // Recorded initial state
    Vector3 initialWallRotation;
    bool initialState;

    protected override void Start() {
        wallRotation = wall.localRotation.eulerAngles;
        base.Start();
    }

    public override bool ChangeState(bool _state) {
        state = _state;
        foreach (Vent vent in vents) {
            vent.ChangeState(_state);
        }
        return true;
    }

    protected override void StartRecording() {
        initialWallRotation = wallRotation;
        initialState = state;
        base.StartRecording();
    }

    protected override void StopRecording() {
        wallRotation = initialWallRotation;
        wall.localRotation = Quaternion.Euler(wallRotation);
        ChangeState(initialState);
        base.StopRecording();
    }
}
