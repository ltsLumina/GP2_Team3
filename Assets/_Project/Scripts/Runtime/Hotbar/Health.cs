using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public interface IDamageable
{
    float CurrentHealth { get; set; }
    float MaxHealth { get; set; }
    
    void TakeDamage(int damage);

    event Action OnDeath;
}

public class Health : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] float maxHealth = 100f;

    [SerializeField] ParticleSystem hitGlowVFX;

    Player player;

    bool isInvincible;
    bool superInvincible; // wont be reset by stuff like dash.

    public bool SetInvincible(float duration) => StartCoroutine(iFrames(duration)) != null;
    
    public void ToggleInvincible()
    {
        isInvincible = !isInvincible;
        superInvincible = !superInvincible;
    }

    IEnumerator iFrames(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        if (superInvincible) yield break;
        isInvincible = false;
    }

    public Material Material
    {
        get
        {
            Material source = GetComponent<Image>().material;
            
            var material = new Material(source);
            material.SetFloat("_ResourceAmount", Mathf.Clamp01(health / maxHealth));
            GetComponent<Image>().material = material;
            return material;
        }
        set => GetComponent<Image>().material = value;
    }

    public float CurrentHealth
    {
        get
        {
            Material.SetFloat("_ResourceAmount", Mathf.Clamp01(health / maxHealth));
            return Mathf.Clamp(health, 0, maxHealth);
        }
        set
        {
            if (isInvincible)
            {
                Debug.Log("Player is invincible! Will not take this instance of damage.", this);
                return;
            }

            if (value <= 0)
            {
                health = 0;
                OnDeath?.Invoke();
            }
            else
            {
                health = value;
            }
        }
    }

    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value;
            CurrentHealth = maxHealth; // reset health to max health if max health is changed
        }
    }
    
    public bool IsDead { get; set; }

    public static event Action OnDeath;

    public static event Action OnDamageTaken;

    public static event Action OnRevive;

    void Awake()
    {
        // ensure there is only ever one instance of the health component
        if (FindObjectsByType<Health>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1) 
            Debug.LogError("There should only be one instance of the Health component in the scene! \nIt should be on the Health canvas object ONLY.", this);
    }

    void Start()
    {
        player = GameManager.Instance.Player;
        originalMeshColor = player.GetComponentInChildren<SkinnedMeshRenderer>().material.color;

        CurrentHealth = MaxHealth;
        Material.SetFloat("_ResourceAmount", MaxHealth);
    }

    void OnEnable() => OnDeath += Death;
    void OnDisable() => OnDeath -= Death;

    Color originalMeshColor;
    
    void Death() // TODO: fix this garbo
    {
        Debug.LogWarning("Player has died!");

        // VERY BAD | THIS IS FOR ALPHA ONLY (IT WAS NOT ONLY FOR ALPHA LOL)
        
        var inputs = player.GetComponentInChildren<InputManager>();
        inputs.enabled = false;

        var character = player.GetComponent<Character>();
        character.enabled = false;

        var followMouse = player.GetComponent<FollowMouse>();
        followMouse.enabled = false;
        
        player.Animator.SetTrigger("death");
    }
    
    public void Revive()
    {
        Debug.Log("Player has been revived!");
        player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = originalMeshColor;
        player.Animator.SetTrigger("revive");
        
        Material.SetFloat("_ResourceAmount", MaxHealth);

        CurrentHealth = MaxHealth;
        var inputs = player.GetComponentInChildren<InputManager>();
        inputs.enabled = true;

        var character = player.GetComponent<Character>();
        character.enabled = true;

        var followMouse = player.GetComponent<FollowMouse>();
        followMouse.enabled = true;
        
        OnRevive?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        OnDamageTaken?.Invoke();
        CurrentHealth -= damage;
        Debug.Log($"{player.name} took {damage} damage! ({CurrentHealth} health remaining)", this);
        
        // instantiate hitglow effect
        Vector3 hitPos = player.transform.position + new Vector3(0, 1.5f, 0);
        var hitGlow = Instantiate(hitGlowVFX, hitPos, Quaternion.identity).gameObject;
        HitFlash();
        
        // flash screen vignette red on hit
        var volume = FindFirstObjectByType<Volume>(FindObjectsInactive.Exclude);
        if (!volume) return;
        //volume.profile.TryGet(out Vignette vignette);
        //Color original = vignette.color.value;
        
        //if (vignetteRoutine != null) StopCoroutine(vignetteRoutine);
        //vignetteRoutine = StartCoroutine(FlashVignette(vignette, original));
    }
    
    Coroutine vignetteRoutine;
    
    IEnumerator FlashVignette(Vignette vignette, Color original)
    {
        // lerp value quickly to red
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5;
            vignette.color.Override(Color.Lerp(original, Color.red, t));
            yield return null;
        }
        
        // lerp value back to original
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5;
            vignette.color.Override(Color.Lerp(Color.red, original, t));
            yield return null;
        }
        
        vignetteRoutine = null;
    }

    Coroutine flashRoutine;

    void HitFlash()
    {
        var mat = player.GetComponentInChildren<SkinnedMeshRenderer>().material;
        Color original = mat.color;

        if (flashRoutine != null) StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(Flash(mat, original));
    }

    IEnumerator Flash(Material mat, Color original)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        mat.color = original;

        flashRoutine = null;
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        Debug.Log($"Healed {amount}! ({CurrentHealth} health remaining)");
    }
}