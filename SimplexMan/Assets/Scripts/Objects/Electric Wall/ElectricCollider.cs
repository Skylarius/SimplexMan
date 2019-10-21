using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricCollider : MonoBehaviour {

    public float repulsiveForce;
    public GameObject collisionEffect;

    void Start() {

    }
    
    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            Repulse(collider);
        }
    }

    void OnTriggerStay(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            Repulse(collider);
        }
    }

    void Repulse(Collider collider) {
        Vector3 direction = (collider.transform.position - transform.position);
        direction.y = 0;
        direction.Normalize();
        collider.gameObject.GetComponent<PlayerController>().Stun();
        collider.attachedRigidbody.AddForce(direction * repulsiveForce, ForceMode.Impulse);
        Destroy(Instantiate(collisionEffect, collider.transform.position, Quaternion.identity), 1);
    }
}
