using UnityEngine;

[RequireComponent(typeof(Outline))]
public class DialogSign : DialogSource, IInteractable
{
    [SerializeField] private DialogDataSO dialog;

    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    private void RequestDialog()
    {
        Logger.Log("DialogSign: " + "RequestDialog - " + "Raising dialog requested event", this);
        RaiseDialogRequested(dialog.DialogData);
    }

    public void OnInteract() => RequestDialog();
    public void OnHoverEnter() => outline.enabled = true;
    public void OnHoverExit() => outline.enabled = false;
}
