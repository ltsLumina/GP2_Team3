using TMPro;
using UnityEngine;

public class TextDialog : MultiPageMenuView
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;

    public void SetText(string title, string content)
    {
        titleText.text = title;
        contentText.text = content;
    }


}
