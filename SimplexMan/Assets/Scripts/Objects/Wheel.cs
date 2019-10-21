using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : RecordableHold {
    
    public float speed;
    public float wallSpeed;
    public Transform wheel;
    public Transform leftWall;
    public Transform rightWall;

    bool isEnabled = false;
    bool isHolding = false;
    float wallDistance = 14;

    public override void Start() {
        base.Start();

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
            //initialState = isActive;
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
        Vector3 currentRot = wheel.rotation.eulerAngles;
        // if (isActive) {
        //     currentRot.z = initialAngle;
        // } else {
        //     currentRot.z = -initialAngle;
        // }
        wheel.rotation = Quaternion.Euler(currentRot);
    }

    IEnumerator Down() {
        Vector3 currentRot = wheel.rotation.eulerAngles;
        while (leftWall.localPosition.z <= wallDistance) {
            leftWall.localPosition += new Vector3(0, 0, Time.deltaTime * wallSpeed);
            rightWall.localPosition -= new Vector3(0, 0, Time.deltaTime * wallSpeed);
            currentRot.x += Time.deltaTime * speed;
            wheel.rotation = Quaternion.Euler(currentRot);
            yield return null;
        }
    }

    IEnumerator Up() {
        Vector3 currentRot = wheel.rotation.eulerAngles;
        while (leftWall.localPosition.z > 0) {
            leftWall.localPosition -= new Vector3(0, 0, Time.deltaTime * wallSpeed * 10);
            rightWall.localPosition += new Vector3(0, 0, Time.deltaTime * wallSpeed * 10);
            currentRot.x -= Time.deltaTime * speed * 10;
            wheel.rotation = Quaternion.Euler(currentRot);
            yield return null;
        }
        leftWall.localPosition = new Vector3(0, 0, 0);
        rightWall.localPosition = new Vector3(0, 0, 0);
        currentRot.x = 0;
        wheel.rotation = Quaternion.Euler(currentRot);
    }

    IEnumerator CheckClone(GameObject clone) {
        while(true) {
            if (clone == null) {
                break;
            }
            yield return null;
        }
        // Should check some stuff before doing this
        StartCoroutine("Up");
    }
}
