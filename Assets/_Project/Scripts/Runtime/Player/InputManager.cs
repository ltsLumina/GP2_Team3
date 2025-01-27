#region
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;
#endregion

public class InputManager : MonoBehaviour
{
    [Tooltip("Key is a string input key, as defined in the Input Actions. \nE.g. '1', 'j', 'k', 'l', etc.")]
    [SerializeField] SerializedDictionary<string, GameObject> hotbarSlots;

    public Vector2 MoveInput { get; private set; }

    #region API
    public bool IsMoving { get; set; }
    #endregion

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        IsMoving = MoveInput != Vector2.zero;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        var dashController = GetComponentInParent<DashController>();
        dashController.Dash();
    }

    public void Ability(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        string key = context.control.name;
        if (hotbarSlots.TryGetValue(key, out GameObject button)) button.GetComponent<HotbarSlot>().OnSlotClicked();
    }
}
