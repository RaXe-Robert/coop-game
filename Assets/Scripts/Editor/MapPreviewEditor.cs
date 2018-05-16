using UnityEditor;
using UnityEngine;

using Assets.Scripts.Map_Generation;

[CustomEditor(typeof(MapPreview))]
public class MapPreviewEditor : Editor {

    public override void OnInspectorGUI()
    {
        MapPreview mapPreview = (MapPreview)target;

        if (DrawDefaultInspector())
        {
            if (mapPreview.autoUpdate)
                mapPreview.DrawMapInEditor();
        }

        
        if (GUILayout.Button("Generate Map"))
            mapPreview.DrawMapInEditor();
    }
}

