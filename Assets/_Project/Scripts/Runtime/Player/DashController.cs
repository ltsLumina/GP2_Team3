using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using Tween = DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions>;
using ColorTween = DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>;

public class DashController : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;

    [Header("Damage Settings")]
    [SerializeField] int damage = 50;
    [SerializeField] int manaRestore = 25;
    [Tooltip("The amount of cooldown time in seconds to restore when the dash is executed.")]
    [SerializeField] float cooldownRestore = 1f;
    [Tooltip("Percentage of enemy health remaining to kill. E.g., 0.25f = 25% health remaining will kill the enemy.")]
    [SerializeField] float executeThreshold = 0.25f;

    [Header("Miscellaneous")]
    [SerializeField] float iFrameDuration = 0.5f;
    [SerializeField] float dashColliderRadius = 1f;
    [SerializeField] ParticleSystem dashParticles;

    [Header("Audio")]
    [SerializeField] EventReference dashSFX;
    [SerializeField] EventReference dashResetSFX;

    Player player;
    InputManager inputs;
    CharacterController controller;
    Vector3 dashVelocity;
    Coroutine dashCoroutine;

    bool canDash = true;
    float dashTimeRemaining;
    readonly HashSet<Enemy> hitEnemies = new ();

    public float ExecuteThreshold => executeThreshold;

    bool unlocked;
    public void Unlock() => unlocked = true;

    void Awake()
    {
        player = GetComponent<Player>();
        inputs = GetComponentInChildren<InputManager>();
        controller = GetComponent<CharacterController>();
    }

    Image availableImage;
    Image cooldownImage;

    void Start()
    {
        // no time to refactor this
        Transform dashObject = GameObject.Find("Dash").transform;
        availableImage = dashObject.GetChild(0).GetComponent<Image>();
        cooldownImage = dashObject.GetChild(1).GetComponent<Image>();
    }

    public void Dash()
    {
        if (!unlocked) return;
        
        if (canDash) InitiateDash();
    }

    public void UpdateDash() => ApplyDash();

    void InitiateDash()
    {
        Vector2 moveInput = inputs.MoveInput.normalized;
        var moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = Quaternion.Euler(0, -45, 0) * moveDirection;

        if (moveDirection == Vector3.zero) moveDirection = transform.forward;

        dashVelocity = moveDirection * dashSpeed;
        dashTimeRemaining = dashDuration;
        canDash = false;

        RuntimeManager.PlayOneShotAttached(dashSFX, gameObject);

        // Start invincibility frames
        player.Health.SetInvincible(iFrameDuration);

        ParticleSystem dash = Instantiate(dashParticles, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity, transform);
        dash.transform.forward = -moveDirection;
        StartCoroutine(DashVFX(dash));

        // TODO: Play Dash Audio & Animation
        //Invoke(nameof(ResetDash), dashCooldown);

        dashCoroutine ??= StartCoroutine(DashCoolDown(dashCooldown));
        cooldown = StartCoroutine(Cooldown());

        hitEnemies.Clear();
    }

    void ApplyDash()
    {
        if (dashTimeRemaining > 0)
        {
            controller.Move(dashVelocity * Time.deltaTime);
            dashTimeRemaining -= Time.deltaTime;

            // check for overlapping enemies
            Collider[] enemies = Physics.OverlapSphere(transform.position, dashColliderRadius, LayerMask.GetMask("Enemy"));

            foreach (var enemy in enemies)
            {
                if (enemy.TryGetComponent(out Enemy e) && hitEnemies.Add(e))
                {
                    // Add the enemy to the set of hit enemies
                    if (e.CurrentHealth / e.MaxHealth <= executeThreshold)
                    {
                        player.Mana.Adjust(manaRestore);
                        e.TakeDamage(9999);
                        RuntimeManager.PlayOneShotAttached(dashResetSFX, player.gameObject);

                        if (dashCoroutine != null)
                        {
                            StopCoroutine(dashCoroutine);
                        }

                        // dash reset has occured
                        StartCoroutine(DashCoolDown(dashCooldown - cooldownRestore));

                        if (cooldown != null) StopCoroutine(cooldown);
                        cooldown = null;
                        fadeIn.Kill();
                        fill.Kill();
                        fadeOut.Kill();
                        
                        cooldownImage.gameObject.SetActive(false);
                        cooldownImage.fillAmount = 1f;
                        
                        availableImage.gameObject.SetActive(false);

                        return;
                    }

                    e.TakeDamage(damage);
                }
            }
        }
    }

    private IEnumerator DashVFX(ParticleSystem dash)
    {
        yield return new WaitForSeconds(0.5f);
        dash.transform.SetParent(null);
        yield return new WaitForSeconds(0.5f);
        Destroy(dash.gameObject);
    }

    private IEnumerator DashCoolDown(float coolDown)
    {
        yield return new WaitForSeconds(coolDown);
        canDash = true;
        dashCoroutine = null;
    }

    #region UI

    Coroutine cooldown;
    bool OnCooldown => cooldown != null;
    IEnumerator Cooldown()
    {
        if (OnCooldown) yield break;

        StartCoroutine(Animation());
        
        yield return new WaitForSeconds(dashCooldown);
    }

    ColorTween fadeIn;
    Tween fill;
    ColorTween fadeOut;

    IEnumerator Animation()
    {
        const float duration = 0.35f;

        fadeIn.Kill();
        fill.Kill();
        fadeOut.Kill();

        #region Set Alpha to 0
        Color color = availableImage.color;
        color.a = 0f;
        availableImage.color = color;
        #endregion

        fadeIn = availableImage.DOFade(0.75f, 0.35f);
        fadeIn.OnStart(() => availableImage.gameObject.SetActive(true));

        cooldownImage.gameObject.SetActive(true);
        cooldownImage.fillAmount = 1f;

        fill = cooldownImage.DOFillAmount(0f, dashCooldown);

        yield return new WaitForSeconds(dashCooldown);

        fadeOut = availableImage.DOFade(0f, duration);
        fadeOut.OnComplete(() => availableImage.gameObject.SetActive(false));

        cooldownImage.gameObject.SetActive(false);
        cooldownImage.fillAmount = 1f;

        cooldown = null;
    }
    
    #endregion
}