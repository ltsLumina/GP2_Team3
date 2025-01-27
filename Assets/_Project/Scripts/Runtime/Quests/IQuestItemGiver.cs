using System;

public interface IQuestItemGiver
{
    public event Action<QuestItemSO> OnQuestItemGiven;
}