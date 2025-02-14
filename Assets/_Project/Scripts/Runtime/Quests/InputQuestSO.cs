using UnityEngine;

[CreateAssetMenu(fileName = "new Input Quest", menuName = "Quests/Input Quest")]
public class InputQuestSO : QuestSO
{
    [SerializeField] private InputQuestObjective questObjective;
    public override QuestObjective Objective => questObjective;
}
