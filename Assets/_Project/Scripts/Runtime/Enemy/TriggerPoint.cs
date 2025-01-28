using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPoint : MonoBehaviour
{
    [SerializeField]
    private List<EnemySpawnPoint> spawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        foreach (EnemySpawnPoint point in spawnPoint)
        {
            if (other.CompareTag("Player") && point.isActive && !point.shouldSpawn)
                point.SpawnEnemies();
        }
    }
}
