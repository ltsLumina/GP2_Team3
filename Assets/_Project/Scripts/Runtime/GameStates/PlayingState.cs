using UnityEngine;

public class PlayingState : State
{
    [SerializeField]
    private AlphaModulateObjects alphaModulator;
    public override void EnterState()
    {
        Logger.Log("Entering Playing State", this);
        
        if (alphaModulator == null)
            alphaModulator = Camera.main.GetComponent<AlphaModulateObjects>();
    }
    
    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance.SwitchState<PauseState>();
        
        alphaModulator.RunAlphaModulation();
    }
    
    public override void ExitState()
    {
        Debug.Log("Exiting Playing State", this);
    }
}
