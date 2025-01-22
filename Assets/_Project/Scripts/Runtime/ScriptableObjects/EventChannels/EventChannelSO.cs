using UnityEngine;
using UnityEngine.Events;

public abstract class EventChannelSO<T> : ScriptableObject
{
    public UnityAction<T> OnEventRaised;

    public virtual void RaiseEvent(T value)
    {
        OnEventRaised?.Invoke(value);
    }
}
