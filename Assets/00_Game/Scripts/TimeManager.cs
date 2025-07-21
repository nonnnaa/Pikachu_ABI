using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : SingletonMono<TimeManager>
{
    [SerializeField] private RectTransform startPos, endPos;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private float startTime;
    [SerializeField] private float timeLeft;
    [SerializeField] private float currentTime;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private List<float> starTimeGoals;
    [SerializeField] private List<GameObject> starPos;
    [SerializeField] private int currentTimeGoal;
    [SerializeField] private int currentStar;
    
    public int CurrentStar => currentStar;
    public float GetCurrentTimeLeft() => timeLeft;
    void Start()
    {
        OnInit();
        GameManager.Instance.OnLevelStart += OnInit;
    }

    private void OnInit()
    {
        currentStar = 3;
        currentTimeGoal = 0;
        currentTime = 0;
        startTime = LevelManager.Instance.GetCurrentLevelData().time;
        starTimeGoals = LevelManager.Instance.GetCurrentLevelData().starTimeGoals;
        timeSlider.value = 1;
        
        
        float distance = endPos.anchoredPosition.x - startPos.anchoredPosition.x;
        for (int i = 0; i < starTimeGoals.Count; i++)
        {
            RectTransform rectTransform = starPos[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = 
                new Vector2(startPos.anchoredPosition.x + starTimeGoals[i] / startTime * distance,rectTransform.anchoredPosition.y);
        }
    }
    void Update()
    {
        currentTime += Time.deltaTime;
        timeLeft = startTime -  currentTime;
        timeSlider.value = timeLeft / startTime;
        timeText.text = timeLeft.ToString("F0"); // lay phan nguyen cua so thuc f1 => lay 1 chu so sau dau phay
        if (currentTimeGoal < 3 && timeLeft < starTimeGoals[currentTimeGoal])
        {
            currentTimeGoal++;
            currentStar--;
        }
        if (timeLeft < 0f)
        {
            GameManager.Instance.EndLevel();
            GameManager.Instance.LoseGame();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.EndLevel();
            GameManager.Instance.WinGame();
        }
    }
}
