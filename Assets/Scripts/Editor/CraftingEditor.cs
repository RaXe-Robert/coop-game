using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class CraftingEditor : EditorWindow {

    private CraftingList selectedCraftingList;
    public List<CraftingRecipe> list;

    private Rect listDisplay;
    private Rect dataDisplay;
    private ReorderableList itemList;

    [MenuItem("Window/CraftingEditor")]
    public static void ShowWindow()
    {
        GetWindow<CraftingEditor>("Crafting Editor");
    }

    private void OnGUI()
    {
        DrawListDisplay();
        DrawDataDisplay();
    }

    private void DrawListDisplay()
    {
        listDisplay = new Rect(0, 0, position.width * 0.3f, position.height);
        GUILayout.BeginArea(listDisplay);

        //Input field for a crafting list
        selectedCraftingList = (CraftingList)EditorGUILayout.ObjectField(selectedCraftingList, typeof(CraftingList), false);

        if(selectedCraftingList != null)
        {
            //TODO: find a way to live update this list
            if (GUILayout.Button("UpdateList"))
            {
                itemList = new ReorderableList(selectedCraftingList.recipes, typeof(CraftingRecipe),true,true,true,true);
            }
            itemList?.DoLayoutList();
        }
        GUILayout.EndArea();
    }

    private void DrawDataDisplay()
    {
        dataDisplay = new Rect(position.width * 0.3f, 0, position.width * 0.7f, position.height);
        GUILayout.BeginArea(dataDisplay);
        GUILayout.Label("crafting recipe options");
        GUILayout.EndArea();
    }
}
