using UnityEngine;

namespace NoSlimes.Loggers
{
	public class LoggerMonoBehaviour : MonoBehaviour
	{
		[field: Header("Logger")]
		[field: SerializeField] protected LoggerSO Logger { get; private set; }
	}
}