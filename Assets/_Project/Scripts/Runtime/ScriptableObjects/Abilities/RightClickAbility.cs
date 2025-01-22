#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Right Click", menuName = "Abilities/Right Click Ability", order = 105)]
public class RightClickAbility : Ability
{
    public override void Use()
    {
        Debug.Log($"{Name} used!");
    }
}