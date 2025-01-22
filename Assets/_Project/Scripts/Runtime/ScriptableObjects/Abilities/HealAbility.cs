#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Heal", menuName = "Abilities/Heal", order = 106)]
public class HealAbility : Ability
{
    public override void Use()
    {
        Debug.Log($"{Name} used!");

        var health = FindAnyObjectByType<Health>();
        health.Heal(10);
    }
}