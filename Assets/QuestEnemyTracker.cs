using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public class QuestEnemyTracker : MonoBehaviour, IQuestItemGiver
{
    [SerializeField] private QuestItemSO questItem;

    private List<Enemy> enemyList;

    public event Action<QuestItemSO> OnQuestItemGiven;

    private void Start()
    {
        var colliders = Physics.OverlapBox(transform.position, transform.localScale / 2 /*, LayerMask.GetMask("Enemy")*/);
        Debug.Log($"Detected {colliders.Length} colliders");

        foreach (var collider in colliders)
        {
            var enemy = collider.GetComponent<Enemy>();
            Debug.Log($"Collider: {collider.name}, Enemy Component: {enemy}", collider);
        }

        enemyList = colliders
            .Select(c => c.GetComponent<Enemy>())
            .Where(e => e != null)
            .ToList();

        Debug.Log($"Enemies found: {enemyList.Count}");

        foreach (var enemy in enemyList)
        {
            enemy.OnDeath += () => HandleEnemyDeath(enemy);
        }
    }


    private void HandleEnemyDeath(Enemy enemy)
    {
        enemyList.Remove(enemy);

        if (enemyList.Count == 0)
        {
            Debug.Log("All enemies are dead");

            OnQuestItemGiven?.Invoke(questItem);

            DialogData da = new("-", "YIPPIEE", DialogType.Bubble);
            DialogManager.Instance.ShowDialog(da);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    [Button]
    void KillAllEnemies()
    {
        foreach (var enemy in enemyList.ToList())
        {
            enemy.CurrentHealth = 0;
        }
    }

#endif
}
