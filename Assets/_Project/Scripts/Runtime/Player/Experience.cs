using JetBrains.Annotations;

public static class Experience
{
    public delegate void LevelUp();
    
    public static int Level { get; private set; } = 1;
    
    public static event LevelUp OnLevelUp;

    public static void GainLevel()
    {
        Level++;
        OnLevelUp?.Invoke();
    }

    public static void ResetLevel() => Level = 1;

    public static void ResetAll()
    {
        ResetLevel();
    }
    
#if UNITY_EDITOR
    [UsedImplicitly]
    public static void EDITOR_GainLevel()
    {
        GainLevel();
    }
#endif
}