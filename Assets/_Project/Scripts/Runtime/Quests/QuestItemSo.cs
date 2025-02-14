using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest Item", fileName = "new Quest Item")]
public class QuestItemSO : ScriptableObject
{
    public bool CanBeCollected { get; set; } = false;
    public string ItemID => name;
}