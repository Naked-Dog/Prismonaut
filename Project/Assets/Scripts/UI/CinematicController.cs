using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        MenuController.Instance.ChangeScene("Level_1");
    }

    private void skipCinematic()
    {
        videoPlayer.Stop();
        MenuController.Instance.ChangeScene("Level_1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            skipCinematic();
        }
    }
}
