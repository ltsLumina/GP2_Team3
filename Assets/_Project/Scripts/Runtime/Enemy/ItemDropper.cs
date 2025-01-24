using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    
    private float GetMonsterDropChance()
    {
        switch (GetComponent<Enemy>().Type)
        {
            case Enemy.EnemyType.Weak:
                return 0.1f;
            case Enemy.EnemyType.Normal:
                return 0.15f;
            case Enemy.EnemyType.Strong:
                return 0.2f;
        }

        // default chance
        return 0.15f;
    }
    
    // create the item and set position to the enemys position, call this upon enemy death
    public void DropItem()
    {
        if (Random.value < GetMonsterDropChance())
        {
            // create the item if the random value falls within the drop chance rate
            // we always want to create a random item
            InventorySystem.Slot itemSlot = (InventorySystem.Slot)Random.Range(1, 6);
            ItemManager.Instance.itemNames.TryGetValue(itemSlot, out string itemName);
            
            Item droppedItem = ItemManager.Instance.CreateItem(itemSlot, itemName);
            
            droppedItem.transform.position = GetComponent<Enemy>().transform.position;
        }
    }
}
