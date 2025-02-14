using UnityEngine;

/// <summary>
/// This is the potion ability that will heal the player.
/// </summary>
[CreateAssetMenu(fileName = "Dash", menuName = "Abilities/Dash", order = 111)]
public class DashAbility : Ability
{
    public override void Use()
    {
        Debug.Log($"{Name} used!");
    }
}
