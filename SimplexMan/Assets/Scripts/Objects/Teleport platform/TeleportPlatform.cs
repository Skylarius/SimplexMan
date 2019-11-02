using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlatform : MutableObject, IPower {
    
    public TeleportPlatform arrivalStation;
    
    GameObject barrier;
    GameObject teleportEffect;
    Transform band1;
    Transform band2;
    Transform band3;
    Transform top;
    Transform arrivalPoint;
    List<Renderer> electricity = new List<Renderer>();

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
    bool hasPower = false;
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

    void Awake() {
        player = FindObjectOfType<PlayerController>();
        band1 = transform.Find("Structure").Find("Band1");
        band2 = transform.Find("Structure").Find("Band2");
        band3 = transform.Find("Structure").Find("Band3");
        top  = transform.Find("Structure").Find("Top");
        teleportEffect = top.Find("Teleport effect").gameObject;
        barrier = transform.Find("Barrier").gameObject;
        foreach (Transform child in transform.Find("Structure")) {
            if (child.name == "Band1" || child.name == "Band2" || child.name == "Band3") {
                foreach (Transform c in child.Find("Light")) {
                    electricity.Add(c.GetComponent<Renderer>());
                }
            }
        }
        foreach (Transform child in transform.Find("Control station B").Find("Connection")) {
            electricity.Add(child.GetComponent<Renderer>());
        }
        electricity.Add(transform.Find("Control station B").Find("LeverObject").Find("Electricity").GetComponent<Renderer>());
        if (arrivalStation != null) {
            arrivalPoint = arrivalStation.transform.Find("Arrival point");
        }
    }

    protected override void Start() {
        //SetPower(hasPower);
        base.Start();
    }

    public override bool ChangeState(bool state) {
        if (!state) {
            activatedControllers++;
            if (activatedControllers == 1 && isIdle) {
                StartTeleport(true);
                arrivalStation.StartTeleport(false);
            } 
        } else if (activatedControllers > 0) {
            activatedControllers--;
            if (!isRecovering && !isIdle) {
                StopTeleport(true);
                arrivalStation.StopTeleport(false);
            }
        }
        return true;
    }

    public void SetPower(bool _hasPower) {
        hasPower = _hasPower;
        foreach (Renderer r in electricity) {
            if (hasPower) {
                r.material.EnableKeyword("_EMISSION");
            } else {
                r.material.DisableKeyword("_EMISSION");
            }
        }
        if (arrivalStation != null) {
            arrivalStation.SetPower(_hasPower);
        }
    }

    public void StartTeleport(bool isTeleporting) {
        isRecovering = false;
        isIdle = false;

        barrier.SetActive(true);
        barrier.GetComponent<TeleportBarrier>().objectsToTeleport = objectsToTeleport;
        
        StartCoroutine("SeparateBands", isTeleporting);
    }

    public void StopTeleport(bool isReceiving) {
        teleportEffect.SetActive(false);
        StopCoroutine("SeparateBands");
        StartCoroutine("MergeBands", isReceiving);
    }

    protected override void StartRecording() {
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

    protected override void StopRecording() {
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

    IEnumerator SeparateBands(bool isTeleporting) {
        
        if (isTeleporting) {
            player.DisableRecording();
            foreach (GameObject objectToTeleport in objectsToTeleport) {
                if (objectToTeleport != null) {
                    objectToTeleport.GetComponent<Controller>().Stun(10);
                }
            }
        }

        teleportEffect.SetActive(true);

        Vector3 band1Rotation = band1.localRotation.eulerAngles;
        Vector3 band2Rotation = band2.localRotation.eulerAngles;
        Vector3 band3Rotation = band3.localRotation.eulerAngles;

        while (top.localPosition.y <= 2) {
            if (isTeleporting) {
                foreach (GameObject objectToTeleport in objectsToTeleport) {
                    if (objectToTeleport != null) {
                        objectToTeleport.GetComponent<Rigidbody>().velocity = new Vector3(0, Random.Range(0f, 6f), 0);
                    }
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
            if (isTeleporting) {
                foreach (GameObject objectToTeleport in objectsToTeleport) {
                    if (objectToTeleport != null) {
                        objectToTeleport.GetComponent<Rigidbody>().velocity = new Vector3(0, Random.Range(0f, 6f), 0);
                    }
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

        if (isTeleporting) {
            foreach (GameObject objectToTeleport in objectsToTeleport) {
                if (objectToTeleport != null) {
                    objectToTeleport.transform.position = arrivalPoint.position;
                }
            }
        }

        StartCoroutine("MergeBands", !isTeleporting);
    }

    IEnumerator MergeBands(bool isReceiving) {

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
        barrier.GetComponent<TeleportBarrier>().objectsToTeleport.Clear();
        if (isReceiving) {
            foreach (GameObject objectToTeleport in objectsToTeleport) {
                if (objectToTeleport != null) {
                    objectToTeleport.GetComponent<Controller>().Stun(0);
                }
            }
            player.EnableRecording();
        }
        objectsToTeleport.Clear();

        teleportEffect.SetActive(false);

        isIdle = true;
        isRecovering = false;
    }
}
