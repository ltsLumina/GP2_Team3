using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDropper : MonoBehaviour
{
    Enemy enemy;

    void Awake() => enemy = GetComponent<Enemy>();

    float MonsterDropChance
    {
        get
        {
            switch (enemy.Type)
            {
                case Enemy.EnemyType.Weak:
                    return 0.1f;

                case Enemy.EnemyType.Normal:
                    return 0.15f;

                case Enemy.EnemyType.Strong:
                    return 0.2f;
                
                case Enemy.EnemyType.DEBUG:
                    return 1f;
            }

            // default chance
            return 0.15f;
        }
    }

    // create the item and set position to the enemys position, call this upon enemy death
    public void DropItem()
    {
        if (Random.value <= MonsterDropChance)
        {
            // create the item if the random value falls within the drop chance rate
            // we always want to create a random item
            InventorySystem.Slot itemSlot = (InventorySystem.Slot)Random.Range(1, 6);
            ItemManager.Instance.itemNames.TryGetValue(itemSlot, out string itemName);
            
            Item droppedItem = ItemManager.Instance.CreateItem(itemSlot, itemName);

            Vector3 position = transform.position;
            
            Collider[] hits = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Interactable"));
            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent(out Item _))
                {
                    position.x += 1f;
                    position.z += 1f;
                    break;
                }
            }
            
            position.y = 0.35f;
            droppedItem.transform.position = position;
        }
    }
}
