using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : InteractiveCollider {
    
    public MutableObject mutableObject;

    Transform wheel;
    Vector3 wheelRotation;

    float wheelSpeed = 10;
    bool isHolding = false;
    
    // Recorded initial state
    float initialWheelRotation;

    void Awake() {
        wheel = transform.Find("Wheel");
        wheelRotation = wheel.localRotation.eulerAngles;
    }

    public override void PlayerInteraction() {
        if (base.isEnabled) {
            isHolding = true;
            StopCoroutine("Release");
            StartCoroutine("Hold");
        }
    }

    public override void StopPlayerInteraction() {
        if (isHolding) {
            isHolding = false;
            StopCoroutine("Hold");
            StartCoroutine("Release");
        }
    }

    protected override void StartRecording() {
        initialWheelRotation = wheelRotation.z;
        base.StartRecording();
    }

    protected override void StopRecording() {
        wheelRotation.z = initialWheelRotation;
        wheel.localRotation = Quaternion.Euler(wheelRotation);
        base.StopRecording();
    }

    IEnumerator Hold() {
        while (mutableObject.ChangeState(true)) {
            wheelRotation.z += Time.deltaTime * wheelSpeed;
            wheel.localRotation = Quaternion.Euler(wheelRotation);
            yield return null;
        }
    }

    IEnumerator Release() {
        while (mutableObject.ChangeState(false)) {
            wheelRotation.z -= Time.deltaTime * wheelSpeed * 10;
            wheel.localRotation = Quaternion.Euler(wheelRotation);
            yield return null;
        }
        wheelRotation.z = 0;
        wheel.localRotation = Quaternion.Euler(wheelRotation);
    }
}
