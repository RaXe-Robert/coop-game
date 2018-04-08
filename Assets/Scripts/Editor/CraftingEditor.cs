using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class CraftingEditor : EditorWindow
{

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

        if (selectedCraftingList != null)
        {
            //TODO: find a way to live update this list
            if (GUILayout.Button("UpdateList"))
            {
                selectedRecipe = null;
                itemList = new ReorderableList(selectedCraftingList.recipes, typeof(CraftingRecipe), true, true, true, true);
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

        if (selectedRecipe != null)
        {
            GUILayout.BeginHorizontal();
            selectedRecipe.result.item = (ScriptableItemData)EditorGUILayout.ObjectField("Result", selectedRecipe.result.item, typeof(ScriptableItemData), false, GUILayout.Width(dataDisplay.width * 0.7f));
            selectedRecipe.result.amount = EditorGUI.IntField(new Rect(dataDisplay.width * 0.7f, dataDisplay.y, dataDisplay.width * 0.3f, EditorGUIUtility.singleLineHeight), "amount", selectedRecipe.result.amount);
            GUILayout.EndHorizontal();
            selectedRecipe.craftingTime = EditorGUI.FloatField(new Rect(0, dataDisplay.y + EditorGUIUtility.singleLineHeight, dataDisplay.width * 0.25f, EditorGUIUtility.singleLineHeight), "Crafting Time", selectedRecipe.craftingTime);
            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            //Required items for this recipe
            var requiredItems = new ReorderableList(selectedRecipe.requiredItems, typeof(CraftingItem), true, true, true, true);
            Debug.Log(requiredItems.list.Count);
            requiredItems.drawHeaderCallback += (Rect rect) => { EditorGUI.LabelField(rect, "Required items"); };

            requiredItems.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                CraftingItem item = ((CraftingItem)requiredItems.list[index]);
                rect.y += 2;
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight), "Required Item", item.item, typeof(ScriptableItemData), false);
                EditorGUI.IntField(new Rect(rect.width * 0.55f, rect.y, rect.width * 0.45f, EditorGUIUtility.singleLineHeight), "Required Amount", item.amount);
            };

            //Index out of bounds ??
            requiredItems?.DoLayoutList();
        }
        GUILayout.EndArea();
    }

    private void ShowRecipe(ReorderableList reorderableList)
    {
        selectedRecipe = (CraftingRecipe)itemList.list[reorderableList.index];
    }
}
