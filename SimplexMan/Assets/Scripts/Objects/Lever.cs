using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// True = UP
// False = DOWN
public class Lever : RecordableHold {
    
    public ElectricWall objectToChange;
    public float speed;
    public Transform lever;

    bool isEnabled = false;
    bool isHolding = false;
    bool initialState;
    float initialAngle = -50;

    public override void Start() {
        base.Start();
        
        initialState = base.isActive;
        Vector3 rot = lever.rotation.eulerAngles;
        rot.z = initialAngle;
        lever.rotation = Quaternion.Euler(rot);

        FindObjectOfType<PlayerController>().PlayerInteraction += PlayerInteraction;
        FindObjectOfType<PlayerController>().StopPlayerInteraction += StopPlayerInteraction;
    }

    public override void Update() {
        base.Update();
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player") {
            isEnabled = true;
        } else if (collider.tag == "Clone") {
            isEnabled = true;
            StartCoroutine("CheckClone", collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            isEnabled = false;
            StopPlayerInteraction();
        }
    }

    void PlayerInteraction() {
        if (isEnabled) {
            isHolding = !isHolding;
            initialState = isActive;
            StopCoroutine("Up");
            StartCoroutine("Down");
        }
    }

    void StopPlayerInteraction() {
        if (isHolding) {
            isHolding = !isHolding;
            StopCoroutine("Down");
            StartCoroutine("Up");
        }
    }

    protected override void ResetState(bool _isActive) {
        Vector3 currentRot = lever.rotation.eulerAngles;
        if (isActive) {
            currentRot.z = initialAngle;
        } else {
            currentRot.z = -initialAngle;
        }
        lever.rotation = Quaternion.Euler(currentRot);
        objectToChange.ChangeState(isActive);
    }

    IEnumerator Down() {
        Vector3 currentRot = lever.rotation.eulerAngles;
        float startRotZ = currentRot.z;
        if (startRotZ > 180) {
            startRotZ -= 360;
        }
        float percentage = 0;
        while (percentage <= 1) {
            currentRot.z = Mathf.Lerp(startRotZ, -initialAngle, percentage);
            lever.rotation = Quaternion.Euler(currentRot);

            percentage += Time.deltaTime * speed;
            yield return null;
        }
        currentRot.z = -initialAngle;
        lever.rotation = Quaternion.Euler(currentRot);
        isActive = !isActive;
        objectToChange.ChangeState(isActive);
    }

    IEnumerator Up() {
        if (isActive != initialState) {
            isActive = !isActive;
            objectToChange.ChangeState(isActive);
        }
        Vector3 currentRot = lever.rotation.eulerAngles;
        float startRotZ = currentRot.z;
        if (startRotZ > 180) {
            startRotZ -= 360;
        }
        float percentage = 0;
        while (percentage <= 1) {
            currentRot.z = Mathf.Lerp(startRotZ, initialAngle, percentage);
            lever.rotation = Quaternion.Euler(currentRot);

            percentage += Time.deltaTime * speed;
            yield return null;
        }
        currentRot.z = initialAngle;
        lever.rotation = Quaternion.Euler(currentRot);
    }

    IEnumerator CheckClone(GameObject clone) {
        while(true) {
            if (clone == null) {
                break;
            }
            yield return null;
        }
        // should check some stuff before doing this
        StartCoroutine("Up");
    }
}
