using UnityEngine;
using Unity.Services.LevelPlay;
using System; 

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    // Constants Placement Names (same DashBoard)
    private const string BANNER_PLACEMENT = "BannerTest";
    private const string INTER_PLACEMENT = "SkipAds";
    private const string REWARD_PLACEMENT = "FreeGold";

    private LevelPlayBannerAd bannerAd;
    private LevelPlayInterstitialAd interstitialAd;
    private LevelPlayRewardedAd rewardedVideoAd;

    private bool isSdkInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeSdk();
    }

    public void InitializeSdk()
    {
        if (isSdkInitialized) return;

        Debug.Log("[AdsManager] Starting LevelPlay SDK Initialization...");
        
       
        LevelPlay.ValidateIntegration(); 

        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

        LevelPlay.Init(AdConfig.AppKey);
    }

    private void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        Debug.Log($"[AdsManager] Init Success. App Key: {AdConfig.AppKey}");
        isSdkInitialized = true;
        LevelPlay.OnImpressionDataReady += ImpressionDataReadyEvent;

        CreateAndLoadAdObjects();
    }

    private void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.LogError($"[AdsManager] Init Failed with Error: {error.ErrorCode} - {error.ErrorMessage}");
    }

    private void CreateAndLoadAdObjects()
    {
        // -------------------------
        // 1. REWARDED VIDEO
        // -------------------------
        rewardedVideoAd = new LevelPlayRewardedAd(AdConfig.RewardedVideoAdUnitId);
        rewardedVideoAd.OnAdLoaded += (adInfo) => Debug.Log($"[AdsManager] Reward Loaded.");
        rewardedVideoAd.OnAdLoadFailed += (error) => Debug.LogWarning($"[AdsManager] Reward Load Failed: {error.ErrorMessage}");
        rewardedVideoAd.OnAdRewarded += (info, reward) => Debug.Log($"[AdsManager] REWARD GRANTED! {reward.Name}: {reward.Amount}");
        rewardedVideoAd.OnAdClosed += (adInfo) => { 
            Debug.Log("[AdsManager] Reward Closed. Reloading..."); 
            rewardedVideoAd.LoadAd(); // Auto Load Again After Close
        };
        rewardedVideoAd.LoadAd(); // Load First Time

        // -------------------------
        // 2. INTERSTITIAL
        // -------------------------
        interstitialAd = new LevelPlayInterstitialAd(AdConfig.InterstitalAdUnitId);
        interstitialAd.OnAdLoaded += (adInfo) => Debug.Log($"[AdsManager] Inter Loaded.");
        interstitialAd.OnAdLoadFailed += (error) => Debug.LogWarning($"[AdsManager] Inter Load Failed: {error.ErrorMessage}");
        interstitialAd.OnAdClosed += (adInfo) => {
            Debug.Log("[AdsManager] Inter Closed. Reloading...");
            interstitialAd.LoadAd(); // Auto Load Again After Close
        };
        interstitialAd.LoadAd(); // Load First Time

        // -------------------------
        // 3. BANNER 
        // -------------------------
        var bannerConfig = new LevelPlayBannerAd.Config.Builder()
            .SetPosition(LevelPlayBannerPosition.BottomCenter) 
            .SetDisplayOnLoad(true)                           
            .SetPlacementName(BANNER_PLACEMENT)               
            .Build();
        
        bannerAd = new LevelPlayBannerAd(AdConfig.BannerAdUnitId, bannerConfig);
      
        
       
        bannerAd.LoadAd();
        bannerAd.OnAdLoaded += (adInfo) => Debug.Log($"[AdsManager] Banner Loaded and Should Be Visible.");
        bannerAd.OnAdLoadFailed += (error) => 
        {
            Debug.LogWarning($"[AdsManager] Banner Load Failed: {error.ErrorMessage}");
    
         
            if (error.ErrorMessage.Contains("No Fill") || error.ErrorCode == 509) 
            {
                Debug.Log("[AdsManager] No Fill detected. Retrying load Banner in 3 seconds...");
                Invoke(nameof(LoadBannerAdDelayed), 3f);
            }
        };
    }
    private void LoadBannerAdDelayed()
    {
       
        Debug.Log("[AdsManager] Load Banner Ad Delayed");
        bannerAd.LoadAd();
    }
   

    // --- Rewarded Video ---
    public void LoadRewardedVideo()
    {
        if (!isSdkInitialized) return;
        Debug.Log("[AdsManager] Manually loading Rewarded Video.");
        rewardedVideoAd?.LoadAd();
    }

    public void ShowRewardedVideo()
    {
        if (rewardedVideoAd != null && rewardedVideoAd.IsAdReady())
        {
            Debug.Log($"[AdsManager] Showing Rewarded Video using placement: {REWARD_PLACEMENT}");
            rewardedVideoAd.ShowAd(REWARD_PLACEMENT);
        }
        else
        {
            Debug.LogWarning("[AdsManager] Rewarded Video NOT Ready. Loading...");
            rewardedVideoAd?.LoadAd(); 
        }
    }

    // --- Interstitial ---
    public void LoadInterstitial()
    {
        if (!isSdkInitialized) return;
        Debug.Log("[AdsManager] Manually loading Interstitial.");
        interstitialAd?.LoadAd();
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.IsAdReady())
        {
            Debug.Log($"[AdsManager] Showing Interstitial using placement: {INTER_PLACEMENT}");
           
            interstitialAd.ShowAd(INTER_PLACEMENT);
        }
        else
        {
            Debug.LogWarning("[AdsManager] Interstitial NOT Ready. Loading...");
            interstitialAd?.LoadAd();
        }
    }

    // --- Banner ---
    public void ShowBanner()
    {
        if (bannerAd == null || !isSdkInitialized) return;
        Debug.Log("[AdsManager] Explicitly Showing Banner.");
       
        bannerAd.LoadAd();
        bannerAd.ShowAd(); 
    }

    public void HideBanner()
    {
        if (bannerAd != null)
        {
            Debug.Log("[AdsManager] Hiding Banner.");
            bannerAd.HideAd();
        }
    }

   
    private void ImpressionDataReadyEvent(LevelPlayImpressionData impressionData)
    {
        Debug.Log($"[AdsManager] Impression Data Ready: {impressionData}");
    }
    

    private void OnDestroy()
    {
      
        bannerAd?.DestroyAd();
        interstitialAd?.DestroyAd();
        rewardedVideoAd?.DestroyAd();
    }
}