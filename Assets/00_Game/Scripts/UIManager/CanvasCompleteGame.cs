using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasCompleteGame : UICanvas
{
    private static readonly int Show = Animator.StringToHash("show");
    private const string lose = "Try Again!";
    private const string win = "Complete!";
    [SerializeField] private TextMeshProUGUI titleCanvasText;
    [SerializeField] private Button replayButton, nextButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Animator[] starAnimators;

    private void Start()
    {
        replayButton.onClick.AddListener(OnClickReplayButton);
        nextButton.onClick.AddListener(OnClickNextButton);
    }


    public override void Open() 
    {
        base.Open();
        OnStarShowAnim(TimeManager.Instance.CurrentStar);
        StartCoroutine(OnScoreShowAnim(BoardManager.Instance.GetCurrentScore()));
    }

    void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    IEnumerator OnScoreShowAnim(int targetScore)
    {
        float duration = 3f; 
        float elapsed = 0f;
        int startScore = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int displayScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, t));
            UpdateScore(displayScore);
            yield return null;
        }
        UpdateScore(targetScore);
    }

    

    public void OnStarShowAnim(int starNumber)
    {
        if (starNumber > 0)
        {
            titleCanvasText.text = win;
        }
        else
        {
            titleCanvasText.text = lose;
        }
        StartCoroutine(ShowStar(starNumber));
    }

    IEnumerator ShowStar(int starNumber)
    {
        for (int i = 0; i < starAnimators.Length; i++)
        {
            starAnimators[i].gameObject.SetActive(false);
        }
        //Debug.Log("Check Log IEnumerator : " + starNumber);
        for(int i = 0; i < starNumber; i++)
        {
            starAnimators[i].gameObject.SetActive(true);
            //Debug.Log("star animator" + i + ": " + starAnimators[i].gameObject.activeInHierarchy);
            starAnimators[i].ResetTrigger(Show);
            starAnimators[i].SetTrigger(Show);
            yield return new WaitForSeconds(1f);
        }
    }

    void OnClickReplayButton()
    {
        GameManager.Instance.EndLevel();
        GameManager.Instance.StartLevel(LevelManager.Instance.CurrentLevelID);
    }

    void OnClickNextButton()
    {
        GameManager.Instance.EndLevel();
        GameManager.Instance.GoToMainMenu();
    }
}
