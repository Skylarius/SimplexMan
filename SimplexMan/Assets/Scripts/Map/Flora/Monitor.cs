using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : MonoBehaviour {

    public enum State {On, Off};
    public State state;

    List<Renderer> electricity = new List<Renderer>();
    Renderer indicatorLight;
    GameObject noise;

    void Awake() {
        foreach (Transform child in transform.Find("Cables")) {
            electricity.Add(child.GetComponent<Renderer>());
        }
        indicatorLight = transform.Find("Screen").Find("IndicatorLight").GetComponent<Renderer>();
        noise = transform.Find("Screen").Find("Noise").gameObject;
        SetState(state);
    }

    void SetState(State state) {
        if (state == State.On) {
            noise.SetActive(true);
            foreach (Renderer r in electricity) {
                r.material.EnableKeyword("_EMISSION");
            }
            indicatorLight.material.SetColor("_EmissionColor", Color.green);
        } else {
            noise.SetActive(false);
            foreach (Renderer r in electricity) {
                r.material.DisableKeyword("_EMISSION");
            }
            indicatorLight.material.SetColor("_EmissionColor", Color.red);
        }
    }
}
