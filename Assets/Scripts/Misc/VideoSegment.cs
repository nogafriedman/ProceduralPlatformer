using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoSegment : MonoBehaviour
{
    public VideoPlayer vp;
    public GameObject videoScreen;
    public double startSec = 0;   // where to start
    public double endSec = -1;
    public float speed = 1f;
    public float holdEndSeconds = 0f;

    void Start()
    {
        videoScreen.SetActive(true);
        vp.playbackSpeed = speed;
        vp.Prepare();
        vp.prepareCompleted += OnPrepared;
    }

    private void OnPrepared(VideoPlayer _)
    {
        vp.time = startSec;
        vp.Play();
        if (endSec > 0) { StartCoroutine(StopAt(endSec)); }
    }

    private IEnumerator StopAt(double stopTime)
    {
        while (vp.isPlaying && vp.time < stopTime) { yield return null; }
        vp.Pause();
        if (holdEndSeconds > 0f) { yield return new WaitForSecondsRealtime(holdEndSeconds); }

    }
}
