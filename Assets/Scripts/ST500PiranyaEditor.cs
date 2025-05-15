

#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
[UnityEditor.CustomEditor(typeof(ST500Piranya))]
public class ST500PiranyaEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ST500Piranya piranya = (ST500Piranya)target;
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Текущее состояние:", 
            piranya.IsDeviceActive ? "ВКЛ" : "ВЫКЛ", 
            piranya.IsDeviceActive ? EditorStyles.boldLabel : EditorStyles.label);

        GUI.backgroundColor = piranya.IsDeviceActive ? Color.green : Color.red;
        if (GUILayout.Button("ST-500: " + (piranya.IsDeviceActive ? "ВКЛ" : "ВЫКЛ"), GUILayout.Height(30)))
        {
            piranya.ToggleDevice();
            EditorUtility.SetDirty(target);
        }
        GUI.backgroundColor = Color.white;
    }
}
#endif