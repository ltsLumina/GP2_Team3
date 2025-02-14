using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using VInspector;

[System.Serializable]
public class ItemQuestObjective : QuestObjective
{
    [SerializeField] private ItemQuestData[] requiredItems;

    public override void StartObjective()
    {
        requiredItems.Select(x => x.Item).ToList().ForEach(item => item.CanBeCollected = true);

        currentProgress = 0;
        RecountTotalProgress();

        base.StartObjective();
    }

    public void RecountTotalProgress()
    {
        totalProgress = 0;
        foreach (var item in requiredItems)
        {
            totalProgress += item.Amount;
        }
    }

    public override void CheckStatus()
    {
        int progress = 0;

        bool isComplete = true;
        foreach (var item in requiredItems)
        {
            if (!QuestObjectiveManager.Instance.HasItem(item.Item, out var itemCount))
            {
                isComplete = false;
            }
            else
            {
                progress += itemCount;

                if (itemCount < item.Amount)
                {
                    isComplete = false;
                }
            }
        }

        if (isComplete)
        {
            EndObjective();
        }

        currentProgress = progress;
        this.isComplete = isComplete;
    }

    private async void EndObjective()
    {
        if (isComplete)
            return;

        isComplete = true;

        await Awaitable.WaitForSecondsAsync(1.3f);
        QuestObjectiveManager.Instance.EndQuest(this);
    }

    public void SetRequiredItems(ItemQuestData[] requiredItems)
    {
        this.requiredItems = requiredItems;
        RecountTotalProgress();
    }
}

[System.Serializable]
public struct ItemQuestData
{
    [SerializeField] private QuestItemSO item;
    [SerializeField] private int amount;

    public QuestItemSO Item => item;
    public int Amount => amount;

    public ItemQuestData(QuestItemSO item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}