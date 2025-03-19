using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     The IDamageable implementation on the player is to allow easier access to the health component.
///     <para>
///         This is to allow us to call e.g. Player.TakeDamage(10) instead of finding the health component and
///         calling it from there.
///     </para>
/// </summary>
[SelectionBase]
public class Player : MonoBehaviour, IDamageable
{
    public enum Stats
    {
        Health,
        Mana,
        Damage,
        CooldownReduction,
        MovementSpeed
    }

    #region unused
#pragma warning disable 0067
    /// <summary>
    ///     Unused. Refer to <see cref="Health" /> instead.
    /// </summary>
    public event Action OnDeath;
#pragma warning restore 0414
    #endregion

    public Health Health { get; private set; }
    public Mana Mana { get; private set; }
    public DashController DashController { get; private set; }

    public float CurrentHealth
    {
        get => Health.CurrentHealth;
        set => Health.CurrentHealth = value;
    }
    public float MaxHealth
    {
        get => Health.MaxHealth;
        set => Health.MaxHealth = value;
    }

    public bool IsDead => CurrentHealth <= 0;
    
    public bool HasWeapon { get; set; }
    
    readonly Dictionary<Stats, int> playerStats = new ();
    public Animator Animator { get; private set; }

    /// <summary>
    ///    Indexer for the player's stats. This allows us to access the player's stats easier.
    /// <example> player[Stats.Health] returns the stat as a percentage. </example>
    /// </summary>
    /// <param name="stat"></param>
    public float this[Stats stat] => playerStats[stat] / 100f;
    
    public IReadOnlyDictionary<Stats, int> PlayerStats => playerStats; // changelog: changed from get-method to property (this comment can be removed after reading)

    public void TakeDamage(int damage) => Health.TakeDamage(damage);

    void Awake()
    {
        DashController = GetComponent<DashController>();
        Animator = GetComponent<Animator>();
    }

    void Start()
    {
        Health = FindAnyObjectByType<Health>();
        Mana = FindAnyObjectByType<Mana>();

        // set default stat values, all stats are in percentages (%)
        // 100 means 100% of the damage dealt. This allows for modifiers to decrease 1 stat and increase another
        // 101% means more than default 100% damage... I think you get what I mean at this point
        playerStats.Add(Stats.Health, 100);
        playerStats.Add(Stats.Mana, 100);
        playerStats.Add(Stats.Damage, 100);
        playerStats.Add(Stats.CooldownReduction, 100);
        playerStats.Add(Stats.MovementSpeed, 100);
    }

    /// <summary>
    ///    Increase a stat by a certain percentage point value.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="amount"> The percentage points to increase the stat by. </param>
    public void IncreaseStat(Stats stat, int amount) => playerStats[stat] += amount;
    
    /// <summary>
    ///   Decrease a stat by a certain percentage point value.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="amount"> The percentage points to decrease the stat by. </param>
    public void DecreaseStat(Stats stat, int amount) => playerStats[stat] -= amount;
    
    public int GetStatValue(Stats stat) => playerStats[stat];
}
