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
        Damage,
        AttackSpeed,
        CooldownReduction,
        MovementSpeed
    }
    
    Health health;
    readonly Dictionary<Stats, int> playerStats = new();

    #region unused
#pragma warning disable 0067
    /// <summary>
    ///     Unused. Refer to <see cref="Health" /> instead.
    /// </summary>
    public event Action OnDeath;
#pragma warning restore 0414
    #endregion
    
    public float CurrentHealth
    {
        get => health.CurrentHealth;
        set => health.CurrentHealth = value;
    }
    public float MaxHealth
    {
        get => health.MaxHealth;
        set => health.MaxHealth = value;
    }
    
    public IReadOnlyDictionary<Stats, int> PlayerStats => playerStats; // changelog: changed from get-method to property (this comment can be removed after reading)
    
    public void TakeDamage(int damage) => health.TakeDamage(damage);

    void Start()
    {
        health = FindAnyObjectByType<Health>();

        // set default stat values, all stats are in percentages (%)
        // 100 means 100% of the damage dealt. This allows for modifiers to decrease 1 stat and increase another
        // 101% means more than default 100% damage... I think you get what I mean at this point
        playerStats.Add(Stats.Health, 100);
        playerStats.Add(Stats.Damage, 100);
        playerStats.Add(Stats.AttackSpeed, 100);
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
