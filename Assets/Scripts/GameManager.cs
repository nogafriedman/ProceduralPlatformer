using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CharacterVoiceSet defaultVoiceSet;
    private ScoreManager scoreManager;
    private bool isGameOver = false;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private float gameOverHoldSeconds = 1.2f;
    [SerializeField] private string endSceneName = "Ending Scene";

    [SerializeField] private GameObject buttonsGroup; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (gameOverText) gameOverText.text = string.Empty;
        if (scoreManager == null) scoreManager = FindFirstObjectByType<ScoreManager>();

    }

    void Start()
    {
        AudioManager.Instance.SetCharacterVoice(defaultVoiceSet);
    }


    // UI Character Selection
    public void SelectCharacter(CharacterVoiceSet chosen)
    {
        GameManager.Instance.defaultVoiceSet = chosen;
        AudioManager.Instance.SetCharacterVoice(chosen);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        AudioManager.Instance.PlayGameOver();
        
        if (buttonsGroup) buttonsGroup.SetActive(false);
        var player = FindFirstObjectByType<PlayerController>();
        if (player)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false;   // stops gravity and collisions until change scene
            }
            player.enabled = false;     // ignore input movement code
        }
        if (scoreManager == null)
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
        }
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText)
        {
            gameOverText.text = "GAME OVER";
        }
        StartCoroutine(GameOverFlow());
        //ReloadScene();
    }
    private IEnumerator GameOverFlow()
    {
        yield return new WaitForSeconds(gameOverHoldSeconds);
        SceneManager.LoadScene(endSceneName);   // ending Scene
    }
}

