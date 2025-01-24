using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class QuestObjectiveManager : MonoBehaviour
{
    private static QuestObjectiveManager instance;
    public static QuestObjectiveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<QuestObjectiveManager>();
            }
            return instance;
        }
    }

    [SerializeField] private List<QuestSO> quests; 

    private QuestObjective currentObjective;
    private HashSet<QuestItemSO> collectedItems = new();

    private void OnEnable()
    {
        QuestItemGiver.OnQuestItemGiven += AddObjectiveItem;
    }

    private void OnDisable()
    {
        QuestItemGiver.OnQuestItemGiven -= AddObjectiveItem;

        foreach (QuestSO quest in quests)
        {
            quest.Objective.ResetObjective();
        }
    }

    private void Start()
    {
        currentObjective = quests[0].Objective;
    }

    private void AddObjectiveItem(QuestItemSO objective)
    {
        collectedItems.Add(objective);
        currentObjective.CheckStatus();
    }

    public bool HasItem(QuestItemSO objective)
    {
        return collectedItems.Contains(objective);
    }

}