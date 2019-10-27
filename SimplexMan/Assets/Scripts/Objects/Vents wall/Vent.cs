using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MutableObject {
    
    public Transform helix;
    public Renderer electricity;
    public GameObject windPrefab;

    float maxWindForce = 60;
    float maxHelixSpeed = 300;
    float helixSpeed = 0;
    Vector3 helixRotation;
    bool isOn = true;
    GameObject wind;
    float maxWindRateOverTime = 50;

    // Recordable initial state
    float initialHelixSpeed;
    float initialWindRate;

    void Awake() {
        wind = Instantiate(windPrefab, transform.position, Quaternion.identity);
        wind.transform.parent = transform.parent;
        wind.transform.localRotation = transform.localRotation;
        helixRotation = helix.localRotation.eulerAngles;
        helixRotation.z = Random.Range(0, 30);
    }

    public override void Update() {
        helixRotation.z += Time.deltaTime * helixSpeed;
        helix.localRotation = Quaternion.Euler(helixRotation);
        base.Update();
    }

    public override void ChangeState(bool state) {
        if (state == true) {
            electricity.material.EnableKeyword("_EMISSION");
            StopCoroutine("Off");
            StartCoroutine("On");
        } else {
            electricity.material.DisableKeyword("_EMISSION");
            StopCoroutine("On");
            StartCoroutine("Off");
        }
    }

    public override void StartRecording() {
        initialHelixSpeed = helixSpeed;
        initialWindRate = wind.GetComponent<ParticleSystem>().emission.rateOverTime.constant;
        base.StartRecording();
    }

    public override void StopRecording() {
        helixSpeed = initialHelixSpeed;
        var emission = wind.GetComponent<ParticleSystem>().emission;
        emission.rateOverTime = initialWindRate;
        base.StopRecording();
    }

    void OnTriggerStay(Collider collider) {
        if (isOn) {
            if (collider.tag == "Player" || collider.tag == "Clone") {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance > maxWindForce) {
                    distance = maxWindForce;
                }
                float force = Mathf.Pow((maxWindForce - distance) * 0.1f, 2);
                collider.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.VelocityChange);
            }
        }
    }

    IEnumerator On() {
        var emission = wind.GetComponent<ParticleSystem>().emission;
        float rate = emission.rateOverTime.constant;
        while (helixSpeed < maxHelixSpeed) {
            helixSpeed += Time.deltaTime * 500;
            rate += Time.deltaTime * 500 * maxWindRateOverTime / maxHelixSpeed;
            emission.rateOverTime = rate;
            yield return null;
        }
        isOn = true;
    }

    IEnumerator Off() {
        isOn = false;
        var emission = wind.GetComponent<ParticleSystem>().emission;
        float rate = emission.rateOverTime.constant;
        while (helixSpeed > 0) {
            helixSpeed -= Time.deltaTime * 100;
            rate -= Time.deltaTime * 100 * maxWindRateOverTime / maxHelixSpeed;
            emission.rateOverTime = rate;
            yield return null;
        }
        helixSpeed = 0;
        emission.rateOverTime = 0;
    }
}
