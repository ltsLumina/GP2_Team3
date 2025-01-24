using NoSlimes.Loggers;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : LoggerMonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private bool enablePopupQueue = false;

    private Queue<DialogData> popupQueue = new();
    private DialogPopup currentPopup;
    private bool popupShown = false;

    private Queue<DialogData> bubbleQueue = new();
    private DialogBubble currentBubble;
    private bool bubbleShown = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        DialogSource.OnDialogRequested += ShowDialog;
    }

    private void OnDisable()
    {
        DialogSource.OnDialogRequested -= ShowDialog;
    }

    public void ShowDialog(DialogData dialogData)
    {
        switch (dialogData.DialogType)
        {
            case DialogType.Popup:
                ShowDialogPopup(dialogData);
                break;
            case DialogType.Bubble:
                ShowDialogBubble(dialogData);
                break;
        }
    }

    private void ShowDialogPopup(DialogData dialogData)
    {
        if (popupShown)
        {
            if (enablePopupQueue)
                popupQueue.Enqueue(dialogData);

            return;
        }

        currentPopup = MenuViewManager.CloneOrOpenAdditive<DialogPopup>();
        var title = dialogData.Title;
        var content = dialogData.Content;

        currentPopup.SetData(dialogData);
        //dialogPopup.SetText(title, content);
        popupShown = true;

        currentPopup.onHideMenu.RemoveAllListeners();
        currentPopup.onHideMenu.AddListener(() =>
        {
            popupShown = false;
            if (popupQueue.Count > 0)
            {
                ShowDialogPopup(popupQueue.Dequeue());
            }
        });
    }

    private async void ShowDialogBubble(DialogData dialogData)
    {
        if (bubbleShown)
        {
            bubbleQueue.Enqueue(dialogData);
            return;
        }

        currentBubble = MenuViewManager.CloneOrOpenAdditive<DialogBubble>();
        var content = dialogData.Content;

        bubbleShown = true;
        await currentBubble.SetData(dialogData);
        await Awaitable.WaitForSecondsAsync(0.5f);

        MenuViewManager.HideView(currentBubble);

        bubbleShown = false;
        if (bubbleQueue.Count > 0)
        {
            ShowDialogBubble(bubbleQueue.Dequeue());
        }
    }
}

public enum DialogType
{
    Popup,
    Bubble
}