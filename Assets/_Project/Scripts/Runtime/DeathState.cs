using System.Collections;
using UnityEngine;

public class DeathState : State
{
    public override void EnterState()
    {
        StartCoroutine(ShowDeathScreenAfterASecondOrSoBecauseOtherwiseTheDeathAnimationWontBeShown());
    }

    IEnumerator ShowDeathScreenAfterASecondOrSoBecauseOtherwiseTheDeathAnimationWontBeShown()
    {
        yield return new WaitForSeconds(2);
        MenuViewManager.Show<DeathMenuView>();
    }
}