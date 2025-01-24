using System.Runtime.CompilerServices;
using UnityEngine;

public class DialogTester : DialogSource
{
    [SerializeField] private DialogDataSO[] dialogs;

    private void OnMouseDown()
    {
        var dialog = dialogs[Random.Range(0, dialogs.Length)];
        RaiseDialogRequested(dialog.DialogData);
    }
}
