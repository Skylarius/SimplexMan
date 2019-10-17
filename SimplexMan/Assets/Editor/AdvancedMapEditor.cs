using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGeneratorAdvanced))]
public class AdvancedMapEditor : Editor {

    public override void OnInspectorGUI() {    
        MapGeneratorAdvanced map = target as MapGeneratorAdvanced;
        DrawDefaultInspector();
        // if (){
        //     map.GenerateMap();
        // }

        if (GUILayout.Button("Generate Map")){
            map.GenerateMap();
        }
    }
}
