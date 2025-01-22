#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "DEV_Ability_2", menuName = "Abilities/DEV_Ability_2", order = 101)]
public class DEV_Ability_2 : Ability
{
    public override void Use() => Debug.Log($"{Name} used!");
}
