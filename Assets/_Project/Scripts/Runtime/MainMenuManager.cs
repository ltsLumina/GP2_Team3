using DG.Tweening;
using FMODUnity;
using NoSlimes;
using NoSlimes.Loggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuManager : LoggerMonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutorialsButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("Panels")]
    [SerializeField] private SettingsMenuView settingsMenu;
    [SerializeField] private CreditsMenuView creditsMenu;
    [SerializeField] private TutorialsMenuView tutorialsMenu;

    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private CanvasGroup introVideoCanvasGroup;

    [Header("Audio")]
    [SerializeField] private FMODUnity.EventReference playClickedSFX;
    [SerializeField] private FMODUnity.EventReference buttonClickedSFX;
    [SerializeField] private FMODUnity.EventReference clickyVideoSFX;

    private Dictionary<MenuView, Vector2> menuPositions = new Dictionary<MenuView, Vector2>();
    private List<MenuView> menuViews = new List<MenuView>();
    private MenuView openView;

    private VideoPlayer introVideoPlayer;

    public static event Action OnMainMenu;

    private void Start()
    {
        tutorialsMenu.Initialize();
        settingsMenu.Initialize();
        creditsMenu.Initialize();

        menuViews.Add(tutorialsMenu);
        menuViews.Add(settingsMenu);
        menuViews.Add(creditsMenu);

        foreach (var view in menuViews)
        {
            view.Hide();
            menuPositions.Add(view, (view.transform as RectTransform).localPosition);
        }

        introVideoPlayer = introVideoCanvasGroup.GetComponent<VideoPlayer>();
        introVideoPlayer.Prepare();

        SettingsManager.LoadSettings();
    }

    private void OnEnable()
    {
        mainMenuCanvasGroup.alpha = 1;
        introVideoCanvasGroup.alpha = 0;

        mainMenuCanvasGroup.interactable = true;
        OnMainMenu?.Invoke();

        playButton.onClick.AddListener(OnPlayButtonClicked);
        tutorialsButton.onClick.AddListener(OnTutorialsButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        tutorialsButton.onClick.RemoveListener(OnTutorialsButtonClicked);
        optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
        creditsButton.onClick.RemoveListener(OnCreditsButtonClicked);
        quitButton.onClick.RemoveListener(OnQuitButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        Logger.Log("Play", this);
        StartCoroutine(StartPlaySequence());

        FMODUnity.RuntimeManager.PlayOneShot(playClickedSFX);
    }

    private void OnTutorialsButtonClicked()
    {
        FMODUnity.RuntimeManager.PlayOneShot(buttonClickedSFX);
        if (openView is TutorialsMenuView)
        {
            Hide<TutorialsMenuView>();
            return;
        }
        Show<TutorialsMenuView>();
    }

    private IEnumerator StartPlaySequence()
    {
        mainMenuCanvasGroup.interactable = false;
        yield return mainMenuCanvasGroup.DOFade(0, 1f).WaitForCompletion();
        mainMenuCanvasGroup.gameObject.SetActive(false);

        introVideoCanvasGroup.gameObject.SetActive(true);
        yield return introVideoCanvasGroup.DOFade(1, 1f).WaitForCompletion();

        yield return new WaitForSeconds(.5f);

        while (!introVideoPlayer.isPrepared)
        {
            yield return null;
        }

        introVideoPlayer.Play();
        FMOD.Studio.EventInstance clickyVideo = RuntimeManager.CreateInstance(clickyVideoSFX);
        clickyVideo.start();

        while (introVideoPlayer.isPlaying)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                break;
            }
            yield return null;
        }

        yield return introVideoCanvasGroup.DOFade(0, 1f).WaitForCompletion();
        introVideoCanvasGroup.gameObject.SetActive(false);
        clickyVideo.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        SceneLoader.Instance.LoadScene((int)SceneIndexes.GAME);
    }


    private void OnOptionsButtonClicked()
    {
        FMODUnity.RuntimeManager.PlayOneShot(buttonClickedSFX);
        if (openView is SettingsMenuView)
        {
            Hide<SettingsMenuView>();
            return;
        }
        Show<SettingsMenuView>();
    }

    private void OnCreditsButtonClicked()
    {
        FMODUnity.RuntimeManager.PlayOneShot(buttonClickedSFX);
        if (openView is CreditsMenuView)
        {
            Hide<CreditsMenuView>();
            return;
        }
        Show<CreditsMenuView>();
    }

    private void OnQuitButtonClicked()
    {
        Logger.Log("Quit", this);
        Application.Quit();
        FMODUnity.RuntimeManager.PlayOneShot(buttonClickedSFX);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private async void Show<T>() where T : MenuView
    {
        if (openView is T)
            return;

        MenuView viewToShow = null;

        foreach (var view in menuViews)
        {
            if (view is not T)
            {
                if (!view.isActiveAndEnabled)
                    continue;

                await view.transform.DOLocalMove(-menuPositions[view], 0.5f).SetEase(Ease.InCubic).AsyncWaitForCompletion();
                view.Hide();
            }
            else
            {
                viewToShow = view;
            }
        }

        viewToShow.transform.localPosition = -menuPositions[viewToShow];
        viewToShow.Show();
        viewToShow.transform.DOLocalMove(menuPositions[viewToShow], 0.5f).SetEase(Ease.OutCubic);

        openView = viewToShow;
    }

    private async void Hide<T>() where T : MenuView
    {
        MenuView viewToHide = null;
        foreach (var view in menuViews)
        {
            if (view is T)
            {
                viewToHide = view;
            }
        }
        await viewToHide.transform.DOLocalMove(-menuPositions[viewToHide], 0.5f).SetEase(Ease.InCubic).AsyncWaitForCompletion();
        viewToHide.Hide();

        openView = null;
    }
}
