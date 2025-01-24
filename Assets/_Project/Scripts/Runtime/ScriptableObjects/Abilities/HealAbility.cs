#region
using UnityEngine;
#endregion

/// <summary>
/// This is the potion ability that will heal the player.
/// </summary>
[CreateAssetMenu(fileName = "Heal", menuName = "Abilities/Heal", order = 106)]
public class HealAbility : Ability
{
    [SerializeField] int healAmount = 10;
    
    public override void Use()
    {
        Debug.Log($"{Name} used!");

        var health = FindAnyObjectByType<Health>();
        health.Heal(healAmount);
    }
}