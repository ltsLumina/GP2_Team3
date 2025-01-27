using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest Item", fileName = "new Quest Item")]
public class QuestItemSO : ScriptableObject
{
    public string ItemID => name;
}