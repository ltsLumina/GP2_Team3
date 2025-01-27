using UnityEngine;

[System.Serializable]
public class QuestObjective
{
    [SerializeField] private string objectiveName;
    [SerializeField, TextArea(2, 3), Tooltip("\"<itemC>\" will display amount of items collected\n \"<itemT>\" will display total amount of items needed")]
    private string objectiveDescription;

    [SerializeField] private QuestItemSO[] requiredItems;

    private int itemsCollected;
    private bool isComplete;

    public bool IsComplete => isComplete;
    public string ObjectiveName => objectiveName;
    public string ObjectiveDescription
    {
        get
        {
            string description = objectiveDescription;
            description = description.Replace("<itemT>", requiredItems.Length.ToString());
            description = description.Replace("<itemC>", itemsCollected.ToString());
            return description;
        }
    }


    public void CheckStatus()
    {
        bool isComplete = true;
        foreach (QuestItemSO item in requiredItems)
        {
            if (!QuestObjectiveManager.Instance.HasItem(item))
            {
                isComplete = false;
            }
            else
            {
                itemsCollected++;
            }
        }
        this.isComplete = isComplete;
    }

    public void ResetObjective()
    {
        itemsCollected = 0;
        isComplete = false;
    }
}