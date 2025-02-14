#region
using System.Collections.Generic;
using System.Linq;
using Lumina.Essentials.Attributes;
using UnityEngine;
using VInspector;
#endregion

public class Hotbar : MonoBehaviour
{
    [ReadOnly, NonReorderable]
    [SerializeField] List<HotbarSlot> slots = new ();
    
    /// <summary>
    /// Defines which level unlocks which ability
    /// </summary>
    [SerializeField] SerializedDictionary<int, HotbarSlot> levelUnlocks = new ();
    /// <summary>
    /// The level at which the dash ability is unlocked
    /// </summary>
    [SerializeField] int dashLevelUnlock = 1;

    public List<HotbarSlot> Slots => slots;

    void Start()
    {
        if (transform != transform.root)
        {
            Debug.LogError("Hotbar must be a root object!", this);
        }
        
        slots = GetComponentsInChildren<HotbarSlot>().ToList();
        
        // sort the slots by their sibling index. Ensures the slots are in the correct order
        slots.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));


        
        UnlockAbility(); // unlock initial abilities
    }

    void OnEnable() => Experience.OnLevelUp += UnlockAbility;
    void OnDisable() => Experience.OnLevelUp -= UnlockAbility;

    void UnlockAbility()
    {

        slots[0].Unlock();

        if (levelUnlocks.Count < slots.Count || levelUnlocks.Count == 0)
        {
            Debug.LogError("Level unlocks are not set up correctly!" + "\nHotbar will use the Default values. (Level 1 = Ability 1, etc.)", this);
            levelUnlocks.Clear();
            for (int i = 0; i < slots.Count; i++)
            {
                levelUnlocks.Add(i + 1, slots[i]); // +1 because levels start at 1
            }
        }

        if (levelUnlocks.TryGetValue(Experience.Level, out HotbarSlot slot)) 
            slot.Unlock();

        if (Experience.Level >= dashLevelUnlock)
        {
            GameManager.Instance.Player.DashController.Unlock();
        }
    }
    
    public static void UnlockDash()
    {
        GameManager.Instance.Player.DashController.Unlock();
    }
}
