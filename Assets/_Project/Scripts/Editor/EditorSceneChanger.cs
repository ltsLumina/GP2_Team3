using UnityEditor.SceneManagement;
using UnityEditor;
using System;
using System.Linq;
using UnityEngine;

[InitializeOnLoad]
public class SceneChanger
{
    static SceneChanger()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            int currentSceneIndex = EditorSceneManager.GetActiveScene().buildIndex;
            var validSceneIndexes = (int[])Enum.GetValues(typeof(SceneIndexes));
            int managerSceneIndex = (int)SceneIndexes.MANAGER;

            if (!validSceneIndexes.Contains(managerSceneIndex))
            {
                Debug.LogError($"{nameof(SceneIndexes.MANAGER)} scene is not included in the build settings.");
                return;
            }

            if (validSceneIndexes.Contains(currentSceneIndex) && currentSceneIndex != managerSceneIndex)
            {
                Debug.Log($"Switching from scene index {currentSceneIndex} to {nameof(SceneIndexes.MANAGER)} scene.");
                EditorSceneManager.LoadScene(managerSceneIndex);
            }
        }
    }

}
