using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : InteractiveCollider {
    
    public float wheelSpeed;
    public float wallSpeed;
    public Transform wheel;
    public Transform leftWall;
    public Transform rightWall;

    bool isHolding = false;
    float wallDistance = 14;

    Vector3 wheelRotation;
    
    // Recorded initial state
    float initialWheelRotation;
    float initialLeftPosition;
    float initialRightPosition;

    public override void Start() {
        base.Start();
        wheelRotation = wheel.localRotation.eulerAngles;
    }

    protected override void PlayerInteraction() {
        if (base.isEnabled) {
            isHolding = true;
            StopCoroutine("Release");
            StartCoroutine("Hold");
        }
    }

    protected override void StopPlayerInteraction() {
        if (isHolding) {
            isHolding = false;
            StopCoroutine("Hold");
            StartCoroutine("Release");
        }
    }

    public override void StartRecording() {
        initialWheelRotation = wheelRotation.x;
        initialLeftPosition = leftWall.localPosition.x;
        initialRightPosition = rightWall.localPosition.x;
        base.StartRecording();
    }

    public override void StopRecording() {
        wheelRotation.x = initialWheelRotation;
        wheel.localRotation = Quaternion.Euler(wheelRotation);
        leftWall.localPosition = new Vector3(initialLeftPosition, 0, 0);
        rightWall.localPosition = new Vector3(initialRightPosition, 0, 0);
        base.StopRecording();
    }

    IEnumerator Hold() {
        while (rightWall.localPosition.x <= wallDistance) {
            leftWall.localPosition -= new Vector3(Time.deltaTime * wallSpeed, 0, 0);
            rightWall.localPosition += new Vector3(Time.deltaTime * wallSpeed, 0, 0);
            wheelRotation.x += Time.deltaTime * wheelSpeed;
            wheel.localRotation = Quaternion.Euler(wheelRotation);
            yield return null;
        }
    }

    IEnumerator Release() {
        while (rightWall.localPosition.x > 0) {
            leftWall.localPosition += new Vector3(Time.deltaTime * wallSpeed * 10, 0, 0);
            rightWall.localPosition -= new Vector3(Time.deltaTime * wallSpeed * 10, 0, 0);
            wheelRotation.x -= Time.deltaTime * wheelSpeed * 10;
            wheel.localRotation = Quaternion.Euler(wheelRotation);
            yield return null;
        }
        leftWall.localPosition = new Vector3(0, 0, 0);
        rightWall.localPosition = new Vector3(0, 0, 0);
        wheelRotation.x = 0;
        wheel.localRotation = Quaternion.Euler(wheelRotation);
    }
}
