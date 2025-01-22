using UnityEngine;

public class PauseState : State
{
    public override void EnterState()
    {
        Logger.Log("Entering Pause State", this);
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