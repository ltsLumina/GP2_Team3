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

    public override string ObjectiveDescription
    {
        get
        {
            string description = base.ObjectiveDescription;

            for(int i = 0; i < requiredItems.Length; i++)
            {
                var item = requiredItems[i];

                //description = description.Replace($"<itemT_{i}>", item.Amount.ToString());
                description = description.Replace($"<itemC_{i}>", item.Amount.ToString());
            }

            return description;
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

    public void UpdateRequiredItem(ItemQuestData item)
    {
        //this.requiredItems = item;
        bool foundMatch = false;
        foreach (var i in requiredItems)
        {
            if (i.Item == item.Item)
            {
                requiredItems[Array.IndexOf(requiredItems, i)] = item;
                foundMatch = true;
                break;
            }
        }

        if(!foundMatch)
            requiredItems = requiredItems.Append(item).ToArray();

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