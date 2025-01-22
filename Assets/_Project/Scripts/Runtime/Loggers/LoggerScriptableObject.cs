using UnityEngine;

namespace NoSlimes.Loggers
{
	public class LoggerScriptableObject : ScriptableObject
	{
		[field: Header("Logger")]

		[SerializeField] private LoggerSO _logger;
		protected LoggerSO Logger
		{
			get
			{
				if(_logger == null)
				{
					Debug.LogError("Logger is not set!", this);
                    return null;
				}

				return _logger;
            }
		}
	}
}