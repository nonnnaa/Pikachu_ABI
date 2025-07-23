using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGamePlay : UICanvas
{
    [SerializeField] private float levelTime;
    [SerializeField] private Slider timeSlider;

    [SerializeField] private int currentScore;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    [SerializeField] private Button pauseButton;
    [SerializeField] private TimeManager timeManager;
    private int[] starsTime; // Thoi gian de tru sao => max la 3 sao
    
    private void Start()
    {
        pauseButton.onClick.AddListener(OnClickPauseButton);
        GameManager.Instance.OnGamePause += () =>
        {
            timeManager.enabled = false;
        };
        GameManager.Instance.OnResumeGame += () =>
        {
            timeManager.enabled = true;
        };
        GameManager.Instance.OnLevelStart += () =>
        {
            timeManager.enabled = true;
            OnInit();
        };
        GameManager.Instance.OnLevelEnd += () =>
        {
            timeManager.enabled = false;
        };

        BoardManager.Instance.UpdateScore += UpdateScore;
        //OnInit();
    }
    private void Update()
    {
        timeSlider.value =  timeManager.GetCurrentTimeLeft() / levelTime;
    }
    
    void OnClickPauseButton()
    {
        GameManager.Instance.PauseGame();
    }

    public override void Open()
    {
        base.Open();
        OnInit();
    }

    void OnInit()
    {
        timeSlider.value = 1f;
        //currentScore = LevelManager.Instance.GetCurrentLevelData().targetScore;
        levelTime = LevelManager.Instance.GetCurrentLevelData().time;
        UpdateScore(0);
    }

    void UpdateScore(int score)
    {
        currentScore = score;
        scoreText.text = currentScore.ToString();
    }
}
