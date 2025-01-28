using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] 
    public Enemy enemyPrefab;
    public static EnemyManager Instance { get; private set; }

    public static EnemySpawnPoint[] spawnPoints;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        spawnPoints = GameObject.FindObjectsByType<EnemySpawnPoint>(FindObjectsSortMode.None);
    }
    
    private List<Enemy> activeEnemies = new List<Enemy>();

    private void UpdateEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i].gameObject.activeInHierarchy)
                activeEnemies[i].UpdateEnemy();
            else
                activeEnemies.RemoveAt(i);
        }
    }

    private void UpdateSpawnPoints()
    {
        foreach (EnemySpawnPoint point in spawnPoints)
            point.UpdateSpawnPoint();
    }
    
    public void UpdateEnemyManager()
    {
        UpdateSpawnPoints();
        
        UpdateEnemies();
        
        // remove a random enemy
        if (Input.GetKeyDown(KeyCode.K) && activeEnemies.Count > 0)
        {
            Debug.Log("Killing enemy");
            int randomIndex = Random.Range(0, activeEnemies.Count);
            activeEnemies[randomIndex].GetComponent<ItemDropper>().DropItem();
            activeEnemies[randomIndex].gameObject.SetActive(false);
        }
    }

    public void SpawnEnemy(Vector3 position)
    {
        Enemy newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity, transform);
        // should probably object pool enemies down the line
        activeEnemies.Add(newEnemy);
    }
}
