using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuView : MenuView
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button applySettingsButton;

    [Header("Pages")]
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    [SerializeField] private GameObject[] pages;

    private int pageIndex;

    public override void Initialize()
    {
        backButton.onClick.AddListener(Back);
        applySettingsButton.onClick.AddListener(() => SettingsManager.ApplySettings());
        nextPageButton.onClick.AddListener(NextPage);
        prevPageButton.onClick.AddListener(PrevPage);

        pageIndex = 0;
    }

    public override void Show()
    {
        base.Show();

        SettingsManager.LoadSettings();

        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }

        pageIndex = 0;
        pages[pageIndex].SetActive(true);
    }

    public override void Hide() => base.Hide();

    private void NextPage()
    {
        pages[pageIndex].SetActive(false);

        if (pageIndex < pages.Length - 1)
        {
            pageIndex++;
        }
        else
        {
            pageIndex = 0;
        }
        pages[pageIndex].SetActive(true);

    }

    private void PrevPage()
    {
        pages[pageIndex].SetActive(false);
        if (pageIndex > 0)
        {
            pageIndex--;
        }
        else
        {
            pageIndex = pages.Length - 1;
        }
        pages[pageIndex].SetActive(true);
    }

    private void Back()
    {
        SettingsManager.LoadSettings();
        MenuViewManager.ShowLast();
    }
}
