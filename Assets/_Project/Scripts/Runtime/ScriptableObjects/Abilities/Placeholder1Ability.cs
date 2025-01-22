using UnityEngine;

[CreateAssetMenu(fileName = "Placeholder1 Menu", menuName = "Menus/Placeholder1 Menu", order = 1001)]
public class Placeholder1Menu : Ability
{
    public override void Use()
    {
        Debug.Log($"{Name} used!");

        // this will open a UI window to show your current gear and stats
    }
}
