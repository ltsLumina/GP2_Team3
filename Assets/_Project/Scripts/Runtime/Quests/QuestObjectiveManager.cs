using System;
using System.Collections.Generic;
using System.Linq;
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

    private static IQuestItemGiver[] _questItemGivers;
    public static IQuestItemGiver[] QuestItemGivers
    {
        get
        {
            if (_questItemGivers == null || _questItemGivers.Length <= 0)
            {
                _questItemGivers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IQuestItemGiver>().ToArray();
            }
            return _questItemGivers;
        }
    }

    [SerializeField] private List<QuestSO> quests;

    private QuestSO currentQuest;
    private HashSet<QuestItemSO> collectedItems = new();

    public event Action<QuestObjective> OnObjectiveUpdated;

    private void OnEnable()
    {
        _questItemGivers = null;

        foreach (var itemGiver in QuestItemGivers)
        {
            itemGiver.OnQuestItemGiven += AddObjectiveItem;
        }
    }

    private void OnDisable()
    {
        foreach (var itemGiver in QuestItemGivers)
        {
            itemGiver.OnQuestItemGiven -= AddObjectiveItem;
        }

        foreach (QuestSO quest in quests)
        {
            quest.Objective.ResetObjective();
        }
    }

    private void Start()
    {
        currentQuest = quests[0];
        OnObjectiveUpdated?.Invoke(currentQuest.Objective);
    }

    private void AddObjectiveItem(QuestItemSO objective)
    {
        collectedItems.Add(objective);
        currentQuest.Objective.CheckStatus();

        if (currentQuest.Objective.IsComplete)
        {
            int currentIndex = quests.IndexOf(currentQuest);

            if (currentIndex + 1 < quests.Count)
            {
                currentQuest = quests[currentIndex + 1];
            }
        }

        OnObjectiveUpdated?.Invoke(currentQuest.Objective);
    }


    public bool HasItem(QuestItemSO objective)
    {
        return collectedItems.Contains(objective);
    }
}