using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWall : MutableObject {
    
    List<ElectricCollider> electricColliders = new List<ElectricCollider>();
    List<ParticleSystem> electricity = new List<ParticleSystem>();

    void Awake() {
        foreach (Transform child in transform) {
            electricColliders.Add(child.Find("Collider").GetComponent<ElectricCollider>());
            electricity.Add(child.Find("Electricity").GetComponent<ParticleSystem>());
        }
    }

    public override bool ChangeState(bool isActive) {
        for (int i = 0; i < electricColliders.Count; i++) {
            electricColliders[i].gameObject.SetActive(isActive);
            electricity[i].gameObject.SetActive(isActive);
        }
        return true;
    }
}
