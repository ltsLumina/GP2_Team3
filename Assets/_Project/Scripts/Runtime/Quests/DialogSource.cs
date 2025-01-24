using NoSlimes.Loggers;
using System;

public class DialogSource : LoggerMonoBehaviour
{
    public static event Action<DialogData> OnDialogRequested;

    protected void RaiseDialogRequested(DialogData dialog)
    {
        OnDialogRequested?.Invoke(dialog);
    }
}