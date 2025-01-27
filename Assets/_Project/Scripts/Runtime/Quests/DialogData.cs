using TMPro;
using UnityEngine;

[System.Serializable]
public class DialogData
{
    [SerializeField] private string title;
    [TextArea(2, 4)]
    [SerializeField] private string content;
    [SerializeField] private DialogType dialogType;

    [Header("Text Settings")]
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private FontStyles fontStyle = FontStyles.Normal;
    [SerializeField] private TextAlignmentOptions textAlignment = TextAlignmentOptions.TopLeft;
    [SerializeField] private float fontSize = 36f;

    public string Title => title;
    public string Content => content;
    public DialogType DialogType => dialogType;

    public TMP_FontAsset Font => font;
    public Color TextColor => textColor;
    public FontStyles FontStyle => fontStyle;
    public TextAlignmentOptions TextAlignment => textAlignment;
    public float FontSize => Mathf.Abs(fontSize);

    public DialogData(string title, string content, DialogType dialogType)
    {
        this.title = title;
        this.content = content;
        this.dialogType = dialogType;
        this.textColor = Color.white;
        this.fontStyle = FontStyles.Normal;
        this.textAlignment = TextAlignmentOptions.Midline;
    }

    public DialogData(string title, string content, DialogType dialogType, TMP_FontAsset font, Color textColor, FontStyles fontStyle, TextAlignmentOptions textAlignment, float fontSize) : this(title, content, dialogType)
    {
        this.font = font;
        this.textColor = textColor;
        this.fontStyle = fontStyle;
        this.textAlignment = textAlignment;
        this.fontSize = fontSize;
    }
}
