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

    // Animation
    float bandSeparationSpeed = 0.5f;
    float bandsSpeed = 10;
    float bandsAcceleration = 1.2f;
    float maxBandsSpeed = 100000000f;
    int finalLoops = 0;

    // Teleporting
    int activatedControllers = 0;
    List<GameObject> objectsToTeleport = new List<GameObject>();

    // Internal
    bool isIdle = true;
    bool isRecovering = false;
    PlayerController player;

    // Recorded initial state
    Vector3 initialBand1Position;
    Quaternion initialBand1Rotation;
    Vector3 initialBand2Position;
    Quaternion initialBand2Rotation;
    Vector3 initialBand3Position;
    Quaternion initialBand3Rotation;
    Vector3 initialTopPosition;
    float initialBandsSpeed;
    int initialFinalLoops;
    bool initialBarrierState;
    bool initialEffectState;
    bool initialIsIdle;
    bool initialIsRecovering;

    public override void Start() {
        player = FindObjectOfType<PlayerController>();
        base.Start();
    }

    public override void ChangeState(bool state) {
        if (!state) {
            activatedControllers++;
            if (activatedControllers == 1 && isIdle) {
                StartTeleport();
            }
        } else if (activatedControllers > 0) {
            activatedControllers--;
            if (!isRecovering && !isIdle) {
                StopTeleport();
            }
        }
    }

    void StartTeleport() {
        isRecovering = false;
        isIdle = false;

        barrier.SetActive(true);
        barrier.GetComponent<TeleportBarrier>().objectsToTeleport = objectsToTeleport;
        
        StartCoroutine("SeparateBands");
    }

    void StopTeleport() {
        teleportEffect.SetActive(false);
        StopCoroutine("SeparateBands");
        StartCoroutine("MergeBands");
    }

    public override void StartRecording() {
        initialBand1Position = band1.localPosition;
        initialBand1Rotation = band1.localRotation;
        initialBand2Position = band2.localPosition;
        initialBand2Rotation = band2.localRotation;
        initialBand3Position = band3.localPosition;
        initialBand3Rotation = band3.localRotation;
        initialTopPosition = top.localPosition;
        initialBarrierState = barrier.activeSelf;
        initialBandsSpeed = bandsSpeed;
        initialFinalLoops = finalLoops;
        initialEffectState = teleportEffect.activeSelf;
        initialIsIdle = isIdle;
        initialIsRecovering = isRecovering;
        base.StartRecording();
    }

    public override void StopRecording() {
        band1.localPosition = initialBand1Position;
        band1.localRotation = initialBand1Rotation;
        band2.localPosition = initialBand2Position;
        band2.localRotation = initialBand2Rotation;
        band3.localPosition = initialBand3Position;
        band3.localRotation = initialBand3Rotation;
        top.localPosition = initialTopPosition;
        bandsSpeed = initialBandsSpeed;
        finalLoops = initialFinalLoops;
        barrier.SetActive(initialBarrierState);
        teleportEffect.SetActive(initialEffectState);
        isIdle = initialIsIdle;
        isRecovering = initialIsRecovering;
        base.StopRecording();
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            objectsToTeleport.Add(collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            objectsToTeleport.Remove(collider.gameObject);
        }
    }

    IEnumerator SeparateBands() {
        player.DisableRecording();

        foreach (GameObject objectToTeleport in objectsToTeleport) {
            if (objectToTeleport != null) {
                objectToTeleport.GetComponent<Controller>().Stun(10);
            }
        }

        teleportEffect.SetActive(true);

        Vector3 band1Rotation = band1.localRotation.eulerAngles;
        Vector3 band2Rotation = band2.localRotation.eulerAngles;
        Vector3 band3Rotation = band3.localRotation.eulerAngles;

        while (top.localPosition.y <= 2) {
            foreach (GameObject objectToTeleport in objectsToTeleport) {
                if (objectToTeleport != null) {
                    objectToTeleport.GetComponent<Rigidbody>().velocity = new Vector3(0, Random.Range(0f, 6f), 0);
                }
            }

            band1.localPosition += new Vector3(0, Time.deltaTime * bandSeparationSpeed * 0.5f, 0);
            band2.localPosition += new Vector3(0, Time.deltaTime * bandSeparationSpeed * 1, 0);
            band3.localPosition += new Vector3(0, Time.deltaTime * bandSeparationSpeed * 1.5f, 0);
            top.localPosition += new Vector3(0, Time.deltaTime * bandSeparationSpeed * 2, 0);

            if (band3.localPosition.y > finalLoops * 0.5) {
                finalLoops++;
            }

            band1Rotation.y += Time.deltaTime * bandsSpeed * 0.8f;
            band2Rotation.y -= Time.deltaTime * bandsSpeed * 0.9f;
            band3Rotation.y += Time.deltaTime * bandsSpeed;

            band1.localRotation = Quaternion.Euler(band1Rotation);
            band2.localRotation = Quaternion.Euler(band2Rotation);
            band3.localRotation = Quaternion.Euler(band3Rotation);

            bandsSpeed *= bandsAcceleration;
            yield return null;
        }

        while (bandsSpeed < maxBandsSpeed) {
            foreach (GameObject objectToTeleport in objectsToTeleport) {
                if (objectToTeleport != null) {
                    objectToTeleport.GetComponent<Rigidbody>().velocity = new Vector3(0, Random.Range(0f, 6f), 0);
                }
            }

            band1Rotation.y += Time.deltaTime * bandsSpeed * 0.8f;
            band2Rotation.y -= Time.deltaTime * bandsSpeed * 0.9f;
            band3Rotation.y += Time.deltaTime * bandsSpeed;

            band1.localRotation = Quaternion.Euler(band1Rotation);
            band2.localRotation = Quaternion.Euler(band2Rotation);
            band3.localRotation = Quaternion.Euler(band3Rotation);

            bandsSpeed *= bandsAcceleration;
            yield return null;
        }

        foreach (GameObject objectToTeleport in objectsToTeleport) {
            if (objectToTeleport != null) {
                objectToTeleport.transform.position = arrivalStation.position;
            }
        }

        StartCoroutine("MergeBands");
    }

    IEnumerator MergeBands() {

        bandsSpeed = 10;
        isRecovering = true;

        Vector3 band1Rotation = band1.localRotation.eulerAngles;
        Vector3 band2Rotation = band2.localRotation.eulerAngles;
        Vector3 band3Rotation = band3.localRotation.eulerAngles;

        while (band1Rotation.y > -360 * finalLoops) {
            band1Rotation.y -= 360;
        }
        while (band2Rotation.y < 360 * finalLoops) {
            band2Rotation.y += 360;
        }
        while (band3Rotation.y > -360 * finalLoops) {
            band3Rotation.y -= 360;
        }

        while (top.localPosition.y > 0) {
            float percentage = Mathf.InverseLerp(2, 0, top.localPosition.y);
            band1Rotation.y = Mathf.Lerp(band1Rotation.y, 0, percentage);
            band2Rotation.y = Mathf.Lerp(band2Rotation.y, 0, percentage);
            band3Rotation.y = Mathf.Lerp(band3Rotation.y, 0, percentage);

            band1.localRotation = Quaternion.Euler(band1Rotation);
            band2.localRotation = Quaternion.Euler(band2Rotation);
            band3.localRotation = Quaternion.Euler(band3Rotation);

            percentage += Time.deltaTime;

            if (band3.localPosition.y <= finalLoops * 0.5) {
                finalLoops--;
            }

            band1.localPosition -= new Vector3(0, Time.deltaTime * bandSeparationSpeed * 0.5f, 0);
            band2.localPosition -= new Vector3(0, Time.deltaTime * bandSeparationSpeed * 1, 0);
            band3.localPosition -= new Vector3(0, Time.deltaTime * bandSeparationSpeed * 1.5f, 0);
            top.localPosition -= new Vector3(0, Time.deltaTime * bandSeparationSpeed * 2, 0);

            yield return null;
        }
        band1.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        band2.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        band3.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        band1.localPosition = new Vector3(0, 0, 0);
        band2.localPosition = new Vector3(0, 0, 0);
        band3.localPosition = new Vector3(0, 0, 0);
        top.localPosition = new Vector3(0, 0, 0);

        barrier.SetActive(false);
        foreach (GameObject objectToTeleport in objectsToTeleport) {
            if (objectToTeleport != null) {
                print("something");
                objectToTeleport.GetComponent<Controller>().Stun(0);
            }
        }
        barrier.GetComponent<TeleportBarrier>().objectsToTeleport.Clear();
        objectsToTeleport.Clear();

        teleportEffect.SetActive(false);

        isIdle = true;
        isRecovering = false;

        player.EnableRecording();
    }
}
