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
    private int comboJumpCount = 0; // how many multi-floor jumps in current combo
    private int comboFloorsTotal = 0; // sum of floors skipped in this combo
    private float comboTimerLeft = 0f;
    private int confirmedComboScore = 0; // sum(comboFloorsTotal^2)
    [SerializeField] private ParticleSystem comboEffect;
    [SerializeField] private Transform fxAnchor;

    private int CurrentScore => HighestFloor * 10 + confirmedComboScore;
    private bool comboActive => comboJumpCount > 0;

    // effects
    private int milestoneStep = 100;
    private int nextMilestone = 100;

    //UI
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image comboFillBar;


    private void Start()
    {
        UpdateScoreDisplay();
    }

    private void Update()
    {
        UpdateComboTimeout();
        UpdateComboUI(); 
    }

    private void UpdateComboTimeout()
    {
        comboTimerLeft -= Time.deltaTime;
        if (comboActive && comboTimerLeft <= 0f)
        {
            EndCombo();
        }
    }

    private void SetHighestFloor(int floor)
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

    private void UpdateCombo(int floor)
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

    private int GameOverScore() => CurrentScore;

    private void PlayComboStartFX()
    {
        var pos = fxAnchor ? fxAnchor.position : Vector3.zero;
        // ParticlePool.Instance.PlayBurstWorld(pos, Quaternion.identity);
        // ParticlePool.Instance.PlayBurstAttached(fxAnchor, Vector3.zero);
        comboEffect.Play();
    }

    public void ResetScore()
    {
        HighestFloor = 0;
        LastLandedFloor = 0;
        comboJumpCount = 0;
        comboFloorsTotal = 0;
        comboTimerLeft = 0f;
        confirmedComboScore = 0;
        nextMilestone = milestoneStep;
        UpdateScoreDisplay();
        UpdateComboUI();
    }

    private void UpdateScoreDisplay()
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