using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Outline), typeof(NavMeshObstacle))]
public class Door : MonoBehaviour, IQuestTarget, IInteractable
{
    [Header("Quest Settings")]
    [SerializeField] private QuestSO quest;
    [SerializeField] private bool startQuestOnInteract = false;

    [Header("Audio")]
    [SerializeField] FMODUnity.EventReference doorOpeningSFX;

    private Outline outline;
    private NavMeshObstacle navObstacle;
    private Animator animator;

    private bool isLocked;

    public QuestSO Quest => quest;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        navObstacle = GetComponent<NavMeshObstacle>();
        navObstacle.enabled = true;

        animator = GetComponentInChildren<Animator>();

        GetComponentInChildren<BoxCollider>().isTrigger = false;
    }

    public void OnInteract()
    {
        if (Quest != null)
        {
            if (!Quest.Objective.IsStarted && startQuestOnInteract)
                QuestObjectiveManager.Instance.StartQuest(Quest);

            if (!Quest.Objective.IsComplete)
            {
                Debug.Log("Door is locked");
                return;
            }

            //Debug.Log("Door is unlocked");
            //QuestObjectiveManager.Instance.EndQuest(Quest);
        }

        bool wasOpen = animator.GetBool("Open");

        SetOpen(!wasOpen);
    }

    public void SetOpen(bool isOpen)
    {
        if (isLocked)
            return;

        navObstacle.enabled = !isOpen;
        GetComponentInChildren<BoxCollider>().isTrigger = isOpen;
        animator.SetBool("Open", isOpen);

        FMODUnity.RuntimeManager.PlayOneShotAttached(doorOpeningSFX, gameObject);

    }

    public void SetLocked(bool isLocked)
    {
        if (isLocked)
        {
            navObstacle.enabled = true;
            SetOpen(false);
        }

        this.isLocked = isLocked;
    }

    public void OnHoverEnter()
    {
        Color outlineColor;

        if (Quest == null)
        {
            outlineColor = Color.white;
        }
        else
        {
            outlineColor = Quest.Objective.IsComplete ? Color.white : Color.red;
        }

        outline.OutlineColor = outlineColor;
        outline.enabled = true;
    }

    public void OnHoverExit()
    {
        outline.enabled = false;
    }

    public bool IsOpen
    {
        get => GetComponent<Animator>().GetBool("Open");
    }
}
