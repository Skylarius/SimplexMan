﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordableTransform : Recordable {

    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public override void Record() {
        base.Record();
        recordings[nClones].Add(new RecordedTransform(transform));
    }

    public override void Reproduce() {
        base.Reproduce();
        RecordedTransform t = recordings[nClones-1][reproductionIndex[nClones-1]] as RecordedTransform;
        transform.position = t.position;
        transform.rotation = t.rotation;
        transform.localScale = t.scale;
    }

    public class RecordedTransform : RecordedItem {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public RecordedTransform(Transform t) {
            position = t.position;
            rotation = t.rotation;
            scale = t.localScale;
        }
    }
}
