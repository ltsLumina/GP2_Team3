using UnityEngine;

[CreateAssetMenu(fileName = "Placeholder2 Menu", menuName = "Menus/Placeholder2 Menu", order = 1004)]
public class Placeholder2Menu : Ability
{
    public override void Use()
    {
        Debug.Log($"{Name} used!");

        // this will open a UI window to show your current gear and stats
    }
}
