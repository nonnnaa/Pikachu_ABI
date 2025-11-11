
using UnityEngine;
using UnityEngine.UI;

public class CanvasMainMenu : UICanvas
{
    [SerializeField] private Slider musicBGSlider, musicVFXSlider;
    [SerializeField] private Button quitButton, settingsButton, closeButton;
    [SerializeField] private Transform content;
    
    [SerializeField] private Button rewardButton;
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
        musicBGSlider.value = SoundManager.Instance.GetMusicBGVolumn();
        musicVFXSlider.value = SoundManager.Instance.GetMusicVFXVolumn();
        quitButton.onClick.AddListener(OnClickQuitButton);
        settingsButton.onClick.AddListener(OnClickSettingsButton);
        closeButton.onClick.AddListener(OnCloseButton);
        rewardButton.onClick.AddListener((() =>
        {
            //AdsManager.Instance.LoadRewardedVideo();
            AdsManager.Instance.ShowRewardedVideo();
        }));
    }
    private void OnClickQuitButton()
    {
        Application.Quit();
    }

    private void OnClickSettingsButton()
    {
        if (content.gameObject.activeInHierarchy)
        {
            content.gameObject.SetActive(false);
        }
        else
        {
            content.gameObject.SetActive(true);
        }
    }

    private void OnCloseButton()
    {
        content.gameObject.SetActive(false);
    }
}
