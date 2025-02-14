using NoSlimes.Loggers;
using System;

public abstract class DialogSource : LoggerMonoBehaviour
{
    public static event Action<DialogData> OnDialogRequested;

    protected void RaiseDialogRequested(DialogData dialog)
    {
        OnDialogRequested?.Invoke(dialog);
    }
}