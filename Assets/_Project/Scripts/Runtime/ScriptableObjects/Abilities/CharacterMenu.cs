#region
using UnityEngine;
using UnityEngine.UI;
#endregion

[CreateAssetMenu(fileName = "Character Menu", menuName = "Menus/Character Menu", order = 1002)]
public class CharacterMenu : Ability
{
    public override void Use()
    {
        Debug.Log($"{Name} used!");
    }
}