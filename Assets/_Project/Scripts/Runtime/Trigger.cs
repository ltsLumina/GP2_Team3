using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onTrigger;

#if UNITY_EDITOR
    [SerializeField] private Color gizmoColor = Color.green;
#endif
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent(out Player _))
        {
            Debug.Log("Triggered by player");
            onTrigger?.Invoke();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError("Trigger component requires a Collider component.");
        }
        else
        {
            GetComponent<Collider>().isTrigger = true;
        }
    }

    private void OnDrawGizmos()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
#endif
}
