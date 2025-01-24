using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] 
    private Enemy enemyPrefab;
    public static EnemyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private List<Enemy> activeEnemies = new List<Enemy>();
    
    public void UpdateEnemyManager()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i].gameObject.activeInHierarchy)
                activeEnemies[i].UpdateEnemy();
            else
                activeEnemies.RemoveAt(i);
        }
        
        // remove a random enemy
        if (Input.GetKeyDown(KeyCode.K) && activeEnemies.Count > 0)
        {
            Debug.Log("Killing enemy");
            int randomIndex = Random.Range(0, activeEnemies.Count);
            activeEnemies[randomIndex].GetComponent<ItemDropper>().DropItem();
            activeEnemies[randomIndex].gameObject.SetActive(false);
        }
        if (Input.GetMouseButtonDown(1))
            SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        Enemy newEnemy = Instantiate(enemyPrefab, transform);
        newEnemy.transform.position = new Vector3(Random.Range(-10, 10), 0, 0);
        // should probably object pool enemies down the line
        activeEnemies.Add(newEnemy);
    }
}
