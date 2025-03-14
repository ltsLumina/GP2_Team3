using UnityEngine;

public class BossMusicTrigger : MonoBehaviour
{
    private AudioManager audioManager;
    private bool active = true;

    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    private void OnEnable()
    {
        GameManager.OnReady += Ready;
    }

    private void Ready()
    {
        Health.OnDeath += EnableTrigger;
    }

    private void OnDisable()
    {
        Health.OnDeath -= EnableTrigger;
    }

    private void EnableTrigger()
    {
        active = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && active)
        {
            audioManager.PlayBossMusic();
            active = false;
        }
    }
}
