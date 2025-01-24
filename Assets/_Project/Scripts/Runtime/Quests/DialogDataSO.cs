using UnityEngine;

[CreateAssetMenu(fileName = "new Dialog Data", menuName = "Quests/DialogData")]
public class DialogDataSO : ScriptableObject
{
    [SerializeField] private DialogData dialogData;

    public DialogData DialogData => dialogData;
}
