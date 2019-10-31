using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveCollider : Recordable {

    protected bool isEnabled = false;
    
    int nCollidingObjects = 0;

    public virtual void PlayerInteraction() {}
    public virtual void StopPlayerInteraction() {}

    // Recorded initial state
    bool initialPowerState;

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
