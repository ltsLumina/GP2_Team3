using UnityEngine;

[CreateAssetMenu(fileName = "Achievements Menu", menuName = "Menus/Achievements Menu", order = 1003)]
public class AchievementsMenu : Ability
{
    public override void Use()
    {
        Debug.Log($"{Name} used!");

        // this will open a UI window to show your current gear and stats
    }
}
