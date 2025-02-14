using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Health))]
public class HealthEditor : Editor
{
    Health health;
    SerializedProperty currentHealth;
    SerializedProperty maxHealth;

    void Awake()
    {
        health = (Health)target;
        currentHealth = serializedObject.FindProperty("health");
        maxHealth = serializedObject.FindProperty("maxHealth");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // slider for health
        EditorGUILayout.Slider(currentHealth, 0, maxHealth.floatValue, new GUIContent("Health"));
        EditorGUILayout.PropertyField(maxHealth);

        if (GUILayout.Button("Revive"))
        {
            health.Revive();
        }

        if (serializedObject.ApplyModifiedProperties())
        {
            var healthComponent = (Health)target;
            healthComponent.Material.SetFloat("_ResourceAmount", currentHealth.floatValue / maxHealth.floatValue);
            
        }
    }
}
