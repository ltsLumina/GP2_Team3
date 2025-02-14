using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBubble : MenuView
{
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private float yOffset = 0f;

    [SerializeField] private FMODUnity.EventReference textTypedSFX;

    private TextTyper contentTyper;
    private Transform playerTransform;

    public override void Initialize()
    {
        contentTyper = new TextTyper(contentText);
        contentTyper.OnTextChanged += () => FMODUnity.RuntimeManager.PlayOneShot(textTypedSFX);
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
        RectTransform rectTransform = transform as RectTransform;
        var canvas = GetComponentInParent<Canvas>();

        if (playerTransform == null || canvas == null || rectTransform == null)
            return;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, playerTransform.position);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out Vector2 localPoint))
        {
            rectTransform.localPosition = new Vector3(localPoint.x, localPoint.y + yOffset, 0);
        }
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
