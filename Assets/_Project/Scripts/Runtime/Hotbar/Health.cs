using System;
using UnityEngine;
using UnityEngine.UI;

public interface IDamageable
{
    float CurrentHealth { get; set; }
    float MaxHealth { get; set; }
    
    void TakeDamage(int damage);

    event Action OnDeath;
}

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] float health = 100f;
    [SerializeField] float maxHealth = 100f;

    public Material Material => GetComponent<Image>().material;

    public event Action OnDeath;
    
    public float CurrentHealth
    {
        get
        {
            Material.SetFloat("_ResourceAmount", Mathf.Clamp01(health / maxHealth));
            return Mathf.Clamp(health, 0, maxHealth);
        }
        set
        {
            if (value <= 0)
            {
                health = 0;
                Debug.LogWarning("Dead!");
                OnDeath?.Invoke();
            }
            else
            {
                health = value;
                Debug.Log($"Set health to {value}!");
            }
        }
    }

    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    void Awake()
    {
        health = maxHealth;
        Material.SetFloat("_ResourceAmount", maxHealth);
        
        // ensure there is only every one instance of the health component
        if (FindObjectsByType<Health>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1) 
            Debug.LogError("There should only be one instance of the Health component in the scene! \nIt should be on the Health canvas object ONLY.", this);
    }

    Player player;

    void Start() => player = FindAnyObjectByType<Player>();

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{player.name} took {damage} damage! ({CurrentHealth} health remaining)", this);
    }

    public void Heal(int amount)
    {
        health += amount;
        Debug.Log($"Healed {amount}! ({CurrentHealth} health remaining)");
    }
}