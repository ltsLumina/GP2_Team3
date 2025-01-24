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
        
        // this should probably run in the players update loop but it's here for now
        alphaModulator.RunAlphaModulation();

        EnemyManager.Instance.UpdateEnemyManager();
        InventorySystem.Instance.UpdateInventorySystem();
    }
    
    public override void ExitState()
    {
        Debug.Log("Exiting Playing State", this);
    }
}
