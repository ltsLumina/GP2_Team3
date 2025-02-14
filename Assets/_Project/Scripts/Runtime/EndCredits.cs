using DG.Tweening;
using NoSlimes;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndCredits : MonoBehaviour
{
    [SerializeField] private RectTransform creditsRect;
    [SerializeField] private float duration = 20f;

    [SerializeField] private TMP_Text skipText;

    public static event Action OnCredits;

    private CancellationToken ct;

    private void Start()
    {
        ct = destroyCancellationToken;

        //creditsRect.localPosition = new Vector2(0, -creditsRect.sizeDelta.y);

        TweenPos();

        ct = destroyCancellationToken;
    }

    private void OnEnable()
    {
        OnCredits?.Invoke();

        skipText.gameObject.SetActive(false);
    }

    private async void TweenPos()
    {
        AllowSkip();

        while (true)
        {
            if (ct.IsCancellationRequested) return;

            creditsRect.localPosition = new Vector2(0, -Screen.height - 50);
            await creditsRect.DOMoveY(Screen.height + creditsRect.sizeDelta.y, duration).SetEase(Ease.Linear).AsyncWaitForCompletion();
        }
    }

    private async void AllowSkip()
    {
        await Awaitable.WaitForSecondsAsync(5f);
        skipText.gameObject.SetActive(true);

        while (true)
        {
            if (ct.IsCancellationRequested) return;
            if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                SceneLoader.Instance.LoadScene((int)SceneIndexes.MAIN_MENU);
            }
            await Awaitable.EndOfFrameAsync();
        }
    }
}
