using System;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(Outline), typeof(Collider))]
public class QuestItemGiver : MonoBehaviour, IInteractable, IQuestItemGiver
{
    [SerializeField] private QuestItemSO questItem;

    [Header("Optional")]
    [SerializeField] private DialogDataSO dialogData;

    [Header("Audio")]
    [SerializeField] private EventReference pickUpQuestItem;

    [Header("Visual")]
    [SerializeField] private float outlineWidth = 4f;
    [SerializeField] private bool removeVisualsOnQuestItemGiven = true;

    public event Action<QuestItemSO> OnQuestItemGiven;

    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = true;

        outline.OutlineWidth = outlineWidth;
        outline.OutlineColor = Color.yellow;
    }


    public void OnHoverEnter()
    {
        outline.OutlineColor = questItem.CanBeCollected ? Color.green : Color.red;
        outline.enabled = true;
    }

    public void OnHoverExit()
    {
        outline.OutlineColor = Color.yellow;
    }

    public void OnInteract()
    {
        if (!questItem.CanBeCollected || !isActiveAndEnabled)
            return;

        if (dialogData)
            dialogData.Show();

        RuntimeManager.PlayOneShot(pickUpQuestItem);

        if (gameObject.name == "Weapon")
        {
            var player = GameManager.Instance.Player;
            player.GetComponent<HideWeaponOnStart>().ShowWeapon();
        }

        enabled = false;
        if (removeVisualsOnQuestItemGiven)
            gameObject.SetActive(false);

        OnQuestItemGiven?.Invoke(questItem);
    }

#if(UNITY_EDITOR)
    private void OnValidate()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
#endif
}