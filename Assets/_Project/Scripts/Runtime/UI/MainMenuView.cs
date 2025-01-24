using NoSlimes;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MenuView
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    public override void Initialize()
    {
        playButton.onClick.AddListener(Play);
        settingsButton.onClick.AddListener(Settings);
        quitButton.onClick.AddListener(Quit);
    }

    private void Play()
    {
        Logger.Log("Play", this);

        SceneLoader.Instance.LoadScene((int)SceneIndexes.GAME);
    }

    private void Settings()
    {
        Logger.Log("Settings", this);

        MenuViewManager.Show<SettingsMenuView>();
    }

    private void Quit()
    {
        Logger.Log("Quit", this);

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
