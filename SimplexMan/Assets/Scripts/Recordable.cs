// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Recordable : MonoBehaviour {
       
//     public enum State {Idle, Reproducing};
//     public State state = State.Idle;
//     public int recordingIndex = -1;
//     public List<int> reproductionIndex = new List<int>();

//     public List<List<RecordedItem>> recordings;

//     public virtual void Start() {
//         FindObjectOfType<PlayerController>().StartRecording += StartRecording;
//         FindObjectOfType<PlayerController>().StopRecording += StopRecording;
//     }

//     public virtual void Update() {
//         if (state == State.Reproducing) {
//             if (reproductionIndex[recordingIndex] >= recordings[recordingIndex].Count) {
//                 recordings.RemoveAt(recordingIndex);
//                 recordingIndex--;
//                 if (recordingIndex == -1) {
//                     state = State.Idle;
//                 }
//             } else {
//                 Reproduce();
//                 reproductionIndex[recordingIndex]++;
//             }
//         }        
//     }

//     public virtual void StartRecording() {
//         recordingIndex++;
//         recordings.Add(new List<RecordedItem>());
//         StartCoroutine("ERecord");
//     }

//     public virtual void StopRecording() {
//         reproductionIndex.Add(0);
//         state = State.Reproducing;
//     }

//     IEnumerator ERecord() {
//         while (true) {
//             Record();
//             yield return null;
//         }
//     }

//     public virtual void Record() { }

//     public virtual void Reproduce() { }

//     public class RecordedItem {

//     }
    
// }

// /*

// // OLD VERSION

// public virtual void Start() {
//         FindObjectOfType<PlayerController>().StartRecording += StartRecording;
//         FindObjectOfType<PlayerController>().StopRecording += StopRecording;
//     }

//     public virtual void StartRecording() {
//         StartCoroutine("ERecord");
//     }

//     public virtual void StopRecording() {
//         StopCoroutine("ERecord");
//         StartCoroutine("EReproduce");
//     }

//     public virtual void Record() {
//         recordLength++;
//     }

//     public virtual void Reproduce(int i) {
        
//     }

//     IEnumerator ERecord() {
//         while (true) {
//             Record();
//             yield return null;
//         }
//     }

//     IEnumerator EReproduce() {
//         for (int i = 0; i < recordLength; i++) {
//             Reproduce(i);
//             yield return null;
//         }
//     }
// */