using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuView : MultiPageMenuView
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button applySettingsButton;
    [SerializeField] private Button resetSettingsButton;

    public override void Initialize()
    {
        base.Initialize();

        backButton.onClick.AddListener(Back);
        applySettingsButton.onClick.AddListener(() =>
        {
            SettingsManager.ApplySettings();
            Reload();
        });

        resetSettingsButton.onClick.AddListener(() =>
        {
            SettingsManager.ResetSettings();
            Reload();
        });
    }

    public override void Show()
    {
        base.Show();

        SettingsManager.LoadSettings();
    }

    public override void Hide()
    {
        base.Hide();

        SettingsManager.ApplySettings();
    }

    private void Back()
    {
        SettingsManager.LoadSettings();
        MenuViewManager.ShowLast();
    }

    public void Reload()
    {
        SettingsManager.LoadSettings();
        foreach(var page in pages)
        {
            var state = page.gameObject.activeSelf;

            page.gameObject.SetActive(false);
            page.gameObject.SetActive(state);
        }
    }
}
