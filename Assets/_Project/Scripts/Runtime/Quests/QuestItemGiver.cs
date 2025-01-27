using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class QuestItemGiver : MonoBehaviour, IInteractable, IQuestItemGiver
{
    [SerializeField] private QuestItemSO questItem;

    public event Action<QuestItemSO> OnQuestItemGiven;

    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void OnHoverEnter() => outline.enabled = true;
    public void OnHoverExit() => outline.enabled = false;
    public void OnInteract()
    {
        OnQuestItemGiven?.Invoke(questItem);
        gameObject.SetActive(false);
    }
}
