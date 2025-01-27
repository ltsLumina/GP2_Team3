using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    
    // we only need one item name per slot since we won't focus on creating multiple items
    // if we want multiple items we can store a list of item names per slot rather than just 1 string
    public SerializedDictionary<InventorySystem.Slot, string> itemNames = new();
    
    private SerializedDictionary<Int64, Item> items = new();

    public enum ItemRarity
    {
        Common = 0,     // 45%
        Uncommon,   // 30%
        Rare,       // 15%
        Epic,       // 7.5%
        Legendary   // 2.5%
    }
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        // populate itemNames with the different names
        itemNames.Add(InventorySystem.Slot.Helmet, "Helmet");
        itemNames.Add(InventorySystem.Slot.Armor, "Armor");
        itemNames.Add(InventorySystem.Slot.Legs, "Legs");
        itemNames.Add(InventorySystem.Slot.Boots, "Boots");
        itemNames.Add(InventorySystem.Slot.Weapon, "Weapon");
    }
    
    public Item CreateItem(InventorySystem.Slot itemSlot, string itemName)
    {
        var prefab = Resources.Load<Item>("Loadable Prefabs/Item");
        Item item = Instantiate(prefab);
        
        //GameObject temp = new GameObject("Item");
        item.tag = "Item";
        item.gameObject.layer = LayerMask.NameToLayer("Interactable");
        // set rotation of the text to 45 degrees to match the camera, makes the text readable from all angles
        var rot = item.transform.rotation.eulerAngles;
        rot.x = 45;
        item.transform.rotation = Quaternion.Euler(rot);
        
        //Item newItem = temp.AddComponent<Item>();
        //newItem.InitializeItem(itemSlot, itemName);
        item.InitializeItem(itemSlot, itemName);

        #region Handled in Item.cs Awake() now.
        //Canvas itemCanvas = temp.AddComponent<Canvas>();
        //itemCanvas.renderMode = RenderMode.WorldSpace;
        
        // style the text displayed above dropped items
        //TextMeshPro itemText = temp.AddComponent<TextMeshPro>();
        // itemText.autoSizeTextContainer = true;
        // itemText.text = $"<mark=#000000>{itemName}</mark>";
        // itemText.fontSize = 4f;
        // itemText.alignment = TextAlignmentOptions.Center;
        // itemText.textWrappingMode = TextWrappingModes.NoWrap;

        // switch (newItem.rarity)
        // {
        //     case ItemRarity.Common: itemText.color = Color.white; break;
        //     case ItemRarity.Uncommon: itemText.color = Color.green; break;
        //     case ItemRarity.Rare: itemText.color = Color.blue; break;
        //     case ItemRarity.Epic: itemText.color = Color.yellow; break;
        //     case ItemRarity.Legendary: itemText.color = Color.red; break;
        // }
        
        //itemText.ForceMeshUpdate();
        
        //BoxCollider itemCollider = temp.AddComponent<BoxCollider>();
        //itemCollider.isTrigger = true; // set to isTrigger to not block any objects in the gameworld but should still be hit by raycast
        //Debug.Log(itemText.textBounds.size);
        //itemCollider.size = itemText.textBounds.size + new Vector3(0, 0, 0.1f);
        #endregion
        
        items.Add(item.id, item);
        
        return item;
    }

    public Item FindItemById(Int64 id) => items.GetValueOrDefault(id);
}
