using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricCollider : MonoBehaviour {

    float repulsiveForce = 25;
    public GameObject collisionEffect;
    float stunnedTime = 1;
    
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
        collider.gameObject.GetComponent<Controller>().Stun(stunnedTime);
        collider.attachedRigidbody.AddForce(direction * repulsiveForce, ForceMode.Impulse);
        Destroy(Instantiate(collisionEffect, collider.transform.position, Quaternion.identity), stunnedTime);
    }
}
