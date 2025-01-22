using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField, Range(0,100)] float health = 100f;
    [SerializeField] float maxHealth = 100f;

    public Material Material => GetComponent<Image>().material;
    
    void Awake() => Material.SetFloat("_ResourceAmount", maxHealth);

    public float CurrentHealth
    {
        get
        {
            Material.SetFloat("_ResourceAmount", Mathf.Clamp01(health / maxHealth));
            return Mathf.Clamp(health, 0, maxHealth);
        }
        set
        {
            if (value <= 0)
            {
                health = 0;
                Debug.LogWarning("Dead!");
            }
            else
            {
                health = value;
                Debug.Log($"Set health to {value}!");
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Took {damage} damage! ({CurrentHealth} health remaining)");
    }

    public void Heal(int amount)
    {
        health += amount;
        Debug.Log($"Healed {amount}! ({CurrentHealth} health remaining)");
    }
}

// custom editor
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
