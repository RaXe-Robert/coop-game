using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(CraftingList))]
public class EditorCraftingList : Editor
{
    private ReorderableList list;

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject,
            serializedObject.FindProperty(nameof(CraftingList.recipes)),
            true, true, true, true);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(CraftingItem))]
public class EditorCraftingItem : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var resultAmountLabelSpace = new Rect(position.x, position.y, 55, position.height);
        var resultAmountSpace = new Rect(resultAmountLabelSpace.x + resultAmountLabelSpace.width, position.y, 90, position.height);
        var craftingTimeLabelSpace = new Rect(resultAmountSpace.x + resultAmountSpace.width + 10, position.y, 40, position.height);
        var craftingTimeSpace = new Rect(craftingTimeLabelSpace.x + craftingTimeLabelSpace.width, position.y, 90, position.height);

        EditorGUI.LabelField(resultAmountLabelSpace, new GUIContent("Amount:"));
        EditorGUI.PropertyField(resultAmountSpace, property.FindPropertyRelative(nameof(CraftingItem.amount)), GUIContent.none);
        EditorGUI.LabelField(craftingTimeLabelSpace, new GUIContent("Item:"));
        EditorGUI.PropertyField(craftingTimeSpace, property.FindPropertyRelative(nameof(CraftingItem.item)), GUIContent.none);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        return base.GetPropertyHeight(property, label);
    }
}