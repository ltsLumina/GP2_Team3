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
}
