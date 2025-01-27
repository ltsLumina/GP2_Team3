using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ability), true)]
public class AbilityEditor : Editor
{
    Ability ability;
    
    public override void OnInspectorGUI()
    {
        ability = (Ability) target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("abilityName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown"));
        
        EditorGUILayout.Space();

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Use Ability", GUILayout.Height(35), GUILayout.Width(100)))
            {
                if (!Application.isPlaying)
                {
                    Debug.LogWarning("Cannot use ability in edit mode.");
                    return;
                }
                
                ability.Use();
            }

            if (GUILayout.Button("Reset", GUILayout.Height(35), GUILayout.Width(100)))
            {
                if (!Application.isPlaying)
                {
                    Debug.LogWarning("Cannot use ability in edit mode.");
                    return;
                }
                
                ability.Reset();
            }
            GUILayout.FlexibleSpace();
        }

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(10);
        GUILayout.Label("", GUI.skin.horizontalSlider);
        GUILayout.Space(20);
        
        base.OnInspectorGUI();
    }
}