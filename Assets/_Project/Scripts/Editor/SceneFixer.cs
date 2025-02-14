#region
using UnityEditor;
using UnityEngine;
#endregion

public static class SceneFixer
{
    [MenuItem("Tools/Round Positions")]
    static void RoundPositions()
    {
        foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)) // warning when using FindObjectsOfType<>() as it is deprecated
        {
            Undo.RecordObject(obj.transform, "Round Positions");

            var roundedPosition = new Vector3
            (
                Mathf.Round(obj.transform.localPosition.x * 100) / 100,
                Mathf.Round(obj.transform.localPosition.y * 100) / 100,
                Mathf.Round(obj.transform.localPosition.z * 100) / 100
            );

            obj.transform.localPosition = roundedPosition;
        }
    }
}
