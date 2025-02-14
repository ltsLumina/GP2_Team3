using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Mana))]
public class ManaEditor : Editor
{
    Mana mana;
    SerializedProperty currentMana;
    SerializedProperty maxMana;

    void Awake()
    {
        mana = (Mana) target;
        currentMana = serializedObject.FindProperty("mana");
        maxMana = serializedObject.FindProperty("maxMana");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // slider for health
        EditorGUILayout.Slider(currentMana, 0, maxMana.floatValue, new GUIContent("Mana"));
        EditorGUILayout.PropertyField(maxMana);

        if (GUILayout.Button("Refill")) currentMana.floatValue = maxMana.floatValue;

        if (serializedObject.ApplyModifiedProperties())
        {
            var manaComponent = (Mana) target;
            manaComponent.Material.SetFloat("_ResourceAmount", currentMana.floatValue / maxMana.floatValue);

        }
    }
}
