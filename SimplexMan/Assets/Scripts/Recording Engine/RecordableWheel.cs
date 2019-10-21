using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordableWheel : Recordable {
    
    public bool isActive = true;

    bool initialActivatedState;

    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public override void Record() {
        recordings[nClones].Add(new RecordedInteraction(isActive));
        base.Record();
    }

    public override void StartRecording() {
        initialActivatedState = isActive;
        base.StartRecording();
    }

    public override void StopRecording() {
        if (isActive != initialActivatedState) {
            isActive = initialActivatedState;
            ResetState(isActive);
        }
        base.StopRecording();
    }

    public class RecordedInteraction : RecordedItem {
        public bool hasInteracted;

        public RecordedInteraction(bool _hasInteracted) {
            hasInteracted = _hasInteracted;
        }
    }

    protected virtual void ResetState(bool _isActive) {}
}
