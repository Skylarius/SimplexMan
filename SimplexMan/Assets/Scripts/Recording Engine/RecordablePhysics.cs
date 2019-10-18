using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordablePhysics : Recordable {

    Rigidbody rb;

    public override void Start() {
        rb = GetComponent<Rigidbody>();
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public override void Record() {
        recordings[nClones].Add(new RecordedPhysics(transform, rb));
        base.Record();
    }

    public override void Reproduce() {
        RecordedPhysics p = recordings[nClones-1][reproductionIndex[nClones-1]] as RecordedPhysics;
        transform.position = p.position;
        transform.rotation = p.rotation;
        transform.localScale = p.scale;
        rb.velocity = p.velocity;
        rb.angularVelocity = p.angularVelocity;
        base.Reproduce();
    }

    public class RecordedPhysics : RecordedItem {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public RecordedPhysics(Transform _t, Rigidbody _rb) {
            position = _t.position;
            rotation = _t.rotation;
            scale = _t.localScale;
            velocity = _rb.velocity;
            angularVelocity = _rb.angularVelocity;
        }
    }
}
