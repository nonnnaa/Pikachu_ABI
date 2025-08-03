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
    [SerializeField] private GameObject content;
    private RectTransform rectTransform;
    private CanvasGroup contentCanvasGroup;
    
    protected override void Awake()
    {
        base.Awake();
        rectTransform = content.GetComponent<RectTransform>();
        contentCanvasGroup = content.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        replayButton.onClick.AddListener(OnClickReplayButton);
        nextButton.onClick.AddListener(OnClickNextButton);
    }
    public override void Open() 
    {
        base.Open();
        OnShowAnim(TimeManager.Instance.CurrentStar);
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

    private AnimationCurve easeOutBack = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.7f, 1.1f), 
        new Keyframe(1f, 1f)
    );
    private AnimationCurve easeInSmooth = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private void OnShowAnim(int starNumber)
    {
        titleCanvasText.text = starNumber > 0 ? win : lose;
        UIAnimator.ScaleBounce(this, rectTransform, contentCanvasGroup,
            startScale: 0.5f,
            overshootScale: 1.2f,
            endScale: 1.0f,
            upDuration: 0.3f,
            downDuration: 0.2f,
            curveUp: easeOutBack,
            curveDown: easeInSmooth,
            onComplete: () =>
            {
                StartCoroutine(OnScoreShowAnim(BoardManager.Instance.GetCurrentScore()));
                if(starNumber > 0) StartCoroutine(ShowStar(starNumber));
            });
    }

    IEnumerator ShowStar(int starNumber)
    {
        foreach (var t in starAnimators)
        {
            t.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.1f);
        for(int i = 0; i < starNumber; i++)
        {
            starAnimators[i].gameObject.SetActive(true);
            starAnimators[i].ResetTrigger(Show);
            starAnimators[i].SetTrigger(Show);
            yield return new WaitForSeconds(1f);
        }
    }
    AnimationCurve easeOutSmooth = AnimationCurve.EaseInOut(0, 0, 1, 1);  // Co dần đều
    AnimationCurve easeInBack = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.3f, 0.9f), // co nhẹ lại trước khi tụt
        new Keyframe(1f, 1f)
    );
    
    void OnClickReplayButton()
    {
        UIAnimator.ScaleBounce(this, rectTransform, contentCanvasGroup,
            startScale: 1.0f,           // Bắt đầu từ kích thước gốc
            overshootScale: 1.1f,       // Tạo hiệu ứng co vào 1 chút
            endScale: 0.5f,             // Co về nhỏ
            upDuration: 0.2f,           // Phase co nhẹ vào (optional)
            downDuration: 0.25f,        // Phase co mạnh về nhỏ
            curveUp: easeInBack,        // Curve co nhẹ vào
            curveDown: easeOutSmooth,   // Co dần đều về nhỏ
            onComplete: () =>
            {
                GameManager.Instance.EndLevel();
                GameManager.Instance.StartLevel(LevelManager.Instance.CurrentLevelID);
            });
        
    }

    void OnClickNextButton()
    {
        UIAnimator.ScaleBounce(this, rectTransform, contentCanvasGroup,
            startScale: 1.0f,           // Bắt đầu từ kích thước gốc
            overshootScale: 1.1f,       // Tạo hiệu ứng co vào 1 chút
            endScale: 0.5f,             // Co về nhỏ
            upDuration: 0.2f,           // Phase co nhẹ vào (optional)
            downDuration: 0.25f,        // Phase co mạnh về nhỏ
            curveUp: easeInBack,        // Curve co nhẹ vào
            curveDown: easeOutSmooth,   // Co dần đều về nhỏ
            onComplete: () =>
            {
                GameManager.Instance.EndLevel();
                GameManager.Instance.GoToMainMenu();
            });
        
    }
}
