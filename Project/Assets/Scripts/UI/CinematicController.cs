using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CinematicController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    private void Start()
    {
        StartCoroutine(MenuController.Instance.FadeOutSolidPanel());
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Video End");
        MenuController.Instance.ChangeScene("Level_1");
    }
}
