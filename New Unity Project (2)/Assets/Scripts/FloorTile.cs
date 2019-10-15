using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour {

    public Color overColor;
    public float enterSpeed;
    public float exitSpeed;

    Color defaultColor;
    Transform collidingObject;

    void Start() {
        defaultColor = GetComponent<Renderer>().material.color;
    }

    // void Update() {
    //     if (collidingObject == null) {
    //         StartCoroutine("Off");
    //     }
    // }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            //collidingObject = collision.gameObject.transform;
            StopCoroutine("Off");
            StartCoroutine("On");
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            StopCoroutine("On");
            StartCoroutine("Off");
        }
    }

    IEnumerator On() {
        Material material = GetComponent<Renderer>().material;
        Color startColor = material.color;
        float percentage = 0;

        while (percentage < 1) {

            material.color = Color.Lerp(startColor, overColor, percentage);

            percentage += Time.deltaTime * enterSpeed;
            yield return null;
        }
    }

    IEnumerator Off() {
        Material material = GetComponent<Renderer>().material;
        Color startColor = material.color;
        float percentage = 0;

        while (percentage < 1) {

            material.color = Color.Lerp(startColor, defaultColor, percentage);

            percentage += Time.deltaTime * exitSpeed;
            yield return null;
        }
    }
}
