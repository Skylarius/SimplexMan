using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneController : Recordable {

    Rigidbody rb;
    
    // Recording variables
    List<Vector3> position = new List<Vector3>();
    List<Quaternion> rotation = new List<Quaternion>();
    List<Vector3> scale = new List<Vector3>();
    List<Vector3> velocity = new List<Vector3>();
    List<bool> isInteracting = new List<bool>();

    public override void Start() {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    public override void Record() {
        base.Record();
        position.Add(transform.position);
        rotation.Add(transform.rotation);
        scale.Add(transform.localScale);
        velocity.Add(rb.velocity);
        //if (isDownInteract) {
        //    isInteracting.Add(true);
        //} else {
        //    isInteracting.Add(false);
        //}
    }

    public override void Reproduce(int i) {
        base.Reproduce(i);
        transform.position = position[i];
        transform.rotation = rotation[i];
        transform.localScale = scale[i];
        rb.velocity = velocity[i];
        //if (isInteracting[i]) {
        //    PlayerInteraction();
        //}
    }

    public override void StopRecording() {
        //isInteracting = CleanList(isInteracting);
        
        transform.position = position[0];
        transform.rotation = rotation[0];
        transform.localScale = scale[0];
        rb.velocity = velocity[0];
        //if (isInteracting[0]) {
        //    PlayerInteraction();
        //}
        
        base.StopRecording();

        //Vector3 deathPosition = cloneT.position;
        //Destroy(clone);
        //Destroy(Instantiate(deathEffect.gameObject, deathPosition, Quaternion.identity), 2);
    }

    public List<bool> CleanList(List<bool> dirtyList) {
        List<bool> list = dirtyList;
        bool isCleaning = false;
        for (int i = 0; i < list.Count; i++) {
            if (list[i] == false) {
                isCleaning = false;
            } else if (list[i] == true && !isCleaning) {
                isCleaning = true;
            } else {
                list[i] = false;
            }
        }
        return list;
    }
}