using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.XR;

[CustomEditor(typeof(Hotbar))]
public class HotbarEditor : Editor
{
    SerializedProperty slotsInTotal;
    SerializedProperty keyCodesForSlots;

    SerializedProperty arrowTF;
    SerializedProperty addArrowPos;
    SerializedProperty R_Hand;
    Hotbar hotbar;

    void OnEnable()
    {
        hotbar = target as Hotbar; //타겟팅

        slotsInTotal = serializedObject.FindProperty("slotsInTotal"); //프로퍼티 직렬화 객체 참조
        keyCodesForSlots = serializedObject.FindProperty("keyCodesForSlots");
        arrowTF = serializedObject.FindProperty("arrowTF");
        addArrowPos = serializedObject.FindProperty("addArrowPos");
        R_Hand = serializedObject.FindProperty("R_Hand");

        slotsInTotal.intValue = hotbar.getSlotsInTotal();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.BeginVertical("Box");

        slotsInTotal.intValue = hotbar.getSlotsInTotal();
        for (int i = 0; i < slotsInTotal.intValue; i++)
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(keyCodesForSlots.GetArrayElementAtIndex(i), new GUIContent("Slot " + (i + 1)));
            GUILayout.EndVertical();
        }
        serializedObject.ApplyModifiedProperties();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("Box");
        hotbar.arrowTF = (RectTransform)EditorGUILayout.ObjectField("arrowTF", hotbar.arrowTF, typeof(RectTransform),true);
        hotbar.addArrowPos = EditorGUILayout.IntField("addArrowPos", hotbar.addArrowPos);
        hotbar.R_Hand = (GameObject)EditorGUILayout.ObjectField("R_Hand", hotbar.R_Hand, typeof(GameObject), true);

        GUILayout.EndVertical();

    }
}
