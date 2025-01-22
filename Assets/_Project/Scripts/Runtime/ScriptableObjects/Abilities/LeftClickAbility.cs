#region
using UnityEngine;
using UnityEngine.EventSystems;
#endregion

[CreateAssetMenu(fileName = "Left Click", menuName = "Abilities/Left Click Ability", order = 104)]
public class LeftClickAbility : Ability
{ 
    /// <summary>
    /// Left and Right Click abilities need additional logic to work properly.
    /// <para>A check must be performed to determine if the cursor is over a UI element.</para>
    /// <para>If the cursor is over a UI element, the ability should not be used.</para>
    /// </summary>
    public override void Use()
    {
        Debug.Log($"{Name} used!");
    }
}