using System;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    [SerializeField] float mana = 100;
    [SerializeField] float maxMana = 100;

    public static event Action OnDepleted;
    
    public Material Material
    {
        get
        {
            Material source = GetComponent<Image>().material;

            var material = new Material(source);
            material.SetFloat("_ResourceAmount", Mathf.Clamp01(mana / maxMana));
            GetComponent<Image>().material = material;
            return material;
        }

        set => GetComponent<Image>().material = value;
    }

    public float CurrentMana
    {
        get
        {
            Material.SetFloat("_ResourceAmount", Mathf.Clamp01(mana / maxMana));
            return Mathf.Clamp(mana, 0, maxMana);
        }
        set
        {
            if (value <= 0)
            {
                mana = 0;
                OnDepleted?.Invoke();
            }
            else mana = value;
        }
    }

    public float MaxMana
    {
        get => maxMana;
        set
        {
            maxMana = value;
            CurrentMana = maxMana; // reset mana to max mana if max mana is changed
        }
    }
    
    void Awake()
    {
        CurrentMana = MaxMana;
        Material.SetFloat("_ResourceAmount", MaxMana);

        // ensure there is only ever one instance of the health component
        if (FindObjectsByType<Health>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1)
            Debug.LogError("There should only be one instance of the Mana component in the scene! \nIt should be on the Mana canvas object ONLY.", this);
    }

    /// <summary>
    ///     Using a positive value will increase the mana amount.
    ///     Vice versa, using a negative value will decrease the mana amount.
    ///     <example> Use 10 to increase the mana by 10. Use -10 to decrease the mana by 10. </example>
    /// </summary>
    /// <param name="value"> The value to amend the mana amount by. </param>
    public void Adjust(int value) => CurrentMana += value;
}
