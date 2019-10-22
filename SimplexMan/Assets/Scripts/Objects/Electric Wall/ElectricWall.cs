using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWall : MonoBehaviour {
    
    ElectricCollider[] electricColliders;
    ParticleSystem[] electricity;

    void Awake() {
        electricColliders = GetComponentsInChildren<ElectricCollider>();
        electricity = GetComponentsInChildren<ParticleSystem>();
    }

    public void ChangeState(bool isActive) {
        for (int i = 0; i < electricColliders.Length; i++) {
            electricColliders[i].gameObject.SetActive(isActive);
            electricity[i].gameObject.SetActive(isActive);
        }
    }
}
