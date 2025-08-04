using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPrePlay : UICanvas
{
    [SerializeField] private TextMeshProUGUI levelId;
    [SerializeField] private TextMeshProUGUI targetScore;
    [SerializeField] private Button playButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject content;
    
    // Lấy rectTransform & canvasGroup của content
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    protected override void Awake()
    {
        base.Awake();
        rectTransform = content.GetComponent<RectTransform>();
        canvasGroup = content.GetComponent<CanvasGroup>();
    }
    
    
    void Start()
    {
        playButton.onClick.AddListener(OnClickPlayButton);
        closeButton.onClick.AddListener(OnClickCloseButton);

        GameManager.Instance.OnLevelStart += () =>
        {
            gameObject.SetActive(false);
        };
    }

    public override void Open()
    {
        base.Open();

        UIAnimator.SlideAndFade(this, rectTransform, canvasGroup, 
            isShow: true,
            direction: UIAnimator.SlideDirection.Down);
        levelId.text = LevelManager.Instance.CurrentLevelID.ToString();

        
        LevelData levelData = LevelManager.Instance.GetCurrentLevelData();
        if(levelData != null)
            targetScore.text = levelData.targetScore.ToString();
    }

    void OnClickPlayButton()
    {
        UIAnimator.SlideAndFade(this, rectTransform, canvasGroup,
            isShow: false,
            duration: 0.3f,
            direction: UIAnimator.SlideDirection.Down,
            onComplete: () =>
            {
                if (!LevelManager.Instance.GetCurrentLevel().IsUnlock) return;
                GameManager.Instance.StartLevel(LevelManager.Instance.CurrentLevelID);
            });
    }
    void OnClickCloseButton()
    {
        UIAnimator.SlideAndFade(this, rectTransform, canvasGroup,
            isShow: false,
            direction: UIAnimator.SlideDirection.Up,
            onComplete: () => gameObject.SetActive(false));
    }
}
