using UnityEngine;
using UnityEngine.UI;

public class CanvasPauseGame : UICanvas
{
    [SerializeField] private Slider musicBGSlider, musicVFXSlider;
    [SerializeField] private Button closeButton, replayButton, quitButton;
    [SerializeField] private GameObject content;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private bool canOpen;
    protected override void Awake()
    {
        base.Awake();
        rectTransform = content.GetComponent<RectTransform>();
        canvasGroup = content.GetComponent<CanvasGroup>();
        canOpen = true;
    }
    private void Start()
    {
        musicBGSlider.onValueChanged.AddListener(value =>
        {
            SoundManager.Instance.SetMusicBGVolumn(value);
        });
        musicVFXSlider.onValueChanged.AddListener(value =>
        {
            SoundManager.Instance.SetMusicVFXVolumn(value);
        });
        closeButton.onClick.AddListener(OnClickCloseButton);
        replayButton.onClick.AddListener(OnClickReplayButton);
        quitButton.onClick.AddListener(OnClickQuitButton);
    }
    public override void Open()
    {
        if (!canOpen) return;
        base.Open();
        canOpen = false;
        UIAnimator.SlideAndFade(this, rectTransform, canvasGroup,
            isShow: true,
            direction: UIAnimator.SlideDirection.Down);
    }

    public override void Close(float time)
    {
        base.Close(time);
        canOpen = true;
    }
    void OnClickCloseButton()
    {
        canOpen = true;
        UIAnimator.SlideAndFade(this, rectTransform, canvasGroup,
            isShow: false,
            direction: UIAnimator.SlideDirection.Up,
            onComplete: () =>
            {
                gameObject.SetActive(false);
                GameManager.Instance.ResumeGame();
            });
    }
    void OnClickReplayButton()
    {
        GameManager.Instance.EndLevel();
        GameManager.Instance.StartLevel(LevelManager.Instance.CurrentLevelID);
    }
    void OnClickQuitButton()
    {
        GameManager.Instance.EndLevel();
        GameManager.Instance.GoToMainMenu();
    }
}
