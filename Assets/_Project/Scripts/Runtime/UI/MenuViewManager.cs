using NoSlimes.Loggers;
using System.Collections.Generic;
using UnityEngine;

public class MenuViewManager : LoggerMonoBehaviour
{
    private static MenuViewManager Instance { get; set; }

    [SerializeField] private MenuView startingView;
    [SerializeField] private MenuView[] views = new MenuView[0];
    
    private MenuView currentView;
    private readonly Stack<MenuView> history = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There can only be one MenuViewManager in the scene. Destroying the new one.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i < views.Length; i++)
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
        for (int i = 0; i < Instance.views.Length; i++)
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

    public static void Show<T>(bool remember = true) where T : MenuView
    {
        for (int i = 0; i < Instance.views.Length; i++)
        {
            if (Instance.views[i] is T)
            {
                Show(Instance.views[i]);
                
                return;
            }
        }
    }

    public static void Show(MenuView view, bool remember = true)
    {
        if (Instance.currentView)
        {
            if (remember)
            {
                Instance.history.Push(Instance.currentView);
            }

            Instance.currentView.Hide();
        }

        view.Show();

        Instance.currentView = view;
    }

    public static void ShowLast()
    {
        if(Instance.history.Count != 0)
        {
            Show(Instance.history.Pop(), false);
        }
    }

    private void OnValidate()
    {
        Instance = this;

        for (int i = 0; i < views.Length; i++)
        {
            views[i].Hide();
        }

        if (startingView)
        {
            Show(startingView);
        }

        Instance = null;
    }
}
