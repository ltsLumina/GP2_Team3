using NoSlimes;
using UnityEngine;
using UnityEngine.UI;

public class DeathMenuView : MenuView
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    public override void Initialize()
    {
        restartButton.onClick.AddListener(() =>
        {
            CheckpointController.Instance.RespawnPlayer();
        });
        settingsButton.onClick.AddListener(() => MenuViewManager.Show<SettingsMenuView>());
        quitButton.onClick.AddListener(() => SceneLoader.Instance.LoadScene((int)SceneIndexes.MAIN_MENU));
    }
}
