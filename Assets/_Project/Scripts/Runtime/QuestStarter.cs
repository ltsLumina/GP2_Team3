using UnityEngine;

public class QuestStarter : MonoBehaviour
{
    [SerializeField] private QuestSO quest;

    public void StartQuest()
    {
        if (!quest) return;

        if(!quest.Objective.IsComplete && !quest.Objective.IsStarted)
            QuestObjectiveManager.Instance.StartQuest(quest);
    }
}
