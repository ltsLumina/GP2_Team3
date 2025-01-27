using System.Collections.Generic;
using NoSlimes.Loggers;
using TMPro;
using UnityEngine;
using VInspector;

public class InventorySystem : LoggerMonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    
    public enum Slot
    {
        None = 0,
        Helmet,
        Armor,
        Legs,
        Boots,
        Weapon
    }
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    // items that are currently in the inventory
    [SerializeField] SerializedDictionary<Slot, Item> inventoryItems = new();

    // get all current items in the inventory
    public Dictionary<Slot, Item> GetInventoryItems() => inventoryItems;

    public void UpdateInventorySystem()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (InventoryMenuView.Instance.gameObject.activeSelf)
                MenuViewManager.HideView(InventoryMenuView.Instance);
            else
                MenuViewManager.Show<InventoryMenuView>();
        }
    }
    

    // add a new item to the inventory, will return true or false if adding the item was successful
    public bool AddItemToInventory(Slot itemSlot, Item item)
    {
        if (inventoryItems.TryAdd(itemSlot, item))
        {
            item.GetComponent<MeshRenderer>().enabled = false;
            item.GetComponent<BoxCollider>().enabled = false;
            OnItemEquipped(itemSlot, item);
            return true;
        }
        return false;
    }

    // remove and return the removed item
    public Item RemoveItemFromInventory(Slot itemSlot)
    {
        inventoryItems.Remove(itemSlot, out Item item);
        item.GetComponent<MeshRenderer>().enabled = true;
        item.GetComponent<BoxCollider>().enabled = true;
        TextMeshPro itemText = item.GetComponent<TextMeshPro>();
        switch (item.rarity)
        {
            case ItemManager.ItemRarity.Common: itemText.color = Color.white; break;
            case ItemManager.ItemRarity.Uncommon: itemText.color = Color.green; break;
            case ItemManager.ItemRarity.Rare: itemText.color = Color.blue; break;
            case ItemManager.ItemRarity.Epic: itemText.color = Color.yellow; break;
            case ItemManager.ItemRarity.Legendary: itemText.color = Color.red; break;
        }
        OnItemDeEquipped(itemSlot, item);
        return item;
    }

    public bool ReplaceItemInInventory(Slot itemSlot, Item item)
    {
        inventoryItems.TryGetValue(itemSlot, out Item itemInInventory);
        if (itemInInventory != null)
            RemoveItemFromInventory(itemSlot).transform.position = item.transform.position;
        return AddItemToInventory(itemSlot, item);
    }

    private void OnItemDeEquipped(Slot itemSlot, Item item)
    {
        foreach (var attribute in item.attributes)
        {
            int oldStat = GameManager.Instance.Player.GetStatValue(attribute.Key);
            GameManager.Instance.Player.DecreaseStat(attribute.Key, attribute.Value);
            int newStat = GameManager.Instance.Player.GetStatValue(attribute.Key);
            
            Debug.Log($"{attribute.Key}: {oldStat} -> {newStat}");
        }
    }

    private void OnItemEquipped(Slot itemSlot, Item item)
    {
        if (item.sprite != null)
            InventoryMenuView.Instance.PlaceInventoryItem(itemSlot, item.sprite);
        foreach (var attribute in item.attributes)
        {
            int oldStat = GameManager.Instance.Player.GetStatValue(attribute.Key);
            GameManager.Instance.Player.IncreaseStat(attribute.Key, attribute.Value);
            int newStat = GameManager.Instance.Player.GetStatValue(attribute.Key);
            
            Debug.Log($"{attribute.Key}: {oldStat} -> {newStat}");
        }
    }
}
