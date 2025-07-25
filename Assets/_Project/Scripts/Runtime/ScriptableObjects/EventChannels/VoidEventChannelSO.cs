using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Void Event Channel", fileName = "new Void Event Channel")]
public class VoidEventChannelSO : ScriptableObject
{
    public UnityAction OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised.Invoke();
    }
}
