using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordablePlayer : MonoBehaviour {

    public GameObject clonePrefab;

    public List<RecordedInput> recordings;
    RecordableClone cloneController;
    Vector3 initialPosition;
    Quaternion initialRotation;

    void Start() {
        FindObjectOfType<PlayerController>().StartRecording += StartRecording;
        FindObjectOfType<PlayerController>().StopRecording += StopRecording;
    }

    void StartRecording() {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        recordings = new List<RecordedInput>();
        StartCoroutine("ERecord");
    }

    void StopRecording() {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        StopCoroutine("ERecord");
        GameObject clone = Instantiate(clonePrefab, transform.position, transform.rotation);
        clone.GetComponent<RecordableClone>().SetInputs(new List<RecordedInput>(recordings));
    }

    IEnumerator ERecord() {
        // Save the first frame separately so that, if the player is already
        // holding the interaction button, it is recorded.
        recordings.Add(new RecordedInput());
        recordings[0].interactDown = Input.GetButton("Interact");
        yield return null;

        while (true) {
            recordings.Add(new RecordedInput());
            yield return null;
        }
    }

    public class RecordedInput {
        public float horizontal;
        public float vertical;
        public float mouseX;
        public bool jump;
        public bool interactDown;
        public bool interactUp;

        public RecordedInput() {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            mouseX = Input.GetAxisRaw("Mouse X");;
            jump = Input.GetButtonDown("Jump");
            interactDown = Input.GetButtonDown("Interact");
            interactUp = Input.GetButtonUp("Interact");
        }
    }
}
