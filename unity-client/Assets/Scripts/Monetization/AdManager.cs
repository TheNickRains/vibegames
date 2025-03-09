using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Handles advertisement integration and monetization.
/// Supports banner ads, interstitial ads, and rewarded video ads.
/// </summary>
public class AdManager : MonoBehaviour
{
    [Header("Ad Settings")]
    [SerializeField] private bool enableAds = true;
    [SerializeField] private float interstitialCooldown = 180f; // 3 minutes
    [SerializeField] private bool showBannerAds = true;
    
    [Header("Ad IDs")]
    [SerializeField] private string androidBannerAdUnitId = "your_android_banner_ad_unit_id";
    [SerializeField] private string iosBannerAdUnitId = "your_ios_banner_ad_unit_id";
    [SerializeField] private string androidInterstitialAdUnitId = "your_android_interstitial_ad_unit_id";
    [SerializeField] private string iosInterstitialAdUnitId = "your_ios_interstitial_ad_unit_id";
    [SerializeField] private string androidRewardedAdUnitId = "your_android_rewarded_ad_unit_id";
    [SerializeField] private string iosRewardedAdUnitId = "your_ios_rewarded_ad_unit_id";
    
    // Private variables
    private bool isInitialized = false;
    private bool isBannerAdShowing = false;
    private bool isInterstitialAdLoaded = false;
    private bool isRewardedAdLoaded = false;
    private float lastInterstitialTime = 0f;
    
    // Events
    public event Action OnInterstitialAdClosed;
    public event Action<bool, string> OnRewardedAdCompleted; // bool success, string rewardType
    
    // Singleton pattern
    public static AdManager Instance { get; private set; }
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        if (enableAds)
        {
            InitializeAds();
        }
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// Initialize the advertisement SDKs
    /// </summary>
    private void InitializeAds()
    {
        // In a real implementation, you would initialize your ad SDKs here
        // For example:
        // - Google AdMob
        // - Unity Ads
        // - IronSource
        // - Facebook Audience Network
        
        Debug.Log("Ad SDKs initialized successfully");
        
        // For this demo, we'll just simulate the initialization process
        StartCoroutine(SimulateAdInitialization());
    }
    
    /// <summary>
    /// Simulate ad initialization (for demo purposes)
    /// </summary>
    private IEnumerator SimulateAdInitialization()
    {
        // Simulate initialization delay
        yield return new WaitForSeconds(1.5f);
        
        isInitialized = true;
        
        // Load initial ads
        LoadInterstitialAd();
        LoadRewardedAd();
        
        if (showBannerAds)
        {
            ShowBannerAd();
        }
        
        Debug.Log("Ads loaded successfully");
    }
    
    #endregion
    
    #region Banner Ads
    
    /// <summary>
    /// Show a banner ad
    /// </summary>
    public void ShowBannerAd()
    {
        if (!enableAds || !isInitialized) return;
        
        if (!isBannerAdShowing)
        {
            // In a real implementation, you would show your banner ad here
            // For example:
            // bannerView.Show();
            
            isBannerAdShowing = true;
            Debug.Log("Banner ad shown");
        }
    }
    
    /// <summary>
    /// Hide the current banner ad
    /// </summary>
    public void HideBannerAd()
    {
        if (!isInitialized) return;
        
        if (isBannerAdShowing)
        {
            // In a real implementation, you would hide your banner ad here
            // For example:
            // bannerView.Hide();
            
            isBannerAdShowing = false;
            Debug.Log("Banner ad hidden");
        }
    }
    
    #endregion
    
    #region Interstitial Ads
    
    /// <summary>
    /// Load an interstitial ad
    /// </summary>
    public void LoadInterstitialAd()
    {
        if (!enableAds || !isInitialized) return;
        
        // In a real implementation, you would load your interstitial ad here
        // For example:
        // interstitialAd.LoadAd(request);
        
        // Simulate ad loading delay
        StartCoroutine(SimulateInterstitialAdLoading());
    }
    
    /// <summary>
    /// Simulate interstitial ad loading (for demo purposes)
    /// </summary>
    private IEnumerator SimulateInterstitialAdLoading()
    {
        // Simulate loading delay
        yield return new WaitForSeconds(1f);
        
        isInterstitialAdLoaded = true;
        Debug.Log("Interstitial ad loaded");
    }
    
    /// <summary>
    /// Show an interstitial ad if available and cooldown has elapsed
    /// </summary>
    public bool ShowInterstitialAd()
    {
        if (!enableAds || !isInitialized || !isInterstitialAdLoaded) return false;
        
        // Check cooldown
        if (Time.time - lastInterstitialTime < interstitialCooldown)
        {
            Debug.Log("Interstitial ad cooldown not elapsed");
            return false;
        }
        
        // In a real implementation, you would show your interstitial ad here
        // For example:
        // if (interstitialAd.IsLoaded())
        // {
        //     interstitialAd.Show();
        // }
        
        // Simulate showing the ad
        StartCoroutine(SimulateInterstitialAdShowing());
        
        lastInterstitialTime = Time.time;
        isInterstitialAdLoaded = false;
        
        // Load the next interstitial ad
        LoadInterstitialAd();
        
        return true;
    }
    
    /// <summary>
    /// Simulate interstitial ad showing (for demo purposes)
    /// </summary>
    private IEnumerator SimulateInterstitialAdShowing()
    {
        Debug.Log("Interstitial ad shown");
        
        // Simulate ad duration
        yield return new WaitForSeconds(2f);
        
        // Simulate ad closed event
        if (OnInterstitialAdClosed != null)
        {
            OnInterstitialAdClosed.Invoke();
        }
        
        Debug.Log("Interstitial ad closed");
    }
    
    #endregion
    
    #region Rewarded Ads
    
    /// <summary>
    /// Load a rewarded video ad
    /// </summary>
    public void LoadRewardedAd()
    {
        if (!enableAds || !isInitialized) return;
        
        // In a real implementation, you would load your rewarded ad here
        // For example:
        // rewardedAd.LoadAd(request);
        
        // Simulate ad loading delay
        StartCoroutine(SimulateRewardedAdLoading());
    }
    
    /// <summary>
    /// Simulate rewarded ad loading (for demo purposes)
    /// </summary>
    private IEnumerator SimulateRewardedAdLoading()
    {
        // Simulate loading delay
        yield return new WaitForSeconds(1.5f);
        
        isRewardedAdLoaded = true;
        Debug.Log("Rewarded ad loaded");
    }
    
    /// <summary>
    /// Show a rewarded video ad if available
    /// </summary>
    public bool ShowRewardedAd(string rewardType = "coins")
    {
        if (!enableAds || !isInitialized || !isRewardedAdLoaded) return false;
        
        // In a real implementation, you would show your rewarded ad here
        // For example:
        // if (rewardedAd.IsLoaded())
        // {
        //     rewardedAd.Show();
        // }
        
        // Simulate showing the ad
        StartCoroutine(SimulateRewardedAdShowing(rewardType));
        
        isRewardedAdLoaded = false;
        
        // Load the next rewarded ad
        LoadRewardedAd();
        
        return true;
    }
    
    /// <summary>
    /// Simulate rewarded ad showing (for demo purposes)
    /// </summary>
    private IEnumerator SimulateRewardedAdShowing(string rewardType)
    {
        Debug.Log("Rewarded ad shown");
        
        // Simulate ad duration
        yield return new WaitForSeconds(3f);
        
        // Simulate reward earned (90% success rate)
        bool success = UnityEngine.Random.value < 0.9f;
        
        // Simulate ad closed event
        if (OnRewardedAdCompleted != null)
        {
            OnRewardedAdCompleted.Invoke(success, rewardType);
        }
        
        Debug.Log($"Rewarded ad completed. Success: {success}, Reward: {rewardType}");
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Check if an interstitial ad is ready to show
    /// </summary>
    public bool IsInterstitialAdReady()
    {
        return enableAds && isInitialized && isInterstitialAdLoaded && 
               (Time.time - lastInterstitialTime >= interstitialCooldown);
    }
    
    /// <summary>
    /// Check if a rewarded ad is ready to show
    /// </summary>
    public bool IsRewardedAdReady()
    {
        return enableAds && isInitialized && isRewardedAdLoaded;
    }
    
    /// <summary>
    /// Enable or disable ads
    /// </summary>
    public void SetAdsEnabled(bool enabled)
    {
        enableAds = enabled;
        
        if (enabled && !isInitialized)
        {
            InitializeAds();
        }
        else if (!enabled && isBannerAdShowing)
        {
            HideBannerAd();
        }
        
        PlayerPrefs.SetInt("AdsEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    #endregion
} 