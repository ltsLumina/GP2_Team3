using System;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using VInspector;

[SelectionBase]
public class Enemy : MonoBehaviour, IDamageable
{
    public enum EnemyType
    {
        Weak,
        Normal,
        Strong,
        DEBUG,
    }

    [Header("Enemy")]
    [SerializeField] EnemyType type = EnemyType.Normal;
    [SerializeField] bool miniBoss;
    [ShowIf("miniBoss")]
    [SerializeField] Vector3 scaleModifier = new Vector3(2, 2, 2);
    [EndIf]

    [Header("Attributes")]
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth = 100;

    [SerializeField] ParticleSystem hitFlash;

    [Header("Boss Settings")]
    public bool isBoss = false;
    public BossAOE bossAttacks;
    public Transform cachedTransform;
    public SkinnedMeshRenderer cachedMeshRenderer; // TODO: why public? @daniel :(

    [Header("Audio")]
    [SerializeField] private EventReference dyingSFX;
    [SerializeField] private EventReference detectPlayerSFX;
    [SerializeField] private EventReference takeDamageSFX;

    Material mat;
    Player player;

    public event Action OnDeath;
    
    public EnemyType Type => type;

    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            // float variance = Random.Range(0.95f, 1.05f);
            // variance = Mathf.Round(variance * 100) / 100;

            Vector3 hitPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

            Instantiate(hitFlash.gameObject, hitPos, Quaternion.identity);

            RuntimeManager.PlayOneShotAttached(takeDamageSFX, gameObject);

            currentHealth = Mathf.Clamp(value, 0, maxHealth);

            float executeThreshold = player.DashController.ExecuteThreshold;

            if(currentHealth / maxHealth <= executeThreshold) 
                mat.SetFloat("_ExecuteThreshhold", 1);

            if (currentHealth <= 0)
            {
                mat.SetFloat("_ExecuteThreshhold", 0); // so that the shader doesn't stay on while the enemy is dead
                RuntimeManager.PlayOneShotAttached(dyingSFX, gameObject);
                OnDeath?.Invoke();
            }
        }
    }
    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }
    
    public bool IsDead => currentHealth <= 0;

    Animator anim;
    EnemyAI ai;
    NavMeshAgent agent;
    new Collider collider;
    
    // THIS IS A DEBUG BUTTON DO NOT ACTUALLY USE
    [Button, UsedImplicitly]
    void DEBUG_Kill()
    {
        anim.enabled = false;
        ai.enabled = false;
        agent.enabled = false;
        collider.enabled = false;

        enabled = false;
    }

    void Kill()
    {
        anim.enabled = false;
        if (!isBoss)
        {
            ai.enabled = false;
            agent.enabled = false;
        }

        collider.enabled = false;
        
        enabled = false;
    }

    void Awake()
    {
        OnDeath += () =>
        {
            Kill();
            if (TryGetComponent(out ItemDropper itemDropper)) 
                itemDropper.DropItem();
        };
        
        currentHealth = maxHealth;
        
        anim = GetComponentInChildren<Animator>();
        ai = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        collider = GetComponent<Collider>();
        
        cachedTransform = transform;
        cachedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        mat = GetComponentInChildren<Transform>().GetComponentInChildren<Renderer>().material;
    }

    void Start()
    {
        player = GameManager.Instance.Player;

        if (miniBoss)
        {
            transform.localScale = scaleModifier;
        }
    }

    public void ShouldGrabBossComponent()
    {
        if (isBoss)
            bossAttacks = GetComponent<BossAOE>();
    }

    public void UpdateEnemy()
    {
        if (!enabled) return;
        
        if (!isBoss)
        {
            ai.UpdateAI();
            return; // early return so we don't attempt calling bossAttacks....
        }

        bossAttacks?.UpdateBossAttack();
    }
    

    public void DisableEnemy()
    {
        ai.enabled = false;
        cachedMeshRenderer.enabled = false;
        enabled = false;
    }

    public void EnableEnemy()
    {
        ai.enabled = true;
        cachedMeshRenderer.enabled = true;
        enabled = true;
    }
    
    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage!");

        #region maybe not
        // Note: all damage has 5% +- variance. Does a few things to make the game feel more dynamic. 
        // If there are damage numbers, for instance, this would make them not all the same.
        #endregion
        CurrentHealth -= damage * player[Player.Stats.Damage];
    }
}

