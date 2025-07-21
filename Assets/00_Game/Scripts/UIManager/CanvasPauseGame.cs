using UnityEngine;
using UnityEngine.UI;

public class CanvasPauseGame : UICanvas
{
    [SerializeField] private Slider musicBGSlider, musicVFXSlider;
    [SerializeField] private Button closeButton, replayButton, quitButton;
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
    void OnClickCloseButton()
    {
        gameObject.SetActive(false);
        GameManager.Instance.ResumeGame();
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
