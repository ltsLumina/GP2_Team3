using UnityEngine;

[CreateAssetMenu(fileName = "new Item Quest", menuName = "Quests/Item Quest")]
public class ItemQuestSO : QuestSO
{
    [SerializeField] private ItemQuestObjective questObjective;
    public override QuestObjective Objective => questObjective;

    public void SetRequiredItems(ItemQuestData[] requiredItems)
    {
        questObjective.SetRequiredItems(requiredItems);
    }
}
