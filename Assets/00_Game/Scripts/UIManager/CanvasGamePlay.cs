using System.Collections;
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
    [SerializeField] private Image alertImage;
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

        TimeManager.Instance.OnStarDecrease += ShowAlert;

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

    void ShowAlert(float time)
    {
        StartCoroutine(ShowAlertAnimation(time));
    }

    IEnumerator ShowAlertAnimation(float time)
    {
        float duration = time;   // Tổng thời gian nhấp nháy
        float timer = 0f;
        float t = 0f;
        bool fadingIn = true;
        Color color = alertImage.color;
        float blinkSpeed = 1.5f;
        float maxAlpha = 0.5f;

        while (timer < duration)
        {
            t += Time.deltaTime * blinkSpeed;
            timer += Time.deltaTime;

            color.a = fadingIn ? Mathf.Lerp(0f, maxAlpha, t) : Mathf.Lerp(maxAlpha, 0f, t);

            alertImage.color = color;

            if (t >= 1f)
            {
                t = 0f;
                fadingIn = !fadingIn;
            }

            yield return null;
        }

        // Kết thúc animation, reset alpha về 0
        color.a = 0f;
        alertImage.color = color;
    }

}
