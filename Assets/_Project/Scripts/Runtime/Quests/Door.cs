using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Door : MonoBehaviour, IQuestTarget, IInteractable
{
    [SerializeField] private string questTargetName = "Door";
    [SerializeField] private QuestSO questObjective;

    [SerializeField] private GameObject[] doorParts;

    private Outline outline;

    public string TargetName => questTargetName;
    public QuestObjective QuestObjective => questObjective.Objective;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void OnInteract()
    {
        if(!QuestObjective.IsComplete)
        {
            Debug.Log("Door is locked");
            return;
        }

        foreach (var doorPart in doorParts)
        {
            doorPart.transform.DOMoveY(5, 5f);
        }

        enabled = false;
    }

    public void OnHoverEnter()
    {
        var outlineColor = QuestObjective.IsComplete ? Color.white : Color.red;
        outline.OutlineColor = outlineColor;
        outline.enabled = true;
    }

    public void OnHoverExit()
    {
        outline.enabled = false;
    }
}
