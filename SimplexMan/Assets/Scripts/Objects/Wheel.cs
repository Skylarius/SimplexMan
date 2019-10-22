using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : Recordable {
    
    public float wheelSpeed;
    public float wallSpeed;
    public Transform wheel;
    public Transform leftWall;
    public Transform rightWall;

    bool isEnabled = false;
    bool isHolding = false;
    float wallDistance = 14;

    Vector3 wheelRotation;
    int nCollidingObjects;

    // Recorded initial state
    float initialWheelRotation;
    float initialLeftPosition;
    float initialRightPosition;

    public override void Start() {
        base.Start();

        wheelRotation = wheel.localRotation.eulerAngles;

        FindObjectOfType<PlayerController>().PlayerInteraction += PlayerInteraction;
        FindObjectOfType<PlayerController>().StopPlayerInteraction += StopPlayerInteraction;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            isEnabled = true;
            nCollidingObjects++;
            if (collider.tag == "Clone") {
                StartCoroutine("OnTriggerExitClone", collider);
            }
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            nCollidingObjects--;
            if (collider.tag == "Clone") {
                StopCoroutine("OnTriggerExitClone");
            }

            if (nCollidingObjects == 0) {
                isEnabled = false;
                StopPlayerInteraction();
            }
        }
    }

    void PlayerInteraction() {
        if (isEnabled) {
            isHolding = true;
            StopCoroutine("Up");
            StartCoroutine("Down");
        }
    }

    void StopPlayerInteraction() {
        if (isHolding) {
            isHolding = false;
            StopCoroutine("Down");
            StartCoroutine("Up");
        }
    }

    public override void StartRecording() {
        initialWheelRotation = wheelRotation.x;
        initialLeftPosition = leftWall.localPosition.z;
        initialRightPosition = rightWall.localPosition.z;
        base.StartRecording();
    }

    public override void StopRecording() {
        wheelRotation.x = initialWheelRotation;
        wheel.localRotation = Quaternion.Euler(wheelRotation);
        leftWall.localPosition = new Vector3(0, 0, initialLeftPosition);
        rightWall.localPosition = new Vector3(0, 0, initialRightPosition);
        base.StopRecording();
    }

    IEnumerator Down() {
        while (leftWall.localPosition.z <= wallDistance) {
            leftWall.localPosition += new Vector3(0, 0, Time.deltaTime * wallSpeed);
            rightWall.localPosition -= new Vector3(0, 0, Time.deltaTime * wallSpeed);
            wheelRotation.x += Time.deltaTime * wheelSpeed;
            wheel.localRotation = Quaternion.Euler(wheelRotation);
            yield return null;
        }
    }

    IEnumerator Up() {
        while (leftWall.localPosition.z > 0) {
            leftWall.localPosition -= new Vector3(0, 0, Time.deltaTime * wallSpeed * 10);
            rightWall.localPosition += new Vector3(0, 0, Time.deltaTime * wallSpeed * 10);
            wheelRotation.x -= Time.deltaTime * wheelSpeed * 10;
            wheel.localRotation = Quaternion.Euler(wheelRotation);
            yield return null;
        }
        leftWall.localPosition = new Vector3(0, 0, 0);
        rightWall.localPosition = new Vector3(0, 0, 0);
        wheelRotation.x = 0;
        wheel.localRotation = Quaternion.Euler(wheelRotation);
    }

    IEnumerator OnTriggerExitClone(Collider clone) {
        while(true) {
            if (clone == null || !clone.enabled) {
                nCollidingObjects--;
                if (nCollidingObjects == 0) {
                    isEnabled = false;
                    StopPlayerInteraction();
                }
                break;
            }
            yield return null;
        }
    }
}
