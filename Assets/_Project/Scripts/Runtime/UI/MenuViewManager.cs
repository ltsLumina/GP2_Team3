using DG.Tweening;
using NoSlimes.Loggers;
using System.Collections.Generic;
using UnityEngine;

public class MenuViewManager : LoggerMonoBehaviour
{
    private static MenuViewManager _instance;
    private static MenuViewManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<MenuViewManager>();
            }
            return _instance;
        }
    }

    [Header("Menu Views")]
    [SerializeField] private MenuView startingView;
    [SerializeField] private List<MenuView> views = new();

    private MenuView currentView;
    private readonly Stack<MenuView> history = new();
    private List<MenuView> openViews = new();
    private List<MenuView> tempViews = new();

    private void Start()
    {
        for (int i = 0; i < views.Count; i++)
        {
            views[i].Initialize();

            views[i].Hide();
        }

        if (startingView)
        {
            Show(startingView);
        }
    }

    public static T GetView<T>() where T : MenuView
    {
        for (int i = 0; i < Instance.views.Count; i++)
        {
            if (Instance.views[i] is T tView)
            {
                return tView;
            }
        }

        return null;
    }

    public static MenuView GetCurrentView()
    {
        return Instance.currentView;
    }

    public static void Show<T>(bool rememberCurrent = true) where T : MenuView
    {
        var view = GetView<T>();
        if (view != null)
        {
            if (Instance.openViews.Contains(view))
            {
                Instance.Logger.LogWarning("View is already open.", Instance);
                return;
            }
            Show(view, rememberCurrent);

            return;
        }

    }

    public static async void Show(MenuView view, bool rememberCurrent = true)
    {
        if (Instance.currentView)
        {
            if (rememberCurrent)
            {
                Instance.history.Push(Instance.currentView);
            }

            await HideViewAsync(Instance.currentView);
        }

        Instance.openViews.Add(view);

        view.Show();
        view.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        Instance.currentView = view;
    }

    public static void ShowLast()
    {
        if (Instance.history.Count != 0)
        {
            Show(Instance.history.Pop(), false);
        }
    }

    public static void ShowAdditive<T>() where T : MenuView
    {
        var view = GetView<T>();
        if (view != null)
        {
            if (Instance.openViews.Contains(view))
            {
                Instance.Logger.LogWarning("View is already open.", Instance);
                return;
            }

            ShowAdditive(view);
            return;
        }
    }

    public static void ShowAdditive(MenuView view)
    {
        if (Instance.openViews.Contains(view))
        {
            Instance.Logger.LogWarning("View is already open.", Instance);
            return;
        }

        Instance.openViews.Add(view);
        ReorderViews();

        view.transform.localScale = Vector3.zero;
        view.Show();
        view.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public static T CloneOrOpenAdditive<T>() where T : MenuView
    {
        var view = GetView<T>();

        if(view == null)
        {
            Instance.Logger.LogError("View is null.", Instance);
            return null;
        }

        if (Instance.openViews.Contains(view))
        {
            var tempView = Instantiate(view, view.transform.parent);
            tempView.Initialize();
            Instance.tempViews.Add(tempView);
            Instance.views.Add(tempView);

            ShowAdditive(tempView);
            return tempView;
        }

        ShowAdditive(view);
        return view;
    }

    public static async void HideView(MenuView view)
    {
        await HideViewAsync(view);
    }

    public static async void HideCurrent()
    {
        if (Instance.currentView)
        {
            await HideViewAsync(Instance.currentView);
            ReorderViews();
        }
    }

    public static async void HideAll()
    {
        foreach (var view in Instance.views)
        {
            await HideViewAsync(view);
        }
    }

    public static async void HideAllExcept(MenuView view)
    {
        foreach (var openView in Instance.openViews)
        {
            if (openView != view)
            {
                await HideViewAsync(openView);
            }
        }
    }

    private static async Awaitable HideViewAsync(MenuView view)
    {
        await view.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).AsyncWaitForCompletion();
        view.Hide();

        Instance.openViews.Remove(view);

        if (Instance.tempViews.Contains(view))
        {
            Instance.tempViews.Remove(view);
            Instance.views.Remove(view);
            Destroy(view.gameObject);
        }
    }

    private static void ReorderViews()
    {
        for (int i = 0; i < Instance.openViews.Count; i++)
        {
            Instance.openViews[i].transform.SetSiblingIndex(i);
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < views.Count; i++)
        {
            views[i].Hide();
        }

        if (startingView)
        {
            startingView.Show();
        }
    }
}
