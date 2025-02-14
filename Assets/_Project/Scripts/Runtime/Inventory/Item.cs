#region
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TMPro;
using UnityEngine;
using VInspector;

// Ensure you add the Newtonsoft.Json package to your Unity project
using Random = UnityEngine.Random;
#endregion

public class Item : MonoBehaviour, IInteractable
{
    TextMeshPro textMesh;
    BoxCollider boxCollider;

    static long _id;
    public long id;
    public string Name { get; set; }
    public InventorySystem.Slot itemSlot { get; private set; }
    public Sprite sprite;
    public ItemManager.ItemRarity Rarity { get; private set; }
    public Dictionary<Player.Stats, int> attributes = new ();

    private string originalText = string.Empty;

    public int accumulatedAttribute = 0;

    void Initialize(string itemName)
    {
        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        var itemText = GetComponent<TextMeshPro>();
        itemText.autoSizeTextContainer = true;
        itemText.text = $"<mark=#000000>{itemName}</mark>";
        originalText = itemText.text;
        itemText.fontSize = 4f;
        itemText.alignment = TextAlignmentOptions.Center;
        itemText.textWrappingMode = TextWrappingModes.NoWrap;

        var manager = ItemManager.Instance;

        if (manager.itemSprites.TryGetValue(itemSlot, out var sprites))
            if (sprites.TryGetValue(Rarity, out Sprite s))
                sprite = s;
        
        if (sprite == null)
            Debug.Log("Failed to find sprite");

        switch (Rarity)
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

        itemText.ForceMeshUpdate();

        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = itemText.textBounds.size + new Vector3(0, 0, 0.1f);
        
        Debug.Log($"Item ID: {id}");
    }

    public void OnInteract() => InventorySystem.Instance.ReplaceItemInInventory(itemSlot, this);

    public void OnHoverEnter()
    {
        if (textMesh    == null) textMesh = GetComponent<TextMeshPro>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
        textMesh.fontSize = 6f;
        textMesh.text = originalText;
        if (InventorySystem.Instance.GetInventoryItems().TryGetValue(itemSlot, out Item item))
        {
            if (accumulatedAttribute > item.accumulatedAttribute)
                textMesh.text = $"<mark=#000000><color=green>+</color> {textMesh.text} <color=green>+</color></mark>";
            else
                textMesh.text = $"<mark=#000000><color=red>-</color> {textMesh.text} <color=red>-</color></mark>";
        }
        textMesh.ForceMeshUpdate();
        boxCollider.size = textMesh.textBounds.size + new Vector3(0, 0, 0.1f);
    }

    public void OnHoverExit()
    {
        if (textMesh    == null) textMesh = GetComponent<TextMeshPro>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
        textMesh.fontSize = 4f;
        textMesh.text = originalText;
        textMesh.ForceMeshUpdate();
        boxCollider.size = textMesh.textBounds.size + new Vector3(0, 0, 0.1f);
    }

    public void InitializeItem(InventorySystem.Slot slot, string itemName)
    {
        id = ++_id;
        itemSlot = slot;
        Name = itemName;
        RollItemRarity();
        RollAttributes();
        Initialize(itemName);
    }

    public void SetId() => id = id == 0 ? id = ++_id : id;

    void RollItemRarity()
    {
        int randomRoll = Random.Range(0, 1001);

        Rarity = randomRoll switch
                 { <= 475  => ItemManager.ItemRarity.Common,
                   <= 825  => ItemManager.ItemRarity.Uncommon,
                   <= 975  => ItemManager.ItemRarity.Rare,
                   <= 1000 => ItemManager.ItemRarity.Legendary,
                   _       => Rarity };
    }

    public void RollAttributes()
    {
        var rollableStats = ItemManager.Instance.rollableStats[itemSlot];
        float rarityFraction = ((int)Rarity + 1) / 4f;
        
        int statsAmount = Mathf.FloorToInt(rollableStats.Count * rarityFraction);
        statsAmount = statsAmount == 0 ? 1 : statsAmount; // make sure statsAmount is never 0, minimum stat is 1

        int statsAssigned = 0;
        while (statsAssigned < statsAmount)
        {
            int attributeValue = Random.Range(10 + (int)Rarity * 10, 20 + (int)Rarity * 10);
            
            Player.Stats playerStat = rollableStats[Random.Range(0, rollableStats.Count)];
            
            if (attributes.ContainsKey(playerStat))
                continue;
        
            accumulatedAttribute += attributeValue;
            
            attributes.Add(playerStat, attributeValue);
            
            statsAssigned++;
        }
    }

    #region DEPRECATED SERIALIZATION
    string SerializeToJson()
    {
        var serializedItem = new SerializableItem
        { Name = Name,
          Rarity = Rarity,
          Attributes = attributes };

        return JsonConvert.SerializeObject(serializedItem, Formatting.Indented);
    }
    
#if UNITY_EDITOR
    [Button] // debug button 
#endif
    public void SaveToJsonFile()
    {
        string json = SerializeToJson();
        string timestamp = DateTime.Now.ToString("dddd-dd-MMMM @ HH.mm.ss");
        
        if (!Directory.Exists(Application.persistentDataPath + "/JSON")) Directory.CreateDirectory(Application.persistentDataPath + "/JSON");
        File.WriteAllText(Application.persistentDataPath + $"/JSON/item_{timestamp}.json", json);
    }

    // Class for serialization
    [Serializable]
    class SerializableItem
    {
        public string Name;
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemManager.ItemRarity Rarity;
        public Dictionary<Player.Stats, int> Attributes;
    }
    #endregion
}
