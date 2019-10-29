using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recordable : MonoBehaviour {
   
    protected int nClones = 0;
    protected List<List<RecordedItem>> recordings;
    protected List<int> reproductionIndex = new List<int>();
    // keeps track of the number of frames recorded in that current recording (not all of them)
    protected int nRecordedFrames = 0;

    bool isReproducing = false;

    protected virtual void Start() {
        recordings = new List<List<RecordedItem>>();
        FindObjectOfType<PlayerController>().StartRecording += StartRecording;
        FindObjectOfType<PlayerController>().StopRecording += StopRecording;
    }

    protected virtual void Update() {
        if (isReproducing) {
            if (reproductionIndex[nClones-1] >= recordings[nClones-1].Count) {
                recordings.RemoveAt(nClones-1);
                reproductionIndex.RemoveAt(nClones-1);
                nClones--;
                if (nClones == 0) {
                    isReproducing = false;
                }
            } else {
                Reproduce();
                reproductionIndex[nClones-1]++;
            }
        }        
    }

    protected virtual void StartRecording() {
        recordings.Add(new List<RecordedItem>());
        nRecordedFrames = 0;
        StartCoroutine("ERecord");
    }

    protected virtual void StopRecording() {
        StopCoroutine("ERecord");
        nClones++;
        reproductionIndex.Add(0);
        isReproducing = true;
    }

    IEnumerator ERecord() {
        while (true) {
            Record();
            yield return null;
        }
    }

    protected virtual void Record() {
        nRecordedFrames++;
    }

    protected virtual void Reproduce() { }

    protected class RecordedItem { }
}