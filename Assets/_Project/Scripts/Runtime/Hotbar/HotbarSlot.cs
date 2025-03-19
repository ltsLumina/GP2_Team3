#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using System.Collections;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.UI;
using Tween = DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions>;
using ColorTween = DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>;

[RequireComponent(typeof(Button)), ExecuteInEditMode]
public class HotbarSlot : MonoBehaviour
{
    [SerializeField] Ability ability;
    [Space(10)]
    [SerializeField, ReadOnly] bool unlocked;
    [SerializeField] bool log;

    Image availableImage;
    Image cooldownImage;

    Button button;

    bool notEnoughMana
    {
        get
        {
            // if the mana cost is positive, the ability will increase the mana amount, so don't check for mana
            if (ability.ManaCost > 0) return false;
            return FindAnyObjectByType<Mana>().CurrentMana < Mathf.Abs(ability.ManaCost);
        }
    }

    public void Unlock() => unlocked = true;

    void Start()
    {
#if UNITY_EDITOR
        if (PrefabStageUtility.GetCurrentPrefabStage()) return;
#endif

        #region Naming Utility & Validation
        if (ability)
        {
            name = $"\"{ability.Name}\" [{transform.GetSiblingIndex() + 1}]";
            GetComponent<Image>().sprite = ability.Icon;
        }
        else
        {
            name = $"Empty Slot [{transform.GetSiblingIndex() + 1}]";
            Debug.LogError("No ability assigned to this slot!" + "\n", this);
        }
        #endregion

        #region Mega Over-Engineered UI Setup
        // i was bored alright leave me alone lol
        
        GameObject availableObject = null;
        GameObject cooldownObject = null;

        // Check for the children by name, and if the name and index is correct, assign it to the variable.
        if (transform.GetChild(1).gameObject.name == "Available")
            availableObject = transform.GetChild(1).gameObject;

        if (transform.GetChild(2).gameObject.name == "Cooldown")
            cooldownObject = transform.GetChild(2).gameObject;

        // If the objects are not found, try to find them by name.
        availableObject ??= gameObject.transform.Find("Available").gameObject;
        cooldownObject ??= gameObject.transform.Find("Cooldown").gameObject;

        // Check if their index is correct.
        Debug.Assert(availableObject.transform.GetSiblingIndex() == 1, "Available object is not in the correct index!" + "\nIt should be at index 1.", availableObject);
        Debug.Assert(cooldownObject.transform.GetSiblingIndex() == 2, "Cooldown object is not in the correct index!" + "\nIt should be at index 2.", cooldownObject);

        // Validate the objects.
        bool availableImageExists = availableObject.TryGetComponent(out Image availableComponent);
        bool cooldownImageExists = cooldownObject.TryGetComponent(out Image cooldownComponent);

        Debug.Assert(availableImageExists, "No Image component found!", availableObject);
        Debug.Assert(availableObject.name == "Available", "Available object is not named correctly!", availableObject);
        Debug.Assert(cooldownImageExists, "No Image component found!", cooldownObject);
        Debug.Assert(cooldownObject.name == "Cooldown", "Cooldown object is not named correctly!", cooldownObject);

        // Initialization
        availableObject.SetActive(false);
        cooldownObject.SetActive(false);

        availableImage = availableComponent;
        cooldownImage = cooldownComponent;
        #endregion

        button = GetComponent<Button>();

        // Cannot use the ability if it is locked. (e.g. not enough level)
        if (unlocked) button.interactable = false;
        else button.onClick.AddListener(OnSlotClicked);
    }

    //bool pointerOverUI; // unity sucks balls

    void Update() // unity sucks balls
    {
        if (!Application.isPlaying) return;
        // check if the cursor is over a UI element and the layer is "Ability"
        //pointerOverUI = EventSystem.current.IsPointerOverGameObject();/* && EventSystem.current.currentSelectedGameObject.layer == LayerMask.NameToLayer("Ability");*/

        if (button) button.interactable = !notEnoughMana && unlocked;

        if (!unlocked)
        {
            // show a lock icon or something
        }
    }

    Coroutine cooldown;

    public void OnSlotClicked()
    {
        if (!GameManager.Instance.Player.HasWeapon) return;
        
        if (!unlocked || notEnoughMana) return;

        if (!ability || cooldown != null) return;

        // If the ability is a menu, use a different cooldown method.
        if (ability.IsMenu) cooldown = StartCoroutine(MenuCooldown());
        else
        {
            // // If the ability is a mouse ability (left/right click) and the cursor is over a UI element, do not use the ability.
            // if (ability.IsMouse/* && pointerOverUI*/) // NEVERMIND ///////bug: if this is commented, as it is, you can click the UI button and use the ability at the same time
            // {
            //     if (log) Debug.Log("Cursor is over a UI element. Ability cannot be used.");
            //     return;
            // }

            cooldown = StartCoroutine(Cooldown());
        }
    }

    bool OnCooldown => cooldown != null;

    IEnumerator Cooldown()
    {
        if (OnCooldown) yield break;

        Activate();

        StartCoroutine(Animation());

        if (log) Debug.Log("Used ability: " + ability.Name);
        yield return new WaitForSeconds(ability.Cooldown);
        if (log) Debug.Log("Ability " + ability.Name + " is ready to use again.");
    }

    IEnumerator MenuCooldown()
    {
        if (OnCooldown) yield break;

        Activate();

        if (log) Debug.Log("Used menu: " + ability.Name);
        yield return new WaitForSeconds(ability.Cooldown);
        if (log) Debug.Log("Menu " + ability.Name + " is ready to use again.");

        cooldown = null;
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

        fill = cooldownImage.DOFillAmount(0f, ability.Cooldown);

        yield return new WaitForSeconds(ability.Cooldown); // Resolves slightly earlier than the cooldown for a smoother transition.

        fadeOut = availableImage.DOFade(0f, duration);
        fadeOut.OnComplete(() => availableImage.gameObject.SetActive(false));

        cooldownImage.gameObject.SetActive(false);
        cooldownImage.fillAmount = 1f;

        cooldown = null;
    }

    /// <summary>
    /// The method that will be called when the ability is used.
    /// <seealso cref="ability"/>
    /// </summary>
    void Activate()
    {
        if (!ability) throw new MissingReferenceException("No ability assigned to this slot!");
        
        ability?.Use();
    }
}