using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWall : MutableObject {
    
    List<ElectricCollider> electricColliders = new List<ElectricCollider>();
    List<ParticleSystem> electricityEffect = new List<ParticleSystem>();
    List<Renderer> electricity = new List<Renderer>();

    void Awake() {
        foreach (Transform child in transform) {
            electricColliders.Add(child.Find("Collider").GetComponent<ElectricCollider>());
            electricityEffect.Add(child.Find("Electricity effect").GetComponent<ParticleSystem>());
            electricity.Add(child.Find("Electricity").GetComponent<Renderer>());
        }
    }

    public override bool ChangeState(bool isActive) {
        for (int i = 0; i < electricColliders.Count; i++) {
            electricColliders[i].gameObject.SetActive(isActive);
            electricityEffect[i].gameObject.SetActive(isActive);
            if (isActive) {
                electricity[i].material.EnableKeyword("_EMISSION");
            } else {
                electricity[i].material.DisableKeyword("_EMISSION");
            }
        }
        return true;
    }
}
