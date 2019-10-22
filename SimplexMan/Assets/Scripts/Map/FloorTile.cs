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

    int nCollidingObjects = 0;

    void Start() {
        defaultColor = GetComponent<Renderer>().material.color;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            StopCoroutine("Off");
            StartCoroutine("On", overColorPlayer);
            nCollidingObjects++;
        } else if (collision.gameObject.tag == "Clone") {
            StopCoroutine("Off");
            StartCoroutine("On", overColorClone);
            StartCoroutine("OnCollisionExitClone", collision.collider);
            nCollidingObjects++;
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Clone") {
            nCollidingObjects--;
            if (collision.gameObject.tag == "Clone") {
                StopCoroutine("OnCollisionExitClone");
            }
            
            if (nCollidingObjects == 0) {
                StopCoroutine("On");
                StopCoroutine("OnClone");
                StartCoroutine("Off");
            } else if (collision.gameObject.tag == "Player") {
                StartCoroutine("On", overColorClone);
            } else {
                StartCoroutine("On", overColorPlayer);
            }           
        }
    }

    IEnumerator On(Color targetColor) {
        Material material = GetComponent<Renderer>().material;
        Color startColor = material.color;
        float percentage = 0;

        while (percentage <= 1) {

            material.color = Color.Lerp(startColor, targetColor, percentage);

            percentage += Time.deltaTime * enterSpeed;
            yield return null;
        }
        material.color = targetColor;
    }

    IEnumerator Off() {
        Material material = GetComponent<Renderer>().material;
        Color startColor = material.color;
        float percentage = 0;

        while (percentage <= 1) {

            material.color = Color.Lerp(startColor, defaultColor, percentage);

            percentage += Time.deltaTime * exitSpeed;
            yield return null;
        }
        material.color = defaultColor;
    }

    IEnumerator OnCollisionExitClone(Collider clone) {
        while(true) {
            if (clone == null || !clone.enabled) {
                nCollidingObjects--;
                if (nCollidingObjects == 0) {
                    StartCoroutine("Off");
                }
                break;
            }
            yield return null;
        }
    }
}
