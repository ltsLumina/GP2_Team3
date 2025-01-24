using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using VInspector;
using Random = UnityEngine.Random;

[SelectionBase]
public class Enemy : MonoBehaviour, IDamageable
{
    public enum EnemyType
    {
        Weak,
        Normal,
        Strong,
    }

    [Header("Enemy")]
    [SerializeField] EnemyType type = EnemyType.Normal;

    [Header("Attributes")]
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth = 100;

    public event Action OnDeath;
    
    public EnemyType Type => type;

    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            if (currentHealth <= 0)
            {
                Debug.Log("Enemy died!");
                OnDeath?.Invoke();
            }
        }
    }
    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

#if UNITY_EDITOR // THIS IS A DEBUG BUTTON DO NOT ACTUALLY USE
    [Button, UsedImplicitly]
    void Kill()
    {
        var anim = GetComponentInChildren<Animator>();
        anim.enabled = false;

        var ai = GetComponent<EnemyAI>();
        ai.enabled = false;

        var agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        var capsule = GetComponent<CapsuleCollider>();
        capsule.enabled = false;

        enabled = false;
    }
#endif

    void Awake()
    {
        OnDeath += () => Destroy(gameObject);
        
        currentHealth = maxHealth;
    }

    public void UpdateEnemy() { }
    
    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage!");
        
        // Note: all damage has 5% +- variance. Does a few things to make the game feel more dynamic. If there are damage numbers, for instance, they won't always be the same.
        CurrentHealth -= damage * Random.Range(0.95f, 1.05f);
    }
}

