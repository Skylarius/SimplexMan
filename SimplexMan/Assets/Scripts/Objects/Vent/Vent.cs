using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour {
    

    public float maxWindForce;
    public Transform helix;
    float helixSpeed = 300;
    Vector3 helixRotation;

    void Start() {
        helixRotation = helix.localRotation.eulerAngles;
    }

    void Update() {
        helixRotation.z += Time.deltaTime * helixSpeed;
        helix.localRotation = Quaternion.Euler(helixRotation);        
    }

    void OnTriggerStay(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            print("Colliding");
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance > maxWindForce) {
                distance = maxWindForce;
            }
            collider.GetComponent<Rigidbody>().AddForce(transform.forward * (maxWindForce - distance), ForceMode.VelocityChange);
        }
    }
}
