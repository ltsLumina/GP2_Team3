using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    private static CinemachineImpulseSource impulseSource;

    void Awake() => impulseSource = GetComponent<CinemachineImpulseSource>();

    public static void Shake(float intensity = 1.0f, float duration = 0.5f)
    {
        if (impulseSource) impulseSource.GenerateImpulseWithForce(intensity);
    }
}

