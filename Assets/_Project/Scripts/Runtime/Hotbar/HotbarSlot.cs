using System.Collections;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Tween = DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions>;
using ColorTween = DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>;

[RequireComponent(typeof(Button)), ExecuteInEditMode]
public class HotbarSlot : MonoBehaviour
{
    [SerializeField] Ability ability;
    [Space(10)]
    [SerializeField, ReadOnly] Image availableImage;
    [SerializeField, ReadOnly] Image cooldownImage;
    [Space(10)]
    [SerializeField] bool log;

    void Start()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage()) return;
        
        #region Naming Utility & Validation
        if (ability)
        {
            name = $"\"{ability.Name}\" [{transform.GetSiblingIndex() + 1}]";
            GetComponent<Image>().sprite = ability.Icon;
        }
        else
        {
            name = $"Empty Slot [{transform.GetSiblingIndex()  + 1}]";
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
        
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnSlotClicked);
    }

    Coroutine cooldown;
    
    public void OnSlotClicked()
    {
        if (!ability) return;

        if (!ability.IsMenu)
        {
            if (ability.IsMouse && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Cursor is over a UI element. Ability cannot be used.");
                return;
            }
            
            if (cooldown != null) return;
        
            cooldown = StartCoroutine(Cooldown());
        }
        else
        {
            if (cooldown != null) return;
        
            cooldown = StartCoroutine(MenuCooldown());
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
    void Activate() => ability?.Use();
}