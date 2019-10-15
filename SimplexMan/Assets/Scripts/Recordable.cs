using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recordable : MonoBehaviour {
    
    int recordLength = 0;

    public virtual void Start() {
        FindObjectOfType<PlayerController>().StartRecording += StartRecording;
        FindObjectOfType<PlayerController>().StopRecording += StopRecording;
    }

    public virtual void StartRecording() {
        StartCoroutine("ERecord");
    }

    public virtual void StopRecording() {
        StopCoroutine("ERecord");
        StartCoroutine("EReproduce");
    }

    public virtual void Record() {
        recordLength++;
    }

    public virtual void Reproduce(int i) {
        
    }

    IEnumerator ERecord() {
        while (true) {
            Record();
            yield return null;
        }
    }

    IEnumerator EReproduce() {
        for (int i = 0; i < recordLength; i++) {
            Reproduce(i);
            yield return null;
        }
    }
}
