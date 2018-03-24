using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;

        if (DrawDefaultInspector() && mapGenerator.autoUpdate)
            mapGenerator.GenetateMap();

        if (GUILayout.Button("Generate Map"))
        {
            mapGenerator.GenetateMap();
        }
    }
}
