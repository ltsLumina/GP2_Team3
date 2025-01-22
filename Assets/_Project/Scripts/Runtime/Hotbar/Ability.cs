using UnityEditor;
using UnityEngine;

/// <summary>
/// Generic ability data class.
/// </summary>
public abstract class Ability : ScriptableObject
{
    [SerializeField] string abilityName;
    [SerializeField] Sprite icon;
    [SerializeField] float cooldown;

    public string Name => !string.IsNullOrEmpty(abilityName) ? abilityName : "Unnamed Ability";
    public Sprite Icon => icon ??= Sprite.Create(new (100, 100), new Rect(0, 0, 100, 100), Vector2.one * 0.5f);
    public float Cooldown => cooldown;

    public bool IsMenu => GetType().Name.Contains("Menu");
    public bool IsMouse => GetType().Name.Contains("Click");
    
    public abstract void Use();

    void Awake()
    {
        string className = nameof(Ability).GetType().Name;
        
        // ensure the class name is suffixed with "Ability" or "Menu"
        if (!className.Contains("DEV"))
        {
            if (!className.EndsWith("Ability") || !className.EndsWith("Menu")) 
                Debug.LogError($"Class name \"{className}\" must contain either \"Ability\" or \"Menu\".");
        }

        if (string.IsNullOrEmpty(abilityName)) abilityName = name;
        if (icon == null) icon = Icon;
        if (cooldown == 0) cooldown = 2.5f;
    }

    public void Reset()
    {
        abilityName = name;
        icon = Icon;
        cooldown = !IsMenu ? 2.5f : 1f;  // Abilities have a 2.5s cooldown, Menus have a 1s cooldown
    }
}

#if UNITY_EDITOR
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
    }
}

public static class AbilityDebugging
{
    [MenuItem("Tools/Lumina/Create DEV Abilities")]
    public static void CreateDEVAbilities()
    {
        // Define the folder path where the assets will be created
        string folderPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
        // convert to relative path
        folderPath = folderPath.Replace(Application.dataPath, "Assets");

        // Ensure the folder exists
        if (!AssetDatabase.IsValidFolder(folderPath)) Debug.LogError("Invalid folder path.");

        for (int i = 1; i <= 4; i++)
        {
            // Create a new instance of the Ability ScriptableObject
            var newAbility = ScriptableObject.CreateInstance($"DEV_Ability_{i}");

            // Create the asset in the specified folder
            AssetDatabase.CreateAsset(newAbility, $"{folderPath}/DEV_Ability_{i}.asset");

            // Save the assets
            AssetDatabase.SaveAssets();

            // Refresh the AssetDatabase to show the new asset in the Project window
            AssetDatabase.Refresh();
        }

        // Save the assets
        AssetDatabase.SaveAssets();

        // Refresh the AssetDatabase to show the new asset in the Project window
        AssetDatabase.Refresh();

        Debug.Log("DEV Abilities created successfully.");
    }
}
#endif
