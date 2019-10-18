using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordablePhysics : RecordableTransform {

    Rigidbody rb;

    public override void Start() {
        rb = GetComponent<Rigidbody>();
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public override void Record() {
        base.Record();
        recordings[nClones].Add(new RecordedPhysics(rb));
    }

    public override void Reproduce() {
        RecordedPhysics p = recordings[nClones-1][reproductionIndex[nClones-1]] as RecordedPhysics;
        rb.velocity = p.velocity;
        rb.angularVelocity = p.angularVelocity;
    }

    public class RecordedPhysics : RecordedItem {
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public RecordedPhysics(Rigidbody rb_) {
            velocity = rb_.velocity;
            angularVelocity = rb_.angularVelocity;
        }
    }
}
