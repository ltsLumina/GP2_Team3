using UnityEngine;
using UnityEngine.InputSystem;

public class PauseState : State
{
    public override void EnterState()
    {
        Logger.Log("Entering Pause State", this);

        MenuViewManager.Show<PauseMenuView>();
        InputSystem.actions.FindActionMap("Player").Disable();
        InputSystem.actions.FindActionMap("UI").Enable();
    }
    
    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance.SwitchState<PlayingState>();
    }
    
    public override void ExitState()
    {
        Logger.Log("Exiting Pause State", this);
    }
}