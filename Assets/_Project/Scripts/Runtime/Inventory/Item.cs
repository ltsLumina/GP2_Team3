using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour, IInteractable
{
    public MonoBehaviour MonoBehaviour { get; }

    private TextMeshPro textMesh;
    private BoxCollider boxCollider;

    private void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void OnInteract()
    {
        InventorySystem.Instance.ReplaceItemInInventory(itemSlot, this);
    }

    public void OnHoverEnter()
    {
        if (textMesh == null) 
            textMesh = GetComponent<TextMeshPro>();
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();
        textMesh.fontSize = 6f;
        textMesh.ForceMeshUpdate();
        boxCollider.size = textMesh.textBounds.size + new Vector3(0, 0, 0.1f);
    }

    public void OnHoverExit()
    {
        if (textMesh == null) 
            textMesh = GetComponent<TextMeshPro>();
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();
        textMesh.fontSize = 4f;
        textMesh.ForceMeshUpdate();
        boxCollider.size = textMesh.textBounds.size + new Vector3(0, 0, 0.1f);
    }
    
    public void InitializeItem(InventorySystem.Slot slot, string itemName)
    {
        id = ++_id;
        itemSlot = slot;
        Name = itemName;
        
        // setting sprite dynamically lets us spawn items without having them ...
        // pre-loaded in memory with predefined slot status and manually assigned sprites
        switch (itemSlot)
        {
            case InventorySystem.Slot.Helmet: break;
            case InventorySystem.Slot.Armor: break;
            case InventorySystem.Slot.Legs: break;
            case InventorySystem.Slot.Boots: break;
            case InventorySystem.Slot.Weapon: break;
        }
        
        RollItemRarity();
        RollAttributes();
    }
    // we should never fill out a whole Int64 with IDs
    // assuming each item created increase this ID by 1
    private static Int64 _id;
    public Int64 id;
    public string Name { get; set; } // alex: 'name' is a reserved keyword, so I changed it to 'Name'. Could be anything other than 'name' (case-sensitive)
    
    public InventorySystem.Slot itemSlot { get; private set; }

    public Sprite sprite;
    
    public ItemManager.ItemRarity rarity { get; private set; }
    
    public Dictionary<Player.Stats, int> attributes = new();

    // yes this is a mess to read but hear me out, it's 1 line
    // sets id to an ID that is not 0
    public void SetId() => id = id == 0 ? id = ++_id : id;

    private void RollItemRarity()
    {
        int randomRoll = Random.Range(0, 1001); // integer random range is non inclusive with the max value
                                                // we're also rolling to 1000 because that lets us use more accurate percentages (decimal values) without
                                                // potential floating point inaccuracy with Random.value which is a 0 - 1 value
        if (randomRoll <= 450)
            rarity = ItemManager.ItemRarity.Common;
        else if (randomRoll <= 750)
            rarity = ItemManager.ItemRarity.Uncommon;
        else if (randomRoll <= 900)
            rarity = ItemManager.ItemRarity.Rare;
        else if (randomRoll <= 975)
            rarity = ItemManager.ItemRarity.Epic;
        else if (randomRoll <= 1000)
            rarity = ItemManager.ItemRarity.Legendary;
    }

    public void RollAttributes()
    {
        // alpha will only allow for 1 attribute per item
        // weapon can roll between 2 attributes
        // higher rarity will allow for higher roll
        int attributeValue = Random.Range(10 + ((int)rarity * 10), 20 + ((int)rarity * 10));

        Player.Stats playerStat = Player.Stats.CooldownReduction; // just initialize it to any of the stats, this will be modified when running through the switch statement
        switch (itemSlot)
        {
            case InventorySystem.Slot.Helmet: playerStat = Player.Stats.CooldownReduction; break;
            case InventorySystem.Slot.Armor: playerStat = Player.Stats.Health; break;
            case InventorySystem.Slot.Legs: playerStat = Player.Stats.Health; break;
            case InventorySystem.Slot.Boots: playerStat = Player.Stats.MovementSpeed; break;
            case InventorySystem.Slot.Weapon:
                playerStat = Random.Range(0, 101) > 50 ? Player.Stats.Damage : Player.Stats.AttackSpeed;
                break;
        }
        
        attributes.Add(playerStat, attributeValue);
    }
}
