using UnityEngine;
using System;
using Void.YoutubeAPI.LiveStreamChat.Messages;

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
        /// Event used to automate API message requests if this class is used.
        /// </summary>
        public static event Action OnAPIMessagesRequested;

        /// <summary> The amount of time (in seconds) needed to pass to request an API call. </summary>
        public float APIRequestInterval
        {
            get { return _apiRequestInterval; }
        }

        private float _apiRequestInterval = 3;
        private float _currentTime = 0;
        private bool _paused = true;

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
                PlayerPrefs.SetString("YT_UseInterval", _useYTInterval.ToString());
            }
        }

        private bool _useYTInterval;

        private void Awake()
        {
            YoutubeLiveChatMessages.RecommendedIntervalMilliseconds += RecommendedWaitUpdate;
            _useYTInterval = PlayerPrefs.GetString("YT_UseInterval", "False") == "True";
            _apiRequestInterval = PlayerPrefs.GetFloat("YT_RequestInterval", 3);
        }        

        private void Update()
        {
            if (!_paused)
                OnDeltaUpdate(Time.unscaledDeltaTime);
        }

        private void OnDeltaUpdate(float delta = 0)
        {
            _currentTime += delta;
            SendCurrentTime?.Invoke(_currentTime);

            if (_currentTime >= APIRequestInterval)
            {
                _currentTime -= APIRequestInterval;
                OnAPIMessagesRequested?.Invoke();
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
                PlayerPrefs.SetFloat("YT_RequestInterval", _apiRequestInterval);
                OnAPIRequestDelayChanged?.Invoke(_apiRequestInterval);
            }

            else
            {
                _apiRequestInterval = value;
                PlayerPrefs.SetFloat("YT_RequestInterval", _apiRequestInterval);
                OnAPIRequestDelayChanged?.Invoke(_apiRequestInterval);
            }
        }

        public void StartTimer() => _paused = false;
        public void PauseTimer() => _paused = true;
        public void ResetTimer() => _currentTime = 0;
    }
}