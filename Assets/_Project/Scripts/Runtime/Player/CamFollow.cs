using UnityEngine;
using VInspector;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float cameraSmoothness;
    [SerializeField] private Vector3 offset;
    
    private Vector3 _currentVelocity = Vector3.zero;

    private void Start()
    {
        transform.position = player.position + offset;
    }

    private void LateUpdate()
    {
        Vector3 targetposition = player.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetposition, ref _currentVelocity, cameraSmoothness);
    }

    [Button]
    void SetOffset()
    {
        offset = transform.position - player.position;
    }

}
