using JetBrains.Annotations;
using Lumina.Essentials.Attributes;
using UnityEngine;

public class ExperienceBar : MonoBehaviour
{
    [Header("Experience")]
    [ReadOnly, UsedImplicitly] 
    [SerializeField] int currentXP;
    [ReadOnly, UsedImplicitly]
    [SerializeField] int level;
    
    [Header("Debug"), Tooltip("This will automatically level up the player 10 times on start.")]
    [SerializeField] bool debugMode;

    public void Reset()
    {
        Experience.ResetAll();
    }

    void Start()
    {
        Reset();
    }

    void OnEnable() => Experience.OnLevelUp += OnLevelUp;
    void OnDisable() => Experience.OnLevelUp -= OnLevelUp;

#if UNITY_EDITOR
    void OnDestroy()
    {
        Experience.ResetLevel(); // required due to enter playmode options not resetting level on exit playmode
    }
#endif

    void OnLevelUp()
    {
        Debug.Log("Level up!" + "\n" + "Level: " + Experience.Level);
    }
}
