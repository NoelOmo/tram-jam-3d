using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NINESOFT.CORE;

using GoogleMobileAds;
using GoogleMobileAds.Api;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D//.ASSET NAME--/// <<---------------
{
    public class CompleteGameADManager : Manager<CompleteGameADManager>
    {
        public CompleteGameConfigration _config;

        private BannerView _bannerView;
        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;

        private new void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            MobileAds.Initialize(initStatus => { });
            LoadBannerAd();
        }

        private void ShowInter()
        {
            Invoke(nameof(ShowInterstitialAd), .1f);
        }

        public void CreateBannerView()
        {
            Debug.Log("Creating banner view");

            // If we already have a banner, destroy the old one.
            if (_bannerView != null)
            {
                DestroyBannerView();
            }

            // Create a 320x50 banner at top of the screen
            _bannerView = new BannerView(_config.Admob_Android_Banner, AdSize.Banner, AdPosition.Bottom);
        }

        public void LoadBannerAd()
        {
            // create an instance of a banner view first.
            if (_bannerView == null)
            {
                CreateBannerView();
            }

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            Debug.Log("Loading banner ad.");
            _bannerView.LoadAd(adRequest);
        }

        public void DestroyBannerView()
        {
            if (_bannerView != null)
            {
                Debug.Log("Destroying banner view.");
                _bannerView.Destroy();
                _bannerView = null;
            }
        }

        public void LoadInterstitialAd()
        {
            // Clean up the old ad before loading a new one.
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            Debug.Log("Loading the interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            InterstitialAd.Load(_config.Admob_Android_Interstitial, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Interstitial ad loaded with response : "
                              + ad.GetResponseInfo());

                    _interstitialAd = ad;
                });
        }

        public void ShowInterstitialAd()
        {
            LoadInterstitialAd();
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                _interstitialAd.Show();
            }
            else
            {
                Debug.LogError("Interstitial ad is not ready yet.");
            }
        }

        public void LoadRewardedAd()
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            Debug.Log("Loading the rewarded ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            RewardedAd.Load(_config.Admob_Android_RewardedVideo, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Rewarded ad loaded with response : "
                              + ad.GetResponseInfo());

                    _rewardedAd = ad;
                });
        }

        private void ShowRewardedAd(Action onRewardReceived)
        {
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                _rewardedAd.Show((Reward reward) =>
                {
                    onRewardReceived?.Invoke();
                });
            }
        }


        public void PlayRw(Action onRewardReceived, Action onFailed = null)
        {
            StartCoroutine(PlayRewardedCoroutine(onRewardReceived, onFailed));
        }

        private IEnumerator PlayRewardedCoroutine(Action onRewardReceived, Action onFailed)
        {
            LoadRewardedAd();

            float timeout = 10f;
            while ((_rewardedAd == null || !_rewardedAd.CanShowAd()) && timeout > 0f)
            {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }

            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                bool rewarded = false;
                bool closed = false;

                void OnAdClosed()
                {
                    closed = true;
                    if (rewarded)
                        onRewardReceived?.Invoke();
                    else
                        onFailed?.Invoke();
                }

                _rewardedAd.OnAdFullScreenContentClosed += OnAdClosed;
                _rewardedAd.Show((Reward reward) =>
                {
                    rewarded = true;
                });

                timeout = 120f;
                while (!closed && timeout > 0f)
                {
                    timeout -= Time.unscaledDeltaTime;
                    yield return null;
                }

                _rewardedAd.OnAdFullScreenContentClosed -= OnAdClosed;

                if (!closed)
                    onFailed?.Invoke();
            }
            else
            {
                onFailed?.Invoke();
            }
        }

        public void WatchAdsForGem()
        {
            PlayRw(() =>
            {
                DataManager.Instance.AddGem(20);
                UIManager.Instance.ShowMessageBox("You earned 20 gems!");
            });
        }

    }
}