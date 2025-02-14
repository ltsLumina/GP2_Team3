using NoSlimes;
using System;
using UnityEngine;

/// <summary>
/// This class is responsible for handling the checkpoint system.
/// Re-Enable when it's time, for now, it's just a placeholder.
/// </summary>

public class CheckpointController : MonoBehaviour
{
    private static CheckpointController _instance;
    private Health playerHealth;
    public static CheckpointController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<CheckpointController>();
            }
            return _instance;
        }
    }

    private Vector3 checkpointPos;

    private void OnEnable()
    {
        if (QuestObjectiveManager.Instance)
            QuestObjectiveManager.Instance.OnQuestCompleted += OnQuestCompleted;
        Health.OnDeath += OnPlayerDeath;

        playerHealth = FindAnyObjectByType<Health>();
    }

    private void OnDisable()
    {
        if (QuestObjectiveManager.Instance)
            QuestObjectiveManager.Instance.OnQuestCompleted -= OnQuestCompleted;
        Health.OnDeath -= OnPlayerDeath;
    }

    private void OnQuestCompleted(QuestSO _)
    {
        //WHAT IS WRONG WITH COPILIOT
        //char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

        //string text = "one\ttwo three:four,five six seven";

        checkpointPos = GameManager.Instance.Player.transform.position;
    }

    private void OnPlayerDeath()
    {
        //SCrap the copilot's idea
        //GameManager.Instance.Player.transform.position = checkpointPos;
        //Debug.Log(Equals(checkpointPos, GameManager.Instance.Player.transform.position)); // ??? Copilot says this is always true

        //Debug.Log("Player sadly passed away, but respawned at checkpoint :D");

        //Do this crap instead

        GameManager.Instance.SwitchState<DeathState>();
    }

    public void RespawnPlayer()
    {
        GameManager.Instance.Player.transform.position = checkpointPos;

        EnemyManager.Instance.GetBoss()?.ResetBossAttacks();
        EnemyManager.Instance.ResetAll();
        playerHealth.Revive();

        GameManager.Instance.SwitchState<PlayingState>();
    }
}
