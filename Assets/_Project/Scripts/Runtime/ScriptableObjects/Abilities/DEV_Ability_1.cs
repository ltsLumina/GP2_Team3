#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "DEV_Ability_1", menuName = "Abilities/DEV_Ability_1", order = 100)]
public class DEV_Ability_1 : Ability
{
    public override void Use()
    {
        Debug.Log($"{Name} used!");

        var health = FindAnyObjectByType<Health>();
        health.TakeDamage(10);
    }
}