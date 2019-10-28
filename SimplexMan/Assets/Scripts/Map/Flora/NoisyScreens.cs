using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisyScreens : MonoBehaviour {
    public Material material;

    void Update()
    {
        material.mainTexture = NoisyScreen.UpdateTexture();;
    }
}
