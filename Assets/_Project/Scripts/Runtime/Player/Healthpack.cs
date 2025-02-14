using System;
using System.Collections;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class Healthpack : MonoBehaviour, IInteractable
{
    [Tooltip("If enabled, the healthpack will trigger when the player interacts with it, rather than when the player enters the trigger.")]
    [Header("Options")]
    [SerializeField] bool triggerThroughInteraction;
    [Space(10)]
    
    [SerializeField] int healAmount = 50;
    [Space(5)]
    [Tooltip("If enabled, the healthpack will restock after a time interval.")]
    [SerializeField] bool restock;
    [SerializeField] float restockTime = 60f;
    [Space(5)]
    [SerializeField] Image fill;
    [Space(10)]

    [Header("Audio")]
    [SerializeField] EventReference pickUpHealthPackSFX;

    [Header("Debug")]
    [SerializeField] bool consumed;
    
    event Action OnConsumed;

    void Awake() => OnConsumed += () => consumed = true;

    void Start()
    {
        var canvas = GetComponentInChildren<Canvas>();
        if (!canvas) return; 
        Camera.main.transform.GetChild(0).TryGetComponent(out Camera UICam);
        canvas.worldCamera = UICam;
        
        fill.enabled = false;
        StartCoroutine(UpdateLookAt());
    }
    
    IEnumerator UpdateLookAt()
    {
        // while player is within a certain distance, look at player
        while (true)
        {
            fill.transform.LookAt(Camera.main.transform.position);
            yield return new WaitForSeconds(0.1f); // Adjust interval as needed
        }
    }

    void Update()
    {
        fill.transform.LookAt(Camera.main.transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggerThroughInteraction) return;
        if (consumed) return;

        if (other.TryGetComponent(out Player player))
        {
            player.Health.Heal(healAmount);
            OnConsumed?.Invoke();
            if (restock) StartCoroutine(Restock());
        }
    }

    public void OnInteract()
    {
        if (!triggerThroughInteraction) return;
        if (consumed) return;
        
        Player player = GameManager.Instance.Player;
        player.Health.Heal(healAmount);
        RuntimeManager.PlayOneShot(pickUpHealthPackSFX);
        OnConsumed?.Invoke();
        if (restock) StartCoroutine(Restock());
    }

    IEnumerator Restock()
    {
        #region DOTween Fill
        const float fillDuration = 1f;

        Sequence sequence = DOTween.Sequence(); 
        // initial setup is to fill the cooldown icon with a small animation, to indicate that the cooldown is active
        sequence.SetRecyclable(true)
                .OnComplete(() => sequence.Kill())
                .OnStart(() => fill.enabled = true)
                .Append(fill.DOFillAmount(1, fillDuration).From(0).SetEase(Ease.OutBack));
        
        // this "un-fills" the cooldown icon after the fill animation is complete, until it reaches 0, indicating the cooldown is over.
        sequence.OnComplete
                 (() => // ticks down the fill amount to 0, indicating the cooldown period until the healthpack restocks
                      DOTween.To(() => fill.fillAmount, x => fill.fillAmount = x, 0, restockTime)
                             .OnStart(() => fill.enabled = true)
                             .From(1)
                             .SetEase(Ease.Linear)
                             .OnComplete(() => { fill.enabled = false; }));
        #endregion
        
        yield return new WaitForSeconds(restockTime + fillDuration); // wait for the restock time + the fill duration, otherwise the restock is 1x fillDuration too short
        consumed = false;
    }

    #region unused interaction methods
    public void OnHoverEnter() { }

    public void OnHoverExit() { }
    #endregion
}
