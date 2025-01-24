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
        string className = GetType().Name;

        // ensure the class name is suffixed with "Ability" or "Menu"
        if (!className.Contains("DEV") && !className.EndsWith("Ability") && !className.EndsWith("Menu")) 
            Debug.LogError($"Class name \"{className}\" must contain either \"Ability\" or \"Menu\".");

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