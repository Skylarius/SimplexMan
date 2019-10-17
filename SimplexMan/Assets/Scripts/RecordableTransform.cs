using System.Collections;
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
        recordings[recordingIndex].Add(new RecordedTransform(transform));
    }

    public override void Reproduce() {
        RecordedTransform t = recordings[recordingIndex][reproductionIndex[recordingIndex]] as RecordedTransform;
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
