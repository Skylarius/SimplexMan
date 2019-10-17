using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingSwitch : MonoBehaviour {
    
    public float enterSpeed;
    public float exitSpeed;
    public Color vignetteSymplexColor;
    public float chromaticAberrationSymplexIntensity;
    public float lensDistortionSymplexIntensity;

    PostProcessVolume volume;
    ChromaticAberration chromaticAberration;
    Vignette vignette;
    LensDistortion lensDistortion;
    Color vignetteDefaultColor;
    float chromaticAberrationDefaultIntensity;
    float lensDistortionDefaultIntensity;

    void Start() {
        volume = GetComponent<PostProcessVolume>();
        
        volume.profile.TryGetSettings(out chromaticAberration);
        volume.profile.TryGetSettings(out vignette);
        volume.profile.TryGetSettings(out lensDistortion);

        vignetteDefaultColor = vignette.color;
        chromaticAberrationDefaultIntensity = chromaticAberration.intensity;
        lensDistortionDefaultIntensity = lensDistortion.intensity;

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
            lensDistortion.intensity.Override(Mathf.Lerp(lensDistortionDefaultIntensity, lensDistortionSymplexIntensity, percentage));

            percentage += Time.deltaTime * enterSpeed;
            yield return null;
        }
        vignette.color.Override(vignetteSymplexColor);
        chromaticAberration.intensity.Override(chromaticAberrationSymplexIntensity);
        lensDistortion.intensity.Override(lensDistortionSymplexIntensity);
    }

    IEnumerator EToDefault() {
        Color startVColor = vignette.color;
        float startCAIntensity = chromaticAberration.intensity;
        float startLDIntensity = lensDistortion.intensity;

        float percentage = 0;
        while (percentage < 1) {
            vignette.color.Override(Color.Lerp(startVColor, vignetteDefaultColor, percentage));
            chromaticAberration.intensity.Override(Mathf.Lerp(startCAIntensity, chromaticAberrationDefaultIntensity, percentage));
            lensDistortion.intensity.Override(Mathf.Lerp(startLDIntensity, lensDistortionDefaultIntensity, percentage));

            percentage += Time.deltaTime * exitSpeed;
            yield return null;
        }
        vignette.color.Override(vignetteDefaultColor);
        chromaticAberration.intensity.Override(chromaticAberrationDefaultIntensity);
        lensDistortion.intensity.Override(lensDistortionDefaultIntensity);
    }
}
