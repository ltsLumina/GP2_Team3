#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Left Click", menuName = "Abilities/Left Click Ability", order = 104)]
public class LeftClickAbility : Ability
{
    [SerializeField] int damage = 10;
    
    /// <summary>
    /// Left and Right Click abilities need additional logic to work properly.
    /// <para>A check must be performed to determine if the cursor is over a UI element.</para>
    /// <para>If the cursor is over a UI element, the ability should not be used.</para>
    /// </summary>
    public override void Use()
    {
        Debug.Log($"{Name} used!");
        
            // TODO: probably redo this logic to use the IInteractable interface, but I was having issues before.
             // Needs to have a range check.
        
        // check for enemy under cursor
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, enemyLayer))
        {
            if (hit.collider.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}