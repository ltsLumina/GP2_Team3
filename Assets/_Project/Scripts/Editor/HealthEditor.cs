using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Health))]
public class HealthEditor : Editor
{
    SerializedProperty health;
    SerializedProperty maxHealth;

    void Awake()
    {
        health = serializedObject.FindProperty("health");
        maxHealth = serializedObject.FindProperty("maxHealth");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // slider for health
        EditorGUILayout.Slider(health, 0, maxHealth.floatValue, new GUIContent("Health"));
        EditorGUILayout.PropertyField(maxHealth);

        if (serializedObject.ApplyModifiedProperties())
        {
            var healthComponent = (Health)target;
            healthComponent.Material.SetFloat("_ResourceAmount", health.floatValue / maxHealth.floatValue);
            
        }
    }
}
