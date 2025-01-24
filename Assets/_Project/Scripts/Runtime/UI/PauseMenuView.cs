using NoSlimes;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : MenuView
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    public override void Initialize()
    {
        resumeButton.onClick.AddListener(() => MenuViewManager.HideCurrent());
        settingsButton.onClick.AddListener(() => MenuViewManager.Show<SettingsMenuView>());
        quitButton.onClick.AddListener(() => SceneLoader.Instance.LoadScene((int)SceneIndexes.MAIN_MENU));
    }
}
