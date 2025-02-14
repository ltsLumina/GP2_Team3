using UnityEngine;
using UnityEngine.UI;

public abstract class MultiPageMenuView : MenuView
{
    [Header("Pages")]
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    [SerializeField] protected RectTransform[] pages;

    protected int pageIndex;

    public override void Initialize()
    {
        pageIndex = 0;
        UpdatePageVisibility();

        nextPageButton.gameObject.SetActive(false);
        prevPageButton.gameObject.SetActive(false);

        if (pages.Length > 1)
        {
            nextPageButton.gameObject.SetActive(true);
            prevPageButton.gameObject.SetActive(true);

            nextPageButton.onClick.AddListener(NextPage);
            prevPageButton.onClick.AddListener(PrevPage);
        }
    }

    public override void Show()
    {
        base.Show();
        pageIndex = 0;
        UpdatePageVisibility();
    }

    protected virtual void NextPage() => SetPage((pageIndex + 1) % pages.Length);
    protected virtual void PrevPage() => SetPage((pageIndex - 1 + pages.Length) % pages.Length);

    protected virtual void SetPage(int newIndex)
    {
        if (pages == null || pages.Length == 0) return;

        pages[pageIndex].gameObject.SetActive(false);
        pageIndex = newIndex;
        pages[pageIndex].gameObject.SetActive(true);
    }

    private void UpdatePageVisibility()
    {
        if (pages == null || pages.Length == 0) return;

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].gameObject.SetActive(i == pageIndex);
        }
    }
}
