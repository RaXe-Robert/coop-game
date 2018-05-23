﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class CraftingEditor : EditorWindow
{
    private CraftingList selectedCraftingList;
    private CraftingRecipe selectedRecipe;

    private Rect recipeListDisplay;
    private Rect recipeDataDisplay;
    private ReorderableList recipeList;
    private ReorderableList requiredItems;

    [MenuItem("Window/CraftingEditor")]
    public static void ShowWindow()
    {
        var x = GetWindow<CraftingEditor>("Crafting Editor");
        x.minSize = new Vector2(1000, 400);
    }

    private void OnGUI()
    {
        DrawRecipeListDisplay();
        DrawRecipeDataDisplay();
    }

    private void DrawRecipeListDisplay()
    {
        recipeListDisplay = new Rect(5, 5, position.width * 0.30f, position.height);
        GUILayout.BeginArea(recipeListDisplay);

        //Input field for a list of recipes
        selectedCraftingList = (CraftingList)EditorGUILayout.ObjectField(selectedCraftingList, typeof(CraftingList), false);

        if (selectedCraftingList != null)
        {
            //TODO: find a way to live update this list
            if (GUILayout.Button("UpdateList"))
            {
                DrawRecipesList();
            }
            recipeList?.DoLayoutList();
        }
        GUILayout.EndArea();
    }

    private void DrawRecipeDataDisplay()
    {
        recipeDataDisplay = new Rect(position.width * 0.315f, 5, position.width * 0.65f, position.height);
        GUILayout.BeginArea(recipeDataDisplay);

        if (selectedRecipe != null)
        {
            DrawRecipeDataFields();
            EditorUtility.SetDirty(selectedCraftingList);
        }
        GUILayout.EndArea();
    }

    private void DrawRecipesList()
    {
        selectedRecipe = null;
        recipeList = new ReorderableList(selectedCraftingList.recipes, typeof(CraftingRecipe), true, true, true, true);
        recipeList.drawHeaderCallback += (Rect rect) => { EditorGUI.LabelField(rect, "Recipes"); };

        recipeList.onSelectCallback += SelectRecipe;
        recipeList.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            ScriptableItemData item = ((CraftingRecipe)recipeList.list[index]).result?.item;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item ? item.name : "New recipe");
        };
    }

    private void DrawRecipeDataFields()
    {
        //Crafting result + amount
        selectedRecipe.result.item = (ScriptableItemData)EditorGUILayout.ObjectField("Result", selectedRecipe.result.item ?? null, typeof(ScriptableItemData), false, GUILayout.Width(recipeDataDisplay.width * 0.5f));
        selectedRecipe.result.amount = EditorGUI.IntField(new Rect(recipeDataDisplay.width * 0.52f, recipeDataDisplay.y - 5, recipeDataDisplay.width * 0.25f, EditorGUIUtility.singleLineHeight), "Amount", selectedRecipe.result != null ? selectedRecipe.result.amount : 0);

        //Crafting Time
        selectedRecipe.craftingTime = EditorGUI.FloatField(new Rect(0, recipeDataDisplay.y + EditorGUIUtility.singleLineHeight, recipeDataDisplay.width * 0.45f, EditorGUIUtility.singleLineHeight), "Crafting Time", selectedRecipe.craftingTime);

        GUILayout.Space(20);        

        requiredItems?.DoLayoutList();
    }

    private void SelectRecipe(ReorderableList reorderableList)
    {
        selectedRecipe = (CraftingRecipe)recipeList.list[reorderableList.index];

        if (selectedRecipe.requiredItems == null)
            selectedRecipe.requiredItems = new List<CraftingItem>();

        if (selectedRecipe.result == null)
            selectedRecipe.result = new CraftingItem();

        CreateRequiredItemList();
    }

    private void CreateRequiredItemList()
    {
        requiredItems = new ReorderableList(selectedRecipe.requiredItems, typeof(CraftingItem), true, true, true, true);
        requiredItems.drawHeaderCallback += (Rect rect) => { EditorGUI.LabelField(rect, "Required items"); };

        requiredItems.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            CraftingItem requiredItem = (CraftingItem)requiredItems.list[index];
            requiredItem.item = (ScriptableItemData)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight), "Required Item", requiredItem.item, typeof(ScriptableItemData), false);
            requiredItem.amount = EditorGUI.IntField(new Rect(rect.width * 0.55f, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight), "Required Amount", requiredItem.amount);
        };
    }

    private void AddNewRecipe(ReorderableList reorderableList)
    {
        selectedRecipe = (CraftingRecipe)recipeList.list[reorderableList.index];
        selectedRecipe.requiredItems = new List<CraftingItem>();
        selectedRecipe.result = new CraftingItem();
    }
}
