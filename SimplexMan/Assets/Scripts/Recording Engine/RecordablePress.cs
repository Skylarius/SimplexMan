using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordablePress : Recordable {

    public bool isActive = true;

    bool initialActivatedState;

    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    // If the value of isActive changes, it means there has been an interaction to record
    // ISSUE: the button does not activate when using the recorded value, probably because 
    // the frame where the interaction is spawned by the clone is not the same frame where 
    // the interaction is recorded
    // SOLUTION: once the new cloning method is implemented the two frames will correspond
    // automatically
    public override void Record() {
        if (nRecordedFrames == 0) {
            recordings[nClones].Add(new RecordedInteraction(false));
        } else {
            bool hasChanged = isActive != ((RecordedInteraction)recordings[nClones][nRecordedFrames-1]).hasInteracted;
            recordings[nClones].Add(new RecordedInteraction(hasChanged));
        }
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
