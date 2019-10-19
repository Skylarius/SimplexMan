using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ElectricWallGenerator))]
public class ElectricWallEditor : Editor {
    
    public override void OnInspectorGUI() {
        
        ElectricWallGenerator wallGenerator = target as ElectricWallGenerator;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Wall")){
            wallGenerator.GenerateWall();
        }
    }
    
}
