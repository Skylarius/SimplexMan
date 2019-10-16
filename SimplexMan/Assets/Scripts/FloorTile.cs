using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour {

    public Color overColorPlayer;
    public Color overColorClone;
    public float enterSpeed;
    public float exitSpeed;

    Color defaultColor;
    Transform collidingObject;

    void Start() {
        defaultColor = GetComponent<Renderer>().material.color;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            StopCoroutine("Off");
            StartCoroutine("On");
        } else if (collision.gameObject.tag == "Clone") {
            StopCoroutine("Off");
            StartCoroutine("OnClone", collision.gameObject);
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            StopCoroutine("On");
            StartCoroutine("Off");
        } else if (collision.gameObject.tag == "Clone") {
            StopCoroutine("OnClone");
            StartCoroutine("Off");
        }
    }

    IEnumerator On() {
        Material material = GetComponent<Renderer>().material;
        Color startColor = material.color;
        float percentage = 0;

        while (percentage < 1) {

            material.color = Color.Lerp(startColor, overColorPlayer, percentage);

            percentage += Time.deltaTime * enterSpeed;
            yield return null;
        }
    }

    IEnumerator OnClone(GameObject clone) {
        Material material = GetComponent<Renderer>().material;
        Color startColor = material.color;
        float percentage = 0;

        while (percentage < 1) {

            material.color = Color.Lerp(startColor, overColorClone, percentage);

            percentage += Time.deltaTime * enterSpeed;
            yield return null;
        }

        while(true) {
            if (clone == null) {
                break;
            }
            yield return null;
        }
        StartCoroutine("Off");
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
