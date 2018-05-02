using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapPreview))]
public class MapPreviewEditor : Editor {

    public override void OnInspectorGUI()
    {
        MapPreview mapPreview = (MapPreview)target;

        if (DrawDefaultInspector())
        { }
            //mapGenerator.GenetateMap();

        if (GUILayout.Button("Generate Map"))
            mapPreview.DrawMapInEditor();
    }
}

