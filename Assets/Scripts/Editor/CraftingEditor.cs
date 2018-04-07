using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class CraftingEditor : EditorWindow {

    private CraftingList selectedCraftingList;
    private CraftingRecipe selectedRecipe;

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
                itemList.drawHeaderCallback += (Rect rect) => { EditorGUI.LabelField(rect, "Recipes"); }; 

                itemList.onSelectCallback += ShowRecipe;
                itemList.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    ScriptableItemData item = ((CraftingRecipe)itemList.list[index]).result?.item;
                    rect.y += 2;
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item ? item.name : "New recipe");
                };
            }
            itemList?.DoLayoutList();
        }
        GUILayout.EndArea();
    }

    private void DrawDataDisplay()
    {
        dataDisplay = new Rect(position.width * 0.35f, 0, position.width * 0.65f, position.height);
        GUILayout.BeginArea(dataDisplay);
        if(selectedRecipe != null)
        {
            GUILayout.BeginHorizontal();
            selectedRecipe.result.item = (ScriptableItemData)EditorGUILayout.ObjectField("Result", selectedRecipe.result.item, typeof(ScriptableItemData), false, GUILayout.Width(dataDisplay.width * 0.7f));
            selectedRecipe.result.amount = EditorGUI.IntField(new Rect(dataDisplay.width *0.7f, dataDisplay.y, dataDisplay.width * 0.3f, EditorGUIUtility.singleLineHeight) , "amount", selectedRecipe.result.amount);
        }
        GUILayout.EndArea();
    }

    private void ShowRecipe(ReorderableList reorderableList)
    {
        selectedRecipe = (CraftingRecipe)itemList.list[reorderableList.index];
    }
}
