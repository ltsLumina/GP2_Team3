using UnityEngine;

namespace NoSlimes.Loggers
{
    [CreateAssetMenu(menuName = "Debug/Logger", fileName = "new Logger")]
    public class LoggerSO : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] private bool enableLogging;

        [Space, SerializeField] private string prefix;
        [SerializeField] private Color prefixColor = Color.white;

        public void Log(object message, Object sender)
        {
            if (enableLogging)
            {
                message = ConstructMessage(message);
                Debug.Log(message, sender);
            }
        }

        public void LogWarning(object message, Object sender)
        {
            if (enableLogging)
            {
                message = ConstructMessage(message);
                Debug.LogWarning(message, sender);
            }
        }

        private string ConstructMessage(object message)
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(prefixColor);
            return $"<color=#{colorHex}>[{prefix}]:</color> {message}";
        }
    } 
}