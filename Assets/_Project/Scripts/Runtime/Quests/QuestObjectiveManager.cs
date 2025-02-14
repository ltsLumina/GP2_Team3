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

    private IQuestItemGiver[] _questItemGivers;
    public IQuestItemGiver[] QuestItemGivers
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

    [Header("Quests")]
    [SerializeField] private QuestSO startQuest;
    [SerializeField] private List<QuestSO> quests;

    private QuestSO currentQuest;
    private Dictionary<string, int> collectedItems = new();

    public QuestSO CurrentQuest => currentQuest;

    public event Action<QuestObjective> OnObjectiveUpdated;
    public event Action<QuestSO> OnQuestStarted;
    public event Action<QuestSO> OnQuestCompleted;

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
            if (quest != null)
                quest.Objective.ResetObjective();
            else
                Debug.LogError("Quest is null!", this);
        }
    }

    private void Start()
    {
        OnObjectiveUpdated?.Invoke(null);
        StartQuest(startQuest);
    }

    private void AddObjectiveItem(QuestItemSO objective)
    {
        if (!collectedItems.ContainsKey(objective.ItemID))
            collectedItems.Add(objective.ItemID, 1);
        else
            collectedItems[objective.ItemID]++;

        if (currentQuest == null)
            return;

        currentQuest.Objective.CheckStatus();
        OnObjectiveUpdated?.Invoke(currentQuest.Objective);

        foreach(var quest in quests)
        {
            if (quest.Objective == currentQuest.Objective)
                continue;
            quest.Objective.CheckStatus();
        }
    }

    public void StartQuest(QuestSO quest)
    {
        if (quest == null)
            return;

        if(quest.PreviousQuest != null && !quest.PreviousQuest.Objective.IsComplete)
        {
            Debug.LogWarning("Cannot start quest. Previous quest not completed.");
            return;
        }

        if (quest == null)
        {
            if (currentQuest != null)
            {
                currentQuest = null;
                OnObjectiveUpdated?.Invoke(null);
            }
            return;
        }

        if (currentQuest != null && (!currentQuest.Objective.IsComplete || quest.Objective.IsStarted))
            return;

        currentQuest = quest;
        currentQuest.Objective.StartObjective();
        OnObjectiveUpdated?.Invoke(currentQuest.Objective);

        currentQuest.Objective.CheckStatus();
        OnQuestStarted?.Invoke(currentQuest);
    }


    public bool EndQuest(QuestSO quest)
    {
        if (currentQuest != quest)
            return false;
        if (!currentQuest.Objective.IsComplete)
            return false;

        OnQuestCompleted?.Invoke(quest);
        StartQuest(currentQuest.NextQuest);

        Experience.GainLevel();
        return true;
    }

    public bool EndQuest(QuestObjective quest)
    {
        if (currentQuest.Objective != quest)
            return false;
        if (!currentQuest.Objective.IsComplete)
            return false;

        OnQuestCompleted?.Invoke(currentQuest);
        StartQuest(currentQuest.NextQuest);

        Experience.GainLevel();
        return true;
    }

    public bool HasItem(QuestItemSO objective, out int itemCount)
    {
        if (collectedItems.ContainsKey(objective.ItemID))
        {
            itemCount = collectedItems[objective.ItemID];
            return true;
        }
        itemCount = 0;
        return false;
    }

    public void UpdateCurrentQuest()
    {
        currentQuest.Objective.CheckStatus();
        OnObjectiveUpdated?.Invoke(currentQuest.Objective);
    }

//#if UNITY_EDITOR

//    private void OnValidate()
//    {
//        if (quests == null || quests.Count <= 0)
//            return;

//        if (UnityEngine.Application.isPlaying)
//            return;

//        quests = quests.OrderBy(quest => quest.name).ToList();
//    }

//#endif
}