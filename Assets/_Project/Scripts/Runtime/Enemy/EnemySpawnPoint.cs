using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]
    private int minAmount = 0;
    [SerializeField]
    private int maxAmount = 0;
    [SerializeField]
    private float spawnDelay = 0f;
    [SerializeField]
    private int spawnCharges = 0;
    [SerializeField]
    private float spawnRadius = 0f;
    [SerializeField]
    private bool isBossSpawner = false;
    [SerializeField]
    private EnemyManager.EnemyType enemyType = EnemyManager.EnemyType.Chaser;
    
    private float lastSpawnTime = 0f;
    private int chargesLeft = 0;
    
    private List<Vector3> spawnPoints = new();
    
    private Transform cachedTransform;

    
    public bool shouldSpawn = false;
    public bool isActive = true;

    private void Awake()
    {
        // run preset settings for boss spawner
        if (isBossSpawner)
        {
            spawnCharges = 1;
            chargesLeft = spawnCharges;
            spawnDelay = 0f;
            minAmount = 1;
            maxAmount = 1;
        }
        else
            chargesLeft = spawnCharges;
        
        cachedTransform = GetComponent<Transform>(); // cache transform since this does an external c++ call
    }

    public void ResetSpawnPoint()
    {
        isActive = true;
        chargesLeft = spawnCharges;
        shouldSpawn = false;
        lastSpawnTime = 0f;
        spawnPoints.Clear();
    }

    public void UpdateSpawnPoint()
    {
        if (isActive)
        {
            if (shouldSpawn)
                if (Time.time - lastSpawnTime > spawnDelay && chargesLeft > 0)
                    SpawnEnemies();

            if (shouldSpawn && chargesLeft <= 0)
                isActive = false;
        }
    }

    private Vector3 generateNewSpawnPoint()
    {
        return new Vector3(cachedTransform.position.x + Random.Range(-spawnRadius, spawnRadius), cachedTransform.position.y, cachedTransform.position.z + Random.Range(-spawnRadius, spawnRadius));
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.2f);
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }
#endif

    public void SpawnEnemies()
    {
        int enemyAmount = Random.Range(minAmount, maxAmount + 1); // make maxAmount inclusive by going + 1
        for (int i = 0; i < enemyAmount; i++)
        {
            // unreadable code but I love ternary operators 
            Vector3 spawnPosition = isBossSpawner ?
                                    new Vector3(cachedTransform.position.x + (spawnRadius / 2), cachedTransform.position.y, cachedTransform.position.z + (spawnRadius / 2))
                                    :
                                    generateNewSpawnPoint();

            if (spawnPoints.Count > 0)
            {
                int index = 0;
                Vector3 currentPoint = spawnPoints[index];
                while (Mathf.Abs(currentPoint.x - spawnPosition.x) < 0.25f ||
                       Mathf.Abs(currentPoint.y - spawnPosition.y) < 0.25f)
                {
                    if (index == spawnPoints.Count)
                        break;
                    
                    spawnPosition = generateNewSpawnPoint();
                    index = (index + 1) % spawnPoints.Count;
                }
            }

            EnemyManager.Instance.SpawnEnemy(spawnPosition, isBossSpawner, enemyType);
        }

        lastSpawnTime = Time.time;
        chargesLeft--;
        shouldSpawn = true;
    }
}
