using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WallGenerator))]
public class WallGenEditor : Editor {
    
    public override void OnInspectorGUI() {
        
        WallGenerator wallGenerator = target as WallGenerator;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Wall")){
            wallGenerator.GenerateWall();
        }
    }
    
}
