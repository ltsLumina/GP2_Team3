using UnityEngine;



public abstract class QuestSO : ScriptableObject
{
    [SerializeField] private QuestSO previousQuest;
    [SerializeField] private QuestSO nextQuest; 

    public virtual QuestObjective Objective { get; }
    
    public QuestSO PreviousQuest => previousQuest;
    public QuestSO NextQuest => nextQuest;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(previousQuest != null && previousQuest == this)
        {
            Debug.LogWarning("Previous quest cannot be the same as the Quest itself");
            previousQuest = null;
        }

        if (nextQuest != null && nextQuest == this)
        {
            Debug.LogWarning("Next quest cannot be the same as the Quest itself");
            nextQuest = null;
        }
    }
#endif
}
