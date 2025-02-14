using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingState : State
{
    [SerializeField]
    private AlphaModulateObjects alphaModulator;

    private DashController playerDash;
    private Character character;
    private FollowMouse followMouse;

    private void Start()
    {
        playerDash = GameManager.Instance.Player.GetComponent<DashController>();
        character = GameManager.Instance.Player.GetComponent<Character>();
        followMouse = GameManager.Instance.Player.GetComponent<FollowMouse>();
        
        if (playerDash == null)
            Debug.Log("No Dash Controller");
        if (character == null)
            Debug.Log("No Character");
        if (followMouse == null)
            Debug.Log("No Follow Mouse");
    }

    private void UpdatePlayerStuff()
    {
        playerDash.UpdateDash();
        character.UpdateCharacter();
        followMouse.UpdateFollow();
    }
    
    public override void EnterState()
    {
        Logger.Log("Entering Playing State", this);
        
        if (alphaModulator == null)
            alphaModulator = Camera.main.GetComponent<AlphaModulateObjects>();

        MenuViewManager.HideAll();

        InputSystem.actions.FindActionMap("Player").Enable();
        InputSystem.actions.FindActionMap("UI").Disable();
    }
    
    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance.SwitchState<PauseState>();
        
        // this should probably run in the players update loop but it's here for now
        alphaModulator.RunAlphaModulation();

        EnemyManager.Instance.UpdateEnemyManager();
        InventorySystem.Instance.UpdateInventorySystem();

        UpdatePlayerStuff();
    }
    
    public override void ExitState()
    {
        Debug.Log("Exiting Playing State", this);
    }
}
