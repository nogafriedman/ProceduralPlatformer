using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public sealed class EndSequenceController : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoScreen;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject leftBubble;
    [SerializeField] private TMP_Text leftBubbleText;
    [SerializeField] private GameObject rightBubble;
    [SerializeField] private TMP_Text rightBubbleText;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject exitButton;


    [Header("Timing")]
    [SerializeField] private float delayAfterVideo = 0.15f;
    // [SerializeField] private float bubbleHold = 1.4f;
    // [SerializeField] private float delayBeforeButton = 0.3f;

    private void Awake()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (leftBubble) leftBubble.SetActive(false);
        if (rightBubble) rightBubble.SetActive(false);
        if (mainMenuButton) mainMenuButton.SetActive(false);
        if (exitButton) exitButton.SetActive(false);
    }

    private void OnEnable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(ShowDialogueSequence());
    }

    private IEnumerator ShowDialogueSequence()
    {

        yield return new WaitForSeconds(delayAfterVideo);
        if (videoScreen) videoScreen.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(true);
        if (leftBubbleText && string.IsNullOrEmpty(leftBubbleText.text))
            leftBubbleText.text = "Eli Kopterit- You did it!";
        if (rightBubbleText && string.IsNullOrEmpty(rightBubbleText.text))
            rightBubbleText.text = "WE did it!";

        if (leftBubble)
        {
            leftBubble.SetActive(true);
        }
        if (rightBubble)
        {
            rightBubble.SetActive(true);
        }
        if (mainMenuButton)
        {
            mainMenuButton.SetActive(true);
        }
        if (exitButton)
        {
            exitButton.SetActive(true);
        }


    }

}
