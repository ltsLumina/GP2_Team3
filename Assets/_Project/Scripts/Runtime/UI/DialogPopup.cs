using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogPopup : MenuView
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button forceFinishButton;

    private TextTyper contentTyper;

    public override void Initialize()
    {
        closeButton.onClick.AddListener(() => MenuViewManager.HideView(this));
        forceFinishButton.onClick.AddListener(() => contentTyper.FinishEarly());

        contentTyper = new TextTyper(contentText);
    }

    public override void Hide()
    {
        base.Hide();

        contentTyper?.FinishEarly();
    }

    private Awaitable SetText(string title, string content)
    {
        titleText.text = title;

        return contentTyper
            .SetWithTotalDuration(content, 0.01f)
            .Start();
    }

    public Awaitable SetData(DialogData dialogData)
    {
        if (dialogData.Font)
            contentText.font = dialogData.Font;

        if(dialogData.FontSize > 0)
            contentText.fontSize = dialogData.FontSize;

        contentText.color = dialogData.TextColor;
        contentText.fontStyle = dialogData.FontStyle;
        contentText.alignment = dialogData.TextAlignment;

        return SetText(dialogData.Title, dialogData.Content);
    }
}
