using UnityEngine;

public class BossMusicTrigger : MonoBehaviour
{
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioManager.PlayBossMusic();
        }
    }
}
