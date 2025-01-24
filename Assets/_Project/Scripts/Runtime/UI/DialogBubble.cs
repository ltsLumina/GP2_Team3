using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBubble : MenuView
{
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private float yOffset = 0f;

    private TextTyper contentTyper;
    private Transform playerTransform;

    public override void Initialize()
    {
        contentTyper = new TextTyper(contentText);
    }

    public override void Show()
    {
        base.Show();
        playerTransform = GameManager.Instance.Player.transform;
    }

    private Awaitable SetText(string content)
    {
        contentText.alignment = TextAlignmentOptions.Center;

        return contentTyper
            .SetWithTotalDuration(content, 1f)
            .Start();
    }

    private void Update()
    {
        var playerScreenPos = Camera.main.WorldToScreenPoint(playerTransform.position);
        transform.position = new(playerScreenPos.x, playerScreenPos.y + yOffset);
    }

    public Awaitable SetData(DialogData dialogData)
    {
        if (dialogData.Font)
            contentText.font = dialogData.Font;

        if (dialogData.FontSize > 0)
            contentText.fontSize = dialogData.FontSize;

        contentText.color = dialogData.TextColor;
        contentText.fontStyle = dialogData.FontStyle;

        return SetText(dialogData.Content);
    }
}
