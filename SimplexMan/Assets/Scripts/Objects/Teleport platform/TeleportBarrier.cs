using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBarrier : MonoBehaviour {

    float repulsiveForce = 30;
    public GameObject collisionEffect;
    float stunnedTime = 1;
    public List<GameObject> objectsToTeleport;
    
    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            if (!objectsToTeleport.Contains(collider.gameObject)) {
                Repulse(collider);
            }
        }
    }

    void OnTriggerStay(Collider collider) {
        if (collider.tag == "Player" || collider.tag == "Clone") {
            if (!objectsToTeleport.Contains(collider.gameObject)) {
                Repulse(collider);
            }
        }
    }

    void Repulse(Collider collider) {
        print("Repulse");
        Vector3 direction = (collider.transform.position - transform.position);
        direction.y = 0;
        direction.Normalize();
        collider.gameObject.GetComponent<Controller>().Stun(stunnedTime);
        collider.attachedRigidbody.AddForce(direction * repulsiveForce, ForceMode.Impulse);
        Destroy(Instantiate(collisionEffect, collider.transform.position, Quaternion.identity), 0.2f);
    }
}
