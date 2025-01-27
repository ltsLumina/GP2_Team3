using System;
using TMPro;
using UnityEngine;

public class QuestObjectiveUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    private void OnEnable()
    {
        QuestObjectiveManager.Instance.OnObjectiveUpdated += HandleObjectiveUpdated; 
    }

    private void OnDisable()
    {
        QuestObjectiveManager.Instance.OnObjectiveUpdated -= HandleObjectiveUpdated;
    }

    private void HandleObjectiveUpdated(QuestObjective objective)
    {
        if(objective == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        titleText.text = objective.ObjectiveName;
        descriptionText.text = objective.ObjectiveDescription;
    }
}
