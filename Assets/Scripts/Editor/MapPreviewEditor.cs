using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapPreview))]
public class MapPreviewEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapPreview mapPreview = (MapPreview)target;
        
        if (GUILayout.Button("Generate Map"))
            mapPreview.DrawMapInEditor();
    }
}

