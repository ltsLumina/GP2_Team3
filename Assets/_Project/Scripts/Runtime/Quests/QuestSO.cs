using UnityEngine;

[CreateAssetMenu(fileName = "new Quest", menuName = "Quests/Quest")]
public class QuestSO : ScriptableObject
{
    [SerializeField] private QuestObjective questObjective;

    public QuestObjective Objective => questObjective;
}
