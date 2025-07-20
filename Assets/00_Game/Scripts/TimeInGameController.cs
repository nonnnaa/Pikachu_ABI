using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeInGameController : MonoBehaviour
{
    [SerializeField] private RectTransform startPos, endPos;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private float startTime;
    [SerializeField] private float timeLeft;
    [SerializeField] private float currentTime;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private List<float> starTimeGoals;
    [SerializeField] private List<GameObject> starPos;
    [SerializeField, Range(0, 2)] private int currentTimeGoal;
    
    public float GetCurrentTimeLeft() => timeLeft;
    void Start()
    {
        OnInit();
        GameManager.Instance.OnLevelStart += OnInit;
    }

    private void OnInit()
    {
        //Debug.Log("OnInit TimeInGameController");
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
        if (timeLeft < starTimeGoals[currentTimeGoal])
        {
            currentTimeGoal = currentTime > 2 ? 2 : currentTimeGoal++;
        }

        if (timeLeft < 0f)
        {
            GameManager.Instance.LoseGame();
        }
    }
}
