using UnityEditor;
using UnityEngine;

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