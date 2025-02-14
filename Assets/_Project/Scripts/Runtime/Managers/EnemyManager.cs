using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] 
    private SerializedDictionary<EnemyType, Enemy> enemyPrefabs = new();
    [SerializeField]
    private Enemy bossPrefab;
    public static EnemyManager Instance { get; private set; }

    public static EnemySpawnPoint[] spawnPoints;

    public float FOWDistance = 0f;

    private Transform playerTransform;

    private BossAOE cachedBoss;

    public enum EnemyType
    {
        Crawler = 0,
        Runner,
        Chaser
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        spawnPoints = FindObjectsByType<EnemySpawnPoint>(FindObjectsSortMode.None);

        playerTransform = GameManager.Instance.Player.transform;

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        
        foreach (Enemy enemy in enemies)
            activeEnemies.Add(enemy);
    }
    
    private List<Enemy> activeEnemies = new List<Enemy>();

    private void UpdateEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            Enemy activeEnemy = activeEnemies[i];

            if (activeEnemy == null)
            {
                activeEnemies.RemoveAt(i);
                continue;
		    }

            if (!activeEnemy.isBoss && Vector3.Distance(activeEnemy.cachedTransform.position, playerTransform.position) > FOWDistance)
            {
                activeEnemy.DisableEnemy();
                continue;
            }
            
            if (!activeEnemy.isBoss)
                activeEnemy.EnableEnemy();

            if (activeEnemy.enabled)
                activeEnemy.UpdateEnemy();
            else
            {
                if (activeEnemy.isBoss)
                    Destroy(activeEnemy.gameObject);
                activeEnemies.RemoveAt(i);
            }
        }
    }

    private void UpdateSpawnPoints()
    {
        foreach (EnemySpawnPoint point in spawnPoints)
            point.UpdateSpawnPoint();
    }

    private void ResetSpawnPoints()
    {
        foreach (EnemySpawnPoint point in spawnPoints)
            point.ResetSpawnPoint();
    }

    private void ResetEnemies()
    {
        foreach (Enemy e in activeEnemies)
        {
            if (e == null)
                continue;
            Destroy(e.gameObject);
        }

        cachedBoss = null;
        activeEnemies.Clear();
    }

    public void ResetAll()
    {
        ResetSpawnPoints();
        ResetEnemies();
    }

    public BossAOE GetBoss()
    {
        return cachedBoss != null ? cachedBoss : null; 
    }
    
    public void UpdateEnemyManager()
    {
        UpdateSpawnPoints();
        
        UpdateEnemies();
    }

    

    public void SpawnEnemy(Vector3 position, bool isBoss, EnemyType type)
    {
        Enemy newEnemy = Instantiate(isBoss ? bossPrefab : enemyPrefabs[type], position, Quaternion.identity, transform);
        newEnemy.isBoss = isBoss;
        newEnemy.ShouldGrabBossComponent();

        if (isBoss)
            cachedBoss = newEnemy.GetComponent<BossAOE>();
        
        // should probably object pool enemies down the line
        activeEnemies.Add(newEnemy);
    }
}