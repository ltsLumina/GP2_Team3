#region
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    GameObject abilitiesHeader;
    
    public Vector2 MoveInput {get; private set;}
    public bool IsMoving => MoveInput != Vector2.zero;

    Player player;

    void Awake() => player = GetComponentInParent<Player>();

    void Start()
    {
        if (!abilitiesHeader) abilitiesHeader = GameObject.Find("Ability Buttons");
    }
    
    bool pressedJ;

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            pressedJ = true;
            
            if (Input.GetKeyDown(KeyCode.O) && pressedJ)
            {
                pressedJ = false;
                Animator anim = player.Animator;
                anim.SetBool("funny", !anim.GetBool("funny"));
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        if (!context.performed) return;
        
        player.DashController.Dash();
    }

    public void Ability(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        if (!context.performed) return;

        // use the ability based on the index
        int index = context.action.GetBindingIndexForControl(context.control);
        abilitiesHeader.transform.GetChild(index).GetComponent<HotbarSlot>().OnSlotClicked();
    }
}
