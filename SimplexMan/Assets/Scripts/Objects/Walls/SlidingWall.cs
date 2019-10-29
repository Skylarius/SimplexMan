using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingWall : MutableObject {
    
    Transform leftWall;
    Transform rightWall;

    float wallSpeed = 1;
    float maxWallDistance = 14;

    // Recorded initial state
    float initialLeftPosition;
    float initialRightPosition;

    void Awake() {
        leftWall = transform.Find("Left");
        rightWall = transform.Find("Right");
    }

    public override bool ChangeState(bool state) {
        if (state == true) {
            if (rightWall.localPosition.x <= maxWallDistance) {
                leftWall.localPosition -= new Vector3(Time.deltaTime * wallSpeed, 0, 0);
                rightWall.localPosition += new Vector3(Time.deltaTime * wallSpeed, 0, 0);
            } else {
                return false;
            }
        } else {
            if (rightWall.localPosition.x > 0) {
                leftWall.localPosition += new Vector3(Time.deltaTime * wallSpeed * 10, 0, 0);
                rightWall.localPosition -= new Vector3(Time.deltaTime * wallSpeed * 10, 0, 0);
            } else {
                leftWall.localPosition = new Vector3(0, 0, 0);
                rightWall.localPosition = new Vector3(0, 0, 0);
                return false;
            }
        }
        return true;
    }

    protected override void StartRecording() {
        initialLeftPosition = leftWall.localPosition.x;
        initialRightPosition = rightWall.localPosition.x;
        base.StartRecording();
    }

    protected override void StopRecording() {
        leftWall.localPosition = new Vector3(initialLeftPosition, 0, 0);
        rightWall.localPosition = new Vector3(initialRightPosition, 0, 0);
        base.StopRecording();
    }
}
