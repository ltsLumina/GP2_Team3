using UnityEngine;

[CreateAssetMenu(fileName = "DEV_Ability_4", menuName = "Abilities/DEV_Ability_4", order = 103)]
public class DEV_Ability_4 : Ability
{
    public override void Use() => Debug.Log($"{Name} used!");
}