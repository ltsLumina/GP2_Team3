using UnityEngine;

[RequireComponent(typeof(Outline))]
public class DialogSign : DialogSource, IInteractable
{
    [SerializeField] private DialogDataSO dialog;

    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = true;
    }

    private void RequestDialog()
    {
        Logger.Log("DialogSign: " + "RequestDialog - " + "Raising dialog requested event", this);
        RaiseDialogRequested(dialog.DialogData);
    }

    public void OnInteract() => RequestDialog();
    public void OnHoverEnter() { }
    public void OnHoverExit() { }
}
