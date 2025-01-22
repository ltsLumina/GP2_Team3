using UnityEngine;

namespace NoSlimes
{
    [CreateAssetMenu(menuName = "Misc/Game Tips", fileName = "new Game Tips")]
    public class TipsSO : ScriptableObject
    {
        [SerializeField, TextArea(2, 5)] private string[] gameTips;

        public string GetRandomTip() => gameTips[Random.Range(0, gameTips.Length)];
    }
}