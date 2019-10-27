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

    public override void Start() {
        wallRotation = wall.localRotation.eulerAngles;
        base.Start();
    }

    public override void ChangeState(bool _state) {
        state = _state;
        foreach (Vent vent in vents) {
            vent.ChangeState(_state);
        }
    }

    public override void ChangeTransform(bool _state) {
        state = _state;
        if (state == true) {
            wallRotation.y += Time.deltaTime * speed;
        } else {
            wallRotation.y -= Time.deltaTime * speed;
        }
        wall.localRotation = Quaternion.Euler(wallRotation);
    }

    public override void StartRecording() {
        initialWallRotation = wallRotation;
        initialState = state;
        base.StartRecording();
    }

    public override void StopRecording() {
        wallRotation = initialWallRotation;
        wall.localRotation = Quaternion.Euler(wallRotation);
        ChangeState(initialState);
        base.StopRecording();
    }
}
