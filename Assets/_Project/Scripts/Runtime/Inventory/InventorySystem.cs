#region
using System;
using System.Collections.Generic;
using System.IO;
using NoSlimes.Loggers;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;
#endregion

public class InventorySystem : LoggerMonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [SerializeField] FMODUnity.EventReference pickUpItemSFX;

    public enum Slot
    {
        None = 0,
        Helmet,
        Armor,
        Pants,
        Boots,
        Weapon,
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField]
    private SerializedDictionary<Slot, Item> inventoryItems = new ();

    public Dictionary<Slot, Item> GetInventoryItems() => inventoryItems;

    public void UpdateInventorySystem()
    {
        if (InputSystem.actions["Inventory"].WasPressedThisFrame())
        {
            if (InventoryMenuView.Instance.gameObject.activeSelf) MenuViewManager.HideView(InventoryMenuView.Instance);
            else MenuViewManager.Show<InventoryMenuView>();
        }
    }

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

    public Item RemoveItemFromInventory(Slot itemSlot)
    {
        inventoryItems.Remove(itemSlot, out Item item);
        item.GetComponent<MeshRenderer>().enabled = true;
        item.GetComponent<BoxCollider>().enabled = true;
        var itemText = item.GetComponent<TextMeshPro>();

        switch (item.Rarity)
        {
            case ItemManager.ItemRarity.Common:
                itemText.color = Color.white;
                break;

            case ItemManager.ItemRarity.Uncommon:
                itemText.color = Color.green;
                break;

            case ItemManager.ItemRarity.Rare:
                itemText.color = new Color(0, 157, 255);
                break;

            case ItemManager.ItemRarity.Legendary:
                itemText.color = Color.yellow;
                break;
        }

        OnItemDeEquipped(itemSlot, item);
        return item;
    }

    public bool ReplaceItemInInventory(Slot itemSlot, Item item)
    {
        FMODUnity.RuntimeManager.PlayOneShot(pickUpItemSFX);
        inventoryItems.TryGetValue(itemSlot, out Item itemInInventory);
        if (itemInInventory != null) RemoveItemFromInventory(itemSlot).transform.position = item.transform.position;
        return AddItemToInventory(itemSlot, item);
    }

    void OnItemDeEquipped(Slot itemSlot, Item item)
    {
        foreach (KeyValuePair<Player.Stats, int> attribute in item.attributes)
        {
            int oldStat = GameManager.Instance.Player.GetStatValue(attribute.Key);
            GameManager.Instance.Player.DecreaseStat(attribute.Key, attribute.Value);
            int newStat = GameManager.Instance.Player.GetStatValue(attribute.Key);

            Debug.Log($"{attribute.Key}: {oldStat} -> {newStat}");
        }
    }

    void OnItemEquipped(Slot itemSlot, Item item)
    {
        if (item.sprite != null) InventoryMenuView.Instance.PlaceInventoryItem(itemSlot, item.sprite);

        foreach (KeyValuePair<Player.Stats, int> attribute in item.attributes)
        {
            int oldStat = GameManager.Instance.Player.GetStatValue(attribute.Key);
            GameManager.Instance.Player.IncreaseStat(attribute.Key, attribute.Value);
            int newStat = GameManager.Instance.Player.GetStatValue(attribute.Key);

            Debug.Log($"{attribute.Key}: {oldStat} -> {newStat}");
        }
    }

    #region DEPRECATED SERIALIZATION
#if UNITY_EDITOR
    [Button]
    public void SaveInventory()
    {
        string json = SerializeInventory();
        string timestamp = DateTime.Now.ToString("dddd-dd-MMMM @ HH.mm.ss");

        if (!Directory.Exists(Application.persistentDataPath + "/JSON")) Directory.CreateDirectory(Application.persistentDataPath + "/JSON");
        File.WriteAllText(Application.persistentDataPath + $"/JSON/inventory_{timestamp}.json", json);
    }

    [Button]
    public void OpenPersistentDataPath() => EditorUtility.RevealInFinder(Application.persistentDataPath);
#endif

    public string SerializeInventory() => JsonUtility.ToJson(new SerializableInventory(inventoryItems), true);

    public void DeserializeInventory(string json)
    {
        var serializableInventory = JsonUtility.FromJson<SerializableInventory>(json);
        inventoryItems = (SerializedDictionary<Slot, Item>) serializableInventory.ToDictionary();
    }

    [Serializable]
    class SerializableInventory
    {
        public List<Slot> slots = new ();
        public List<Item> items = new ();

        public SerializableInventory(Dictionary<Slot, Item> inventoryItems)
        {
            foreach (KeyValuePair<Slot, Item> kvp in inventoryItems)
            {
                slots.Add(kvp.Key);
                items.Add(kvp.Value);
            }
        }

        public Dictionary<Slot, Item> ToDictionary()
        {
            Dictionary<Slot, Item> inventoryItems = new ();
            for (int i = 0; i < slots.Count; i++) { inventoryItems[slots[i]] = items[i]; }
            return inventoryItems;
        }
    }
    #endregion
}
