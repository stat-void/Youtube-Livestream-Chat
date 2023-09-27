using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UIElements;
using Void.YoutubeAPI.LiveStreamChat.Messages;

namespace Void.YoutubeAPI
{
    public class YoutubeDataAPI : MonoBehaviour
    {

        private YoutubeDataAPI Instance;

        // Quota and APIKey handler
        public YoutubeKeyManager KeyManager { get; private set; }
        [SerializeField] public YoutubeAPITimer APITimer;

        private YoutubeLiveChatMessages _ytLiveChatMessages;

        /// <summary>
        /// If the API Timer is used or this event is manually invoked, 
        /// all currently active API features would be called to do their tasks.
        /// </summary>
        public static event Action OnRequest;

        /// <summary>
        /// Event used to synchronously close all YT API components when this class is destroyed.
        /// </summary>
        public static event Action OnQuit;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("There should only be one MonoBehavior instance of YoutubeDataAPI!");
                return;
            }


            KeyManager = new YoutubeKeyManager();
            YoutubeAPITimer.OnRequest += MakeCallRequest;
        }

        private void OnDestroy()
        {
            QuitCalled();
        }

        /// <summary>
        /// Create an event invoke that causes all active API components to activate.
        /// </summary>
        public void MakeCallRequest()
            => OnRequest?.Invoke();


        /// <summary> Connect to a livestream chat using the provided video ID and a stored Youtube API key. </summary>
        /// <param name="videoID"></param>
        public async Task<bool> ConnectToLivestreamChat(string videoID)
        {
            if (string.IsNullOrWhiteSpace(KeyManager.APIKey))
            {
                Debug.LogError("No API Key has been previously saved or currently provided.");
                return false;
            }

            return await ConnectToLivestreamChat(videoID, KeyManager.APIKey);

        }

        /// <summary>
        /// Connect to a livestream chat using the provided video ID and Youtube API key.
        /// </summary>
        /// <param name="videoID"></param>
        /// <param name="apiKey"></param>
        public async Task<bool> ConnectToLivestreamChat(string videoID, string apiKey)
        {
            _ytLiveChatMessages ??= new();
            (bool successfulConnect, string reason) = await _ytLiveChatMessages.Connect(videoID, apiKey);
            Debug.Log($"Connection success - {successfulConnect}; Reason - {reason}");

            return successfulConnect;
        }

        private void QuitCalled()
        {
            YoutubeAPITimer.OnRequest -= MakeCallRequest;

            KeyManager.QuitCalled();

            if (APITimer != null)
                APITimer.QuitCalled();

            OnQuit?.Invoke();

            YoutubeSaveData.SaveData();
        }
    }
}