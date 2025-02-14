using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestObjectiveUI : MonoBehaviour
{
    [SerializeField] private float animDuration = 1f;
    [SerializeField] private RectTransform objectivePanel;

    [Space, SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Audio")]
    [SerializeField] FMODUnity.EventReference questStartSFX;

    private QuestObjective currentObjective;
    private readonly Queue<QuestObjective> objectiveQueue = new();
    private bool isAnimating = false;

    private Vector2 originalPos => (transform as RectTransform).rect.center;

    private float OffscreenX
    {
        get
        {
            return -objectivePanel.sizeDelta.x * 2f;
        }
    }

    private void OnEnable()
    {
        if (QuestObjectiveManager.Instance)
            QuestObjectiveManager.Instance.OnObjectiveUpdated += HandleObjectiveUpdated;
    }

    private void OnDisable()
    {
        if (QuestObjectiveManager.Instance)
            QuestObjectiveManager.Instance.OnObjectiveUpdated -= HandleObjectiveUpdated;
    }

    private void HandleObjectiveUpdated(QuestObjective objective)
    {
        if (objective != null && objective == currentObjective || objectiveQueue.Contains(objective))
        {
            titleText.text = objective.ObjectiveName;
            descriptionText.text = objective.ObjectiveDescription;
            return;
        }

        objectiveQueue.Enqueue(objective);
        if (!isAnimating && (currentObjective == null || currentObjective.IsComplete))
        {
            ShowNextObjective();
        }
    }

    private async void ShowNextObjective()
    {
        if (objectiveQueue.Count == 0)
        {
            return;
        }

        isAnimating = true;
        QuestObjective nextObjective = objectiveQueue.Dequeue();
        await ShowNewObjective(nextObjective);
        isAnimating = false;

        if (objectiveQueue.Count > 0 && (currentObjective == null || currentObjective.IsComplete))
        {
            ShowNextObjective();
        }
    }

    private async Awaitable ShowNewObjective(QuestObjective objective)
    {
        if (currentObjective == null && objective != null)
        {
            HandleFirstObjective(objective);
        }
        else if (currentObjective == null && objective == null)
        {
            HideObjectivePanel();
        }
        else
        {
            await SwitchObjective(objective);
        }
        currentObjective = objective;
    }

    private void HandleFirstObjective(QuestObjective objective)
    {
        objectivePanel.localPosition = new Vector3(OffscreenX, originalPos.y, 0);
        Debug.Log(objectivePanel.localPosition);

        UpdateObjectiveUI(objective);
        objectivePanel.gameObject.SetActive(true);

        objectivePanel.DOLocalMoveX(originalPos.x, animDuration).SetEase(Ease.OutBack);
        Debug.Log(objectivePanel.localPosition);
    }

    private async Awaitable SwitchObjective(QuestObjective objective)
    {
        await Awaitable.WaitForSecondsAsync(1.5f);

        await objectivePanel.DOLocalMoveX(OffscreenX, animDuration).SetEase(Ease.InBack).AsyncWaitForCompletion();

        if (objective == null)
        {
            HideObjectivePanel();
            return;
        }

        UpdateObjectiveUI(objective);
        objectivePanel.gameObject.SetActive(true);

        FMODUnity.RuntimeManager.PlayOneShot(questStartSFX);
        objectivePanel.DOLocalMoveX(originalPos.x, animDuration).SetEase(Ease.OutBack);
    }

    private void UpdateObjectiveUI(QuestObjective objective)
    {
        titleText.text = objective.ObjectiveName;
        descriptionText.text = objective.ObjectiveDescription;
    }

    private void HideObjectivePanel()
    {
        objectivePanel.gameObject.SetActive(false);
    }
}
