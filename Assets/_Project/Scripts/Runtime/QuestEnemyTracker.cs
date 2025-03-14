using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public class QuestEnemyTracker : MonoBehaviour, IQuestItemGiver
{
    [SerializeField] private QuestItemSO questItem;
    [SerializeField] private ItemQuestSO quest;

    private List<Enemy> enemyList;
    private List<Enemy> killedEnemies;
    private bool hasTriggered;

    public event Action<QuestItemSO> OnQuestItemGiven;

    public async void FindEnemies(float delay = 0f)
    {
        if (hasTriggered) return;

        await Awaitable.WaitForSecondsAsync(delay);

        killedEnemies = new();

        var colliders = Physics.OverlapBox(transform.position, transform.localScale / 2);
        enemyList = colliders
            .Select(c => c.GetComponent<Enemy>())
            .Where(e => e != null)
            .ToList();

        Debug.Log($"Enemies found: {enemyList.Count}");

        quest.UpdateRequiredItem(new ItemQuestData(questItem, enemyList.Count));
        QuestObjectiveManager.Instance.UpdateCurrentQuest();

        foreach (var enemy in enemyList)
        {
            enemy.OnDeath += () => HandleEnemyDeath(enemy);
        }

        hasTriggered = true;
    }


    private void HandleEnemyDeath(Enemy enemy)
    {
        if (killedEnemies.Contains(enemy)) return;
        if (!enemyList.Contains(enemy)) return;

        enemyList.Remove(enemy);
        killedEnemies.Add(enemy);

        Debug.Log($"Enemy killed. Total kills: {killedEnemies}/{enemyList.Count + killedEnemies.Count}");

        OnQuestItemGiven?.Invoke(questItem);

        if (enemyList.Count == 0)
        {
            Debug.Log("All enemies are dead. Quest complete.");
            QuestObjectiveManager.Instance.EndQuest(quest);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

#endif
}
