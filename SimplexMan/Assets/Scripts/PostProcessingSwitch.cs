using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingSwitch : MonoBehaviour {
    
    public float enterSpeed;
    public float exitSpeed;
    public Color vignetteSymplexColor;
    public float chromaticAberrationSymplexIntensity;

    PostProcessVolume volume;
    ChromaticAberration chromaticAberration;
    Vignette vignette;
    Color vignetteDefaultColor;
    float chromaticAberrationDefaultIntensity;

    void Start() {
        volume = GetComponent<PostProcessVolume>();
        
        volume.profile.TryGetSettings(out chromaticAberration);
        volume.profile.TryGetSettings(out vignette);

        vignetteDefaultColor = vignette.color;
        chromaticAberrationDefaultIntensity = chromaticAberration.intensity;

        FindObjectOfType<PlayerController>().StartRecording += StartRecording;
        FindObjectOfType<PlayerController>().StopRecording += StopRecording;
    }

    void StartRecording() {
        StopCoroutine("EToDefault");
        StartCoroutine("EToSymplex");
    }

    void StopRecording() {
        StopCoroutine("EToSymplex");
        StartCoroutine("EToDefault");
    }

    IEnumerator EToSymplex() {
        float percentage = 0;
        while (percentage <= 1) {
            vignette.color.Override(Color.Lerp(vignetteDefaultColor, vignetteSymplexColor, percentage));
            chromaticAberration.intensity.Override(Mathf.Lerp(chromaticAberrationDefaultIntensity, chromaticAberrationSymplexIntensity, percentage));

            percentage += Time.deltaTime * enterSpeed;
            yield return null;
        }
        vignette.color.Override(vignetteSymplexColor);
        chromaticAberration.intensity.Override(chromaticAberrationSymplexIntensity);
    }

    IEnumerator EToDefault() {
        Color startColor = vignette.color;
        float startIntensity = chromaticAberration.intensity;

        float percentage = 0;
        while (percentage < 1) {
            vignette.color.Override(Color.Lerp(startColor, vignetteDefaultColor, percentage));
            chromaticAberration.intensity.Override(Mathf.Lerp(startIntensity, chromaticAberrationDefaultIntensity, percentage));

            percentage += Time.deltaTime * exitSpeed;
            yield return null;
        }
        vignette.color.Override(vignetteDefaultColor);
        chromaticAberration.intensity.Override(chromaticAberrationDefaultIntensity);
    }
}
