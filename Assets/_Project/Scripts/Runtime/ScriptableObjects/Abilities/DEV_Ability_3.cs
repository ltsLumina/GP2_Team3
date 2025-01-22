using UnityEngine;

[CreateAssetMenu(fileName = "DEV_Ability_3", menuName = "Abilities/DEV_Ability_3", order = 102)]
public class DEV_Ability_3 : Ability
{
    public override void Use() => Debug.Log($"{Name} used!");
}
