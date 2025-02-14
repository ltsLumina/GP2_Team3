using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossAOE : MonoBehaviour
{
    private Image HealthBar;
    private Image HealthBarFill;

    [SerializeField]
    private List<AOERing> aoeRingPrefabs; // index 0: blood ring, index 1: spike
    private List<AOERing> rings = new();

    [SerializeField]
    private GameObject indicatorPrefab;

    private EnemySpawnPoint[] spawnPoints = new EnemySpawnPoint[4];
    
    private float spawnTime = 0f;
    [SerializeField]
    private float spawnToAttackTime = 7f;

    private float spikeTimer = 0f;
    private float poolTimer = 0f;
    
    private Player player;
    private Animator anim;

    private Enemy enemyComponent;

    private float internalSpikeCooldown;
    
    [Header("Enemy Spawn settings")]
    [SerializeField]
    private float enemySpawningTime = 30f;
    private float enemySpawnCooldown = 0f;

    [Header("Spike attack settings")]
    [SerializeField]
    private float timeToDamageSpike = 0.5f;
    [SerializeField]
    private float spikeSpawnRange = 12f;
    [SerializeField]
    private int spikeDamage = 100;
    [SerializeField]
    private float cooldownAfterSpike = 2f;
    [SerializeField]
    private float spikeAreaSize = 0.1f;
    [SerializeField]
    private float spikeRingOffset = 0f;

    [Header("Pool attack settings")]
    [SerializeField]
    private float bloodPoolDuration = 12f;
    [SerializeField]
    private int bloodPoolDamage = 25;
    [SerializeField]
    private float bloodRingScale = 18f;
    [SerializeField]
    private float cooldownAfterPool = 5f;
    [SerializeField]
    private float maxPoolDistance = 5f;
    [SerializeField]
    private float poolChance = 35f;
    
    [Header("Audio")]
    [SerializeField] private FMODUnity.EventReference spikeAttackSFX;
    [SerializeField] private FMODUnity.EventReference poolAttackSFX;
    [SerializeField] private FMODUnity.EventReference spawnAttackSFX;

    public enum AttackType
    {
        BloodRing = 0,
        SpikeRing
    }

    public void ResetBossAttacks()
    {
        foreach (AOERing ring in rings)
        {
            if (ring.gameObject.activeSelf)
                Destroy(ring.gameObject);
        }

        rings.Clear();
    }

    private void OnDestroy()
    {
        HealthBar.gameObject.SetActive(false);
        ResetBossAttacks();
    }
    
    private void Start()
    {
        enemyComponent = GetComponent<Enemy>();
        player = GameManager.Instance.Player;
        anim = GetComponent<Animator>();
        spawnTime = Time.time;
        enemySpawnCooldown = Time.time;
        internalSpikeCooldown = cooldownAfterSpike;

        HealthBar = GameManager.Instance.HealthBar;
        HealthBarFill = GameManager.Instance.HealthBarFill;
        
        HealthBar.gameObject.SetActive(false);
        
        int index = 0;
        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("BossSpawner"))
        {
            spawnPoints[index] = spawner.GetComponent<EnemySpawnPoint>();
            index++;
        }
    }

    private bool DoesContainRing(AttackType type)
    {
        foreach (AOERing ring in rings)
            if (ring.ringType == type)
                return true;
        return false;
    }
    
    private void SpawnEnemiesAttack()
    {
        foreach (EnemySpawnPoint point in spawnPoints)
            point?.SpawnEnemies();

        FMODUnity.RuntimeManager.PlayOneShotAttached(spawnAttackSFX, gameObject);

        enemySpawnCooldown = Time.time;
    }
    
    private void SpawnDOTRing()
    {
        AOERing aoeRing = Instantiate(aoeRingPrefabs[0], transform.position + new Vector3(0, 0.01f, 0), aoeRingPrefabs[0].transform.rotation, transform);
        aoeRing.transform.localScale = Vector3.one;
        aoeRing.ringType = AttackType.BloodRing;
        aoeRing.aoeRingMat.SetFloat("_Amount", 0.4f);
        aoeRing.bloodRingScale = bloodRingScale;
        rings.Add(aoeRing);

        FMODUnity.RuntimeManager.PlayOneShotAttached(poolAttackSFX, gameObject);
        
        StartCoroutine(SpawnRingVisual(aoeRing.aoeRingMat, aoeRing));
    }

    public void SliceSpikeCooldown() => cooldownAfterSpike = internalSpikeCooldown / 2;
    
    public void ResetSpikeCooldown() => cooldownAfterSpike = internalSpikeCooldown;

    public void SpawnSpike()
    {
        AOERing aoeRing = Instantiate(aoeRingPrefabs[1], GameManager.Instance.Player.transform.position + new Vector3(0, 0.01f, 0), aoeRingPrefabs[1].transform.rotation);
        aoeRing.ringIndicator = Instantiate(indicatorPrefab, aoeRing.transform.position, Quaternion.Euler(90, 0, 0));
        aoeRing.ringIndicator.transform.localScale = new Vector3(spikeAreaSize, spikeAreaSize, spikeAreaSize);
        
        Vector3 position = aoeRing.transform.position;
        position.y -= aoeRing.meshRenderer.bounds.size.y;
        aoeRing.transform.position = position;
        
        aoeRing.transform.localScale = new Vector3(1f, 1f, 1f);
        aoeRing.ringType = AttackType.SpikeRing;
        aoeRing.aoeRingMat.SetFloat("_Amount", 1f);
        aoeRing.ringDamage = spikeDamage;

        aoeRing.ringIndicator.TryGetComponent(out SpriteRenderer indicatorRenderer);
        if (indicatorRenderer != null)
            aoeRing.SetRingSize(indicatorRenderer.bounds.size * spikeAreaSize, spikeRingOffset);

        FMODUnity.RuntimeManager.PlayOneShotAttached(spikeAttackSFX, gameObject);

        rings.Add(aoeRing);
    }

    private void RunAttackAI()
    {
        if (poolTimer >= cooldownAfterPool)
        {   // 35% chance
            if (Random.value <= poolChance && Vector3.Distance(transform.position, player.transform.position) < maxPoolDistance && !DoesContainRing(AttackType.BloodRing))
                SpawnDOTRing();
            poolTimer = 0f;
        }

        if (spikeTimer >= cooldownAfterSpike && Vector3.Distance(transform.position, player.transform.position) <= spikeSpawnRange)
        {
            SpawnSpike();
            spikeTimer = 0f;
        }

        if (Time.time - enemySpawnCooldown >= enemySpawningTime)
            SpawnEnemiesAttack();
    }

    private void UpdateRings()
    {
        if (rings.Count <= 0) return;
        
        for (int i = rings.Count - 1; i >= 0; i--)
        {
            // we have to call this before we potentially remove the ring at the end of this scope
            rings[i].UpdateRing();

            if (rings[i].ringType == AttackType.BloodRing)
            {
                if (Time.time - rings[i].spawnTime >= bloodPoolDuration && rings[i].isDealingDamage)
                    StartCoroutine(DeSpawnRingVisual(rings[i].aoeRingMat, rings[i]));
                continue;
            }

            if (Time.time - rings[i].spawnTime > timeToDamageSpike && !rings[i].didStart)
            {
                rings[i].didStart = true;
                StartCoroutine(SpikeAnimate(rings[i]));
            }
        }
    }

    private float UpdateHealth() => enemyComponent.CurrentHealth >= 0 ? enemyComponent.CurrentHealth / enemyComponent.MaxHealth : 0;

    public void UpdateBossAttack()
    {
        // if player is within x distance from boss we should display the HP!
        HealthBar.gameObject.SetActive(Vector3.Distance(transform.position, player.transform.position) <= 18f);
        if (HealthBar.gameObject.activeSelf)
            HealthBarFill.fillAmount = UpdateHealth();

        transform.rotation = Quaternion.Euler(0, -90f, 0);
        // Please don't comment on my lovely timers.... they're great
        if (Time.time - spawnTime <= spawnToAttackTime) return;
        
        RunAttackAI();

        UpdateRings();

        spikeTimer += Time.deltaTime;
        poolTimer += Time.deltaTime;
    }

    private IEnumerator SpikeAnimate(AOERing spike)
    {
        anim.SetTrigger("Sneeze");
        
        while (spike.transform.position.y < 0.01f)
        {
            spike.transform.position += (Vector3.up * 15)  * Time.deltaTime;
            
            yield return null;
        }
        
        spike.DoInstantDamage();

        yield return new WaitForSeconds(1f);

        Destroy(spike.ringIndicator.gameObject);
        Destroy(spike.gameObject);
        rings.RemoveAt(rings.IndexOf(spike));
    }

    private IEnumerator SpawnRingVisual(Material mat, AOERing aoeRing)
    {
        anim.SetTrigger("Sneeze");
        
        float vfxAmount = 0.4f;

        while(vfxAmount < 1)
        {
            vfxAmount += Time.deltaTime;
            mat.SetFloat("_Amount", vfxAmount);
            yield return null;
        }

        aoeRing.isDealingDamage = true;
        aoeRing.ringDamage = bloodPoolDamage;
    }

    private IEnumerator DeSpawnRingVisual(Material mat, AOERing aoeRing)
    {
        float vfxAmount = 1f;
        aoeRing.isDealingDamage = false;

        while (vfxAmount > 0.4)
        {
            vfxAmount -= Time.deltaTime;
            mat.SetFloat("_Amount", vfxAmount);
            yield return null;
        }

        Destroy(aoeRing.gameObject);
        rings.RemoveAt(rings.IndexOf(aoeRing));
    }
}