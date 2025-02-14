using UnityEngine;

[CreateAssetMenu(fileName = "new Dialog Data", menuName = "Quests/DialogData")]
public class DialogDataSO : ScriptableObject
{
    [SerializeField] private DialogData dialogData;

    private void Awake()
    {
        dialogData = new(dialogData.Title, dialogData.Content, dialogData.DialogType, dialogData.Font, Color.white, dialogData.FontStyle, dialogData.TextAlignment, 36f);
    }

    public DialogData DialogData => dialogData;

    public void Show()
    {
        DialogManager.Instance.ShowDialog(dialogData);
    }
}
