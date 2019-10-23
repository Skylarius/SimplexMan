using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlatform : MutableObject {
    
    public Transform arrivalStation;
    public GameObject barrier;
    public GameObject teleportEffect;
    public Transform band1;
    public Transform band2;
    public Transform band3;
    public Transform top;
    public float teleportTime = 4;
    public float recoveryTime = 2;
    public float bandSeparationSpeed = 0.5f;
    public float bandsAcceleration = 1.2f;

    int activatedControllers = 1;
    List<GameObject> objectsToTeleport = new List<GameObject>();

    bool isTeleporting = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ChangeState(bool state) {
        if (!state) {
            activatedControllers++;
        } else if (activatedControllers > 0) {
            activatedControllers--;
        }

        if (!isTeleporting) {
            if (activatedControllers == 1) {
                StartCoroutine("TeleportAnimation");
                StartCoroutine("TeleportObjects");
            }
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (!isTeleporting) {
            if (collider.tag == "Player" || collider.tag == "Clone") {
                objectsToTeleport.Add(collider.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider collider) {
        if (!isTeleporting) {
            if (collider.tag == "Player" || collider.tag == "Clone") {
                objectsToTeleport.Remove(collider.gameObject);
            }
        }
    }

    IEnumerator TeleportObjects() {
        foreach (GameObject objectToTeleport in objectsToTeleport) {
            if (objectToTeleport != null) {
                objectToTeleport.GetComponent<Controller>().Stun(10);
            }
        }
        float passedTime = 0;
        while (passedTime < teleportTime) {
            foreach (GameObject objectToTeleport in objectsToTeleport) {
                if (objectToTeleport != null) {
                    objectToTeleport.GetComponent<Rigidbody>().velocity = new Vector3(0, Random.Range(0f, 6f), 0);
                }
            }
            passedTime += Time.deltaTime;
            yield return null;
        }
        foreach (GameObject objectToTeleport in objectsToTeleport) {
            if (objectToTeleport != null) {
                objectToTeleport.transform.position = arrivalStation.position;
            }
        }
        foreach (GameObject objectToTeleport in objectsToTeleport) {
            if (objectToTeleport != null) {
                objectToTeleport.GetComponent<Controller>().Stun(0);
            }
        }
        objectsToTeleport.Clear();
    }

    IEnumerator TeleportAnimation() {
        isTeleporting = true;
        barrier.SetActive(true);
        barrier.GetComponent<TeleportBarrier>().objectsInside = objectsToTeleport;
        teleportEffect.SetActive(true);
        StartCoroutine("SeparateBands");
        StartCoroutine("RotateBands");
        yield return new WaitForSeconds(teleportTime);
        StartCoroutine("MergeBands");
        yield return new WaitForSeconds(recoveryTime);
        barrier.GetComponent<TeleportBarrier>().objectsInside.Clear();
        barrier.SetActive(false);
        teleportEffect.SetActive(false);
        isTeleporting = false;
    }

    IEnumerator SeparateBands() {
        float percentage = 0;
        while (percentage < 1) {
            band1.localPosition = new Vector3(0, Mathf.Lerp(0, 0.5f, percentage), 0);
            band2.localPosition = new Vector3(0, Mathf.Lerp(0, 1, percentage), 0);
            band3.localPosition = new Vector3(0, Mathf.Lerp(0, 1.5f, percentage), 0);
            top.localPosition = new Vector3(0, Mathf.Lerp(0, 2, percentage), 0);

            percentage += Time.deltaTime * bandSeparationSpeed;
            yield return null;
        }
    }

    IEnumerator MergeBands() {
        float timePassed = 0;
        while (timePassed < recoveryTime) {
            float percentage = Mathf.InverseLerp(0, recoveryTime, timePassed);
            band1.localPosition = new Vector3(0, Mathf.Lerp(0.5f, 0, percentage), 0);
            band2.localPosition = new Vector3(0, Mathf.Lerp(1, 0, percentage), 0);
            band3.localPosition = new Vector3(0, Mathf.Lerp(1.5f, 0, percentage), 0);
            top.localPosition = new Vector3(0, Mathf.Lerp(2, 0, percentage), 0);

            timePassed += Time.deltaTime;
            yield return null;
        }
        band1.localPosition = new Vector3(0, 0, 0);
        band2.localPosition = new Vector3(0, 0, 0);
        band3.localPosition = new Vector3(0, 0, 0);
        top.localPosition = new Vector3(0, 0, 0);
    }

    IEnumerator RotateBands() {

        // Acceleration
        Vector3 band1Rotation = band1.localRotation.eulerAngles;
        Vector3 band2Rotation = band2.localRotation.eulerAngles;
        Vector3 band3Rotation = band3.localRotation.eulerAngles;

        float timePassed = 0;
        float speed = 10;
        while (timePassed < teleportTime) {
            band1Rotation.y += Time.deltaTime * speed * 0.8f;
            band2Rotation.y -= Time.deltaTime * speed * 0.9f;
            band3Rotation.y += Time.deltaTime * speed;

            band1.localRotation = Quaternion.Euler(band1Rotation);
            band2.localRotation = Quaternion.Euler(band2Rotation);
            band3.localRotation = Quaternion.Euler(band3Rotation);

            speed *= bandsAcceleration;
            timePassed += Time.deltaTime;
            yield return null;
        }

        // Braking
        int finalLoops = 2;

        band1Rotation = band1.localRotation.eulerAngles;
        band2Rotation = band2.localRotation.eulerAngles;
        band3Rotation = band3.localRotation.eulerAngles;
        while (band1Rotation.y < 360*finalLoops) {
            band1Rotation.y += 360;
        }
        while (band2Rotation.y > -360*finalLoops) {
            band2Rotation.y -= 360;
        }
        while (band3Rotation.y < 360*finalLoops) {
            band3Rotation.y += 360;
        }

        timePassed = 0;
        while (timePassed < recoveryTime) {
            float percentage = Mathf.InverseLerp(0, recoveryTime, timePassed);
            band1Rotation = new Vector3(0, Mathf.Lerp(band1Rotation.y, 0, percentage), 0);
            band2Rotation = new Vector3(0, Mathf.Lerp(band2Rotation.y, 0, percentage), 0);
            band3Rotation = new Vector3(0, Mathf.Lerp(band3Rotation.y, 0, percentage), 0);

            band1.localRotation = Quaternion.Euler(band1Rotation);
            band2.localRotation = Quaternion.Euler(band2Rotation);
            band3.localRotation = Quaternion.Euler(band3Rotation);

            timePassed += Time.deltaTime;
            yield return null;
        }
        band1.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        band2.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        band3.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
}
