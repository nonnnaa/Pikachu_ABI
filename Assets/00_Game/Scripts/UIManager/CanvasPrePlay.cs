using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPrePlay : UICanvas
{
    [SerializeField] private TextMeshProUGUI levelId;
    [SerializeField] private TextMeshProUGUI targetScore;
    
    [SerializeField] private Button playButton;
    [SerializeField] private Button closeButton;
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
        levelId.text = LevelManager.Instance.CurrentLevelID.ToString();

        
        LevelData levelData = LevelManager.Instance.GetCurrentLevelData();
        if(levelData != null)
            targetScore.text = levelData.targetScore.ToString();
    }

    void OnClickPlayButton()
    {
        if (!LevelManager.Instance.GetCurrentLevel().IsUnlock) return;
        GameManager.Instance.StartLevel(LevelManager.Instance.CurrentLevelID);
    }
    void OnClickCloseButton() => gameObject.SetActive(false);
}
