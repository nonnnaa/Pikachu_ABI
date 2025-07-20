using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGamePlay : UICanvas
{
    [SerializeField] private float levelTime;
    [SerializeField] private Slider timeSlider;

    [SerializeField] private int score;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    [SerializeField] private Button pauseButton;
    [SerializeField] private TimeInGameController timeInGameController;
    private int[] starsTime; // Thoi gian de nhan duoc sao
    
    
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
        score = LevelManager.Instance.GetCurrentLevelData().targetScore;
        levelTime = LevelManager.Instance.GetCurrentLevelData().time;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }
    
    private void Start()
    {
        pauseButton.onClick.AddListener(OnClickPauseButton);
        GameManager.Instance.OnGamePause += () =>
        {
            timeInGameController.enabled = false;
        };
        GameManager.Instance.OnResumeGame += () =>
        {
            timeInGameController.enabled = true;
        };
        GameManager.Instance.OnLevelStart += () =>
        {
            timeInGameController.enabled = true;
        };
        OnInit();
    }
    private void Update()
    {
        timeSlider.value =  timeInGameController.GetCurrentTimeLeft() / levelTime;
    }
    
    
}
