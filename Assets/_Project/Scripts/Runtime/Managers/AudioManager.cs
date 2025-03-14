using FMODUnity;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : SingletonPersistent<AudioManager>
{

    [SerializeField] FMODUnity.EventReference deathSound;
    [SerializeField] FMODUnity.EventReference hurtSound;

    [SerializeField] FMODUnity.EventReference menuMusic;
    [SerializeField] FMODUnity.EventReference gamePlayMusic;
    [SerializeField] FMODUnity.EventReference bossMusic;
    [SerializeField] FMODUnity.EventReference creditsMusic;


    private FMOD.Studio.EventInstance deathSoundInstance;

    private bool bossMusicPlaying = false;

    FMOD.Studio.EventInstance menuMusicInstance;
    FMOD.Studio.EventInstance gamePlayMusicInstance;
    FMOD.Studio.EventInstance bossMusicInstance;
    FMOD.Studio.EventInstance creditsMusicInstance;


    protected override void Awake()
    {
        base.Awake();
        menuMusicInstance = RuntimeManager.CreateInstance(menuMusic);
        gamePlayMusicInstance = RuntimeManager.CreateInstance(gamePlayMusic);
        bossMusicInstance = RuntimeManager.CreateInstance(bossMusic);
        creditsMusicInstance = RuntimeManager.CreateInstance(creditsMusic);
    }

    private void OnEnable()
    {
        GameManager.OnReady += Ready;
        MainMenuManager.OnMainMenu += PlayMenuMusic;
        GameManager.OnGameplay += PlayGamePlayMusic;
        EndCredits.OnCredits += PlayCreditMusic;
    }

    private void OnDisable()
    {
        GameManager.OnReady -= Ready;
        MainMenuManager.OnMainMenu -= PlayMenuMusic;
    }

    private void Ready()
    {
        Health.OnDeath += PlayDeathSound;
        Health.OnRevive += Revive;
        Health.OnDamageTaken += TakeDamageSound;
        Debug.Log("READY");
    }

    private void PlayDeathSound()
    {
        deathSoundInstance = RuntimeManager.CreateInstance(deathSound);
        deathSoundInstance.start();

        if (bossMusicPlaying)
        {
            bossMusicPlaying = false;
            PlayGamePlayMusic();
        }
    }

    private void TakeDamageSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(hurtSound);
    }

    private void Revive()
    {
        deathSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        deathSoundInstance.release();
    }

    private void PlayMenuMusic()
    {
        gamePlayMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bossMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        creditsMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        menuMusicInstance.start();
    }

    private void PlayGamePlayMusic()
    {
        menuMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bossMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        creditsMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        gamePlayMusicInstance.start();
    }

    public void PlayBossMusic()
    {
        if (bossMusicPlaying)
        {
            return;
        }
        menuMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        gamePlayMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        creditsMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bossMusicPlaying = true;
        bossMusicInstance.start();
    }

    private void PlayCreditMusic()
    {
        menuMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        gamePlayMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bossMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        creditsMusicInstance.start();
    }


}
