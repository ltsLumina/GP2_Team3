using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuView : MultiPageMenuView
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button applySettingsButton;

    public override void Initialize()
    {
        base.Initialize();

        backButton.onClick.AddListener(Back);
        applySettingsButton.onClick.AddListener(() => SettingsManager.ApplySettings());
    }

    public override void Show()
    {
        base.Show();

        SettingsManager.LoadSettings();
    }

    public override void Hide() => base.Hide();

    private void Back()
    {
        SettingsManager.LoadSettings();
        MenuViewManager.ShowLast();
    }
}
