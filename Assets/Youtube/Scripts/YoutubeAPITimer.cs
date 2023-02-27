using UnityEngine;
using System;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using SimpleJSON;

namespace Void.YoutubeAPI
{
    /// <summary>
    /// Handles all time based components and requests the API to fetch
    /// messages whenever the conditions (delay reaching 0 seconds) have been met.
    /// </summary>
    public class YoutubeAPITimer : MonoBehaviour
    {
        /// <summary>
        /// Event used to share consistent time state. Given float is current time before being cut down by crossing delay.
        /// Use for visuals requiring time state, comparing against delay or listening for OnAPIMessagesRequested.
        /// </summary>
        public event Action<float> SendCurrentTime;

        /// <summary>
        /// Event used to notify whenever the current API delay to invoke message request was changed.
        /// </summary>
        public event Action<float> OnAPIRequestDelayChanged;

        /// <summary>
        /// Event used to notify if any piece of code externally updated play status of this class timer.
        /// </summary>
        public event Action<bool> OnTimerPlayUpdate;

        /// <summary>
        /// Event used to automate API message requests if this class is used.
        /// </summary>
        public static event Action OnRequest;

        /// <summary> The amount of time (in seconds) needed to pass to request an API call. </summary>
        public float APIRequestInterval
        {
            get { return _apiRequestInterval; }
        }

        public bool IsPlaying { get; private set; } = false;

        private float _apiRequestInterval = 3;
        private float _currentTime = 0;

        /// <summary> Decide if this class should use the interval gotten in each chat message request, or set it manually </summary>
        public bool UseYoutubeInterval
        {
            get
            {
                return _useYTInterval;
            }

            set
            {
                _useYTInterval = value;
                _apiData["YT"]["useInterval"] = _useYTInterval.ToString();
            }
        }

        private bool _useYTInterval;

        private JSONNode _apiData;

        /// <summary> If no JSON data exists, should the application use the suggested wait time from Youtube GET requests?  </summary>
        private const bool _useYTIntervalDefault = false;

        /// <summary>  If no JSON data exists and Youtube GET request wait time is not used, how long should the delay be?  </summary>
        private const float _apiRequestIntervalDefault = 3;

        private void Awake()
        {
            YoutubeLiveChatMessages.OnIntervalUpdateMilliseconds += RecommendedWaitUpdate;
            _apiData = YoutubeData.GetData();

            // If JSON data exists, use it, otherwise set default values.
            _useYTInterval = !string.IsNullOrEmpty(_apiData["YT"]["useInterval"]) ? _apiData["YT"]["useInterval"].AsBool : _useYTIntervalDefault;
            _apiRequestInterval = !string.IsNullOrEmpty(_apiData["YT"]["requestInterval"]) ? _apiData["YT"]["requestInterval"].AsFloat : _apiRequestIntervalDefault;
        }

        private void Update()
        {
            if (IsPlaying)
                OnDeltaUpdate(Time.unscaledDeltaTime);
        }

        private void OnDeltaUpdate(float delta = 0)
        {
            _currentTime += delta;
            SendCurrentTime?.Invoke(_currentTime);

            if (_currentTime >= APIRequestInterval)
            {
                _currentTime -= APIRequestInterval;
                OnRequest?.Invoke();
            }
        }

        private void RecommendedWaitUpdate(int waitTimeMilliseconds)
        {
            if (_useYTInterval)
                SetAPIRequestInterval(waitTimeMilliseconds / 1000f);
        }

        public void SetAPIRequestInterval(float value)
        {
            if (value <= 0)
                Debug.LogError("Request delay can't be 0 or negative.");

            else if (value < 0.5f)
                Debug.LogError("Going below 0.5 seconds is wasteful on quota and volatile at fetching messages, stopping.");

            else if (value < 0.7f)
            {
                Debug.LogWarning("Setting delay below 0.7 seconds can cause duplicate messages to appear as Youtube API corrects timestamps.");
                _apiRequestInterval = value;
                _apiData["YT"]["requestInterval"] = _apiRequestInterval.ToString();
                OnAPIRequestDelayChanged?.Invoke(_apiRequestInterval);
            }

            else
            {
                _apiRequestInterval = value;
                _apiData["YT"]["requestInterval"] = _apiRequestInterval.ToString();
                OnAPIRequestDelayChanged?.Invoke(_apiRequestInterval);
            }
        }

        public void StartTimer()
        {
            IsPlaying = true;
            OnTimerPlayUpdate?.Invoke(true);
        }

        public void PauseTimer()
        {
            IsPlaying = false;
            OnTimerPlayUpdate?.Invoke(false);
        }
        public void ResetTimer() => _currentTime = 0;

        public void QuitCalled()
        {
            IsPlaying = false;
            YoutubeLiveChatMessages.OnIntervalUpdateMilliseconds -= RecommendedWaitUpdate;
        }
    }
}