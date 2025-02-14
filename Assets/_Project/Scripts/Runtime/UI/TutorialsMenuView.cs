using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialsMenuView : MultiPageMenuView
{
    private Dictionary<int, VideoPlayer> videoPlayers = new();

    public override void Initialize()
    {
        base.Initialize();

        for (int i = 0; i < pages.Length; i++)
        {
            VideoPlayer vp = pages[i].GetComponentInChildren<VideoPlayer>();
            videoPlayers.Add(i, vp);

            vp.Prepare();
        }
    }

    public override void Show()
    {
        base.Show();

        PlayVideo(pageIndex);
    }

    public override void Hide()
    {
        base.Hide();

        StopVideos();
    }

    protected override void SetPage(int newIndex)
    {
        base.SetPage(newIndex);
        StopVideos();
        PlayVideo(newIndex);
    }

    private void PlayVideo(int index)
    {
        VideoPlayer vp = videoPlayers[index];
        vp.time = 0;

        vp.Play();
    }

    private void StopVideos()
    {
        foreach (var vp in videoPlayers.Values)
        {
            vp.Stop();
            vp.time = 0;
        }
    }
}
