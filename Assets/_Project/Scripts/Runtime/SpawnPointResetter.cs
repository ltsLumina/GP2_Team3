using UnityEngine;

public class SpawnPointResetter : MonoBehaviour
{
    private EnemySpawnPoint[] spawnPoints;

    private void Start()
    {
        spawnPoints = transform.parent.GetComponentsInChildren<EnemySpawnPoint>();
    }

    public void ResetSpawnPoints()
    {
        foreach(var spawnPoint in spawnPoints)
        {
            spawnPoint.ResetSpawnPoint();
            spawnPoint.SpawnEnemies();
        }
        gameObject.SetActive(false);
    }
}
