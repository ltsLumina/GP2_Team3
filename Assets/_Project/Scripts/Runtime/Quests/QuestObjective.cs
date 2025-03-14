using System;
using UnityEngine;

[System.Serializable]
public abstract class QuestObjective
{
    [SerializeField] private string objectiveName;
    [SerializeField, TextArea(2, 3), Tooltip("\"<itemC>\" will display amount of items collected\n \"<itemT>\" will display total amount of items needed")]
    private string objectiveDescription;

    [SerializeField] private bool giveLevelOnComplete;

    protected bool isStarted;
    protected bool isComplete;

    protected int currentProgress;
    protected int totalProgress;

    public bool IsStarted => isStarted;
    public bool IsComplete => isComplete;
    public string ObjectiveName => string.IsNullOrEmpty(objectiveName) ? "WHY NO NAME, HUH??" : objectiveName;
    public bool GiveLevelOnComplete => giveLevelOnComplete;

    public virtual string ObjectiveDescription
    {
        get
        {
            string description = objectiveDescription;
            description = description.Replace("<itemT>", totalProgress.ToString());
            description = description.Replace("<itemC>", currentProgress.ToString());

            return description;
        }
    }

    public virtual void StartObjective()
    {
        isStarted = true;
        CheckStatus();

        Debug.Log("QUEST OBJECTIVE STARTED: " + ObjectiveName + " /type:" + this.GetType().ToString());
    }

    public abstract void CheckStatus();

    public virtual void ResetObjective()
    {
        totalProgress = 0;
        currentProgress = 0;
        isStarted = false;
        isComplete = false;
    }
}