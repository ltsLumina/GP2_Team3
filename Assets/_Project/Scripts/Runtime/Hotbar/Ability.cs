using Lumina.Essentials.Attributes;
using UnityEngine;

/// <summary>
/// Generic ability data class.
/// </summary>
public abstract class Ability : ScriptableObject
{
    [SerializeField] string abilityName;
    [SerializeField] Sprite icon;
    [SerializeField] float cooldown;
    [Tooltip("A positive value will increase the mana amount. Vice versa, a negative value will decrease the mana amount." + "\nE.g. Use 10 to increase the mana by 10. Use -10 to decrease the mana by 10.")]
    [SerializeField] int manaCost;

    [Space(10)]
    [SerializeField] bool cameraShake;
    [Header("Camera Shake")]
    [SerializeField] protected float duration = 0.1f;
    [SerializeField] protected float strength = 0.35f;
    [SerializeField] protected int vibrato = 3;
    [SerializeField] protected float randomness = 45;
    [Space(10)]
    [Header("Misc")]
    [SerializeField, ReadOnly] bool ignore; 
    
    public string Name => !string.IsNullOrEmpty(abilityName) ? abilityName : "Unnamed Ability";
    public Sprite Icon => icon ??= Sprite.Create(new (100, 100), new Rect(0, 0, 100, 100), Vector2.one * 0.5f);
    public float Cooldown => Mathf.Clamp(cooldown, 0.25f, cooldown) * Player[Player.Stats.CooldownReduction];
    public int ManaCost => manaCost;

    public bool IsMenu => GetType().Name.Contains("Menu");
    public bool IsMouse => GetType().Name.Contains("Click");

    Player player;
    protected Player Player
    {
        get
        {
            if (player == null) player = GameManager.Instance.Player;
            return player;
        }
    }

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