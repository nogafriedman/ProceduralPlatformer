using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private float comboTimeout = 3f;

    public int HighestFloor { get; private set; } = 0;
    public int LastLandedFloor { get; private set; } = 0;

    // combo state
    public int comboJumpCount = 0; // how many multi-floor jumps in current combo
    public int comboFloorsTotal = 0; // sum of floors skipped in this combo
    public float comboTimerLeft = 0f;
    public int confirmedComboScore = 0; // sum(comboFloorsTotal^2)
    [SerializeField] public ParticleSystem comboEffect;
    [SerializeField] private Transform fxAnchor;

    public int totalScore = 0;
    public int CurrentScore => HighestFloor * 10 + confirmedComboScore;
    private bool comboActive => comboJumpCount > 0;

    // effects
    [SerializeField] private int milestoneStep = 100;
    private int nextMilestone = 100;

    //UI
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Combo UI Elements")]
    [SerializeField] private Image comboFillBar;


    void Start()
    {
        UpdateScoreDisplay();
    }

    void Update()
    {
        UpdateComboTimeout();
        UpdateComboUI(); 
    }

    public void UpdateComboTimeout()
    {
        comboTimerLeft -= Time.deltaTime;
        if (comboActive && comboTimerLeft <= 0f)
        {
            EndCombo();
        }
    }

    public void SetHighestFloor(int floor)
    {
        if (floor > HighestFloor)
        {
            HighestFloor = floor;

            while (HighestFloor >= nextMilestone)
            {
                AudioManager.Instance?.PlayMilestone();
                nextMilestone += milestoneStep;
            }
        }
    }

    // called on each valid landing on a floor
    public void UpdateState(int floor)
    {
        if (floor > HighestFloor)
        {
            SetHighestFloor(floor);
        }

        UpdateCombo(floor);
        LastLandedFloor = floor;
        UpdateScoreDisplay();
    }

    public void UpdateCombo(int floor)
    {
        int diff = floor - LastLandedFloor;

        if (diff >= 2)
        {
            if (!comboActive)
            {                
                AudioManager.Instance?.ResetComboTierProgress();
            }

            PlayComboStartFX();

            comboJumpCount++;
            comboFloorsTotal += diff;
            comboTimerLeft = comboTimeout;

            AudioManager.Instance?.OnComboFloorsProgress(comboFloorsTotal);
            UpdateComboUI();
        }
        else
        {
            if (comboActive)
            {
                EndCombo();
            }
        }
    }

    private void ResetCombo()
    {
        comboJumpCount = 0;
        comboFloorsTotal = 0;
        comboTimerLeft = 0f;
    }

    private void EndCombo()
    {
        if (comboJumpCount >= 2)
        {
            int add = comboFloorsTotal * comboFloorsTotal;
            confirmedComboScore += add;
        }
        
        ResetCombo();

        AudioManager.Instance?.ResetComboTierProgress();
        UpdateScoreDisplay();
        UpdateComboUI();
    }

    public int GameOverScore() => CurrentScore;

    private void PlayComboStartFX()
    {
        var pos = fxAnchor ? fxAnchor.position : Vector3.zero;
        // ParticlePool.Instance.PlayBurstWorld(pos, Quaternion.identity);
        // ParticlePool.Instance.PlayBurstAttached(fxAnchor, Vector3.zero);
        comboEffect.Play();
    }

    // helper
    private void LogScore(string context = "")
    {
        string msg = $"[SCORE] {context} | Floor={HighestFloor}, Base={HighestFloor * 10}, " +
                     $"Combos={confirmedComboScore}, Total={CurrentScore}";
    }

    public void ResetScore()
    {
        HighestFloor = 0;
        LastLandedFloor = 0;
        comboJumpCount = 0;
        comboFloorsTotal = 0;
        comboTimerLeft = 0f;
        confirmedComboScore = 0;
        totalScore = 0;
        nextMilestone = milestoneStep;
        UpdateScoreDisplay();
        UpdateComboUI();
    }

    public void UpdateScoreDisplay()
    {        
        if (scoreText != null)
        {
            scoreText.text = $"{CurrentScore}";
        }
    }

    private void UpdateComboUI()
    {
        if (comboFillBar != null)
        {
            // Calculate the fill amount based on the remaining time
            // The bar will gradually empty as the timer counts down
            float fillAmount = Mathf.Clamp01(comboTimerLeft / comboTimeout);
            comboFillBar.fillAmount = fillAmount;
        }
    }
}