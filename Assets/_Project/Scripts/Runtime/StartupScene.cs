using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class StartupScene : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();

        StartCoroutine(StartVideo());
    }

    private IEnumerator StartVideo()
    {
        canvasGroup.alpha = 0;

        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new(1);
        while (!videoPlayer.isPrepared)
        {
            yield return waitForSeconds;
            break;
        }
        videoPlayer.Play();

        canvasGroup.DOFade(1, 1);

        while (videoPlayer.isPlaying)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                break;
            }

            yield return null;
        }
        
        canvasGroup.alpha = 1;
        yield return canvasGroup.DOFade(0, 1).WaitForCompletion();

        SceneManager.LoadScene((int)SceneIndexes.MAIN_MENU);
    }
}
