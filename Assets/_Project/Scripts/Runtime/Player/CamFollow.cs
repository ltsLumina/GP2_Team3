using UnityEngine;

public class CamFollow : MonoBehaviour
{
    private Vector3 m_offset;
    [SerializeField] private Transform m_Player;
    [SerializeField] private float m_cameraSmoothness;
    private Vector3 _currentVelocity = Vector3.zero;

    private void Start()
    {
        m_offset = transform.position - m_Player.position;
    }

    private void LateUpdate()
    {
        Vector3 targetposition = m_Player.position + m_offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetposition, ref _currentVelocity, m_cameraSmoothness);
    }

}
