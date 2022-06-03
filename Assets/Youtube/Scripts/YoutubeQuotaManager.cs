using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Void.YoutubeAPI
{
    public class YoutubeQuotaManager : MonoBehaviour
    {

        public event Action<int> OnQuotaUpdate;

        internal string APIKey
        {
            get { return _apiKey; }
            private set => _apiKey = value;
        }

        public int CurrentQuota { get; private set; }
        private int _maxQuota;
        private string _apiKey;

        


        private void Awake()
        {
            CurrentQuota = PlayerPrefs.GetInt("YT_CurrentQuota", 0);
            _maxQuota = PlayerPrefs.GetInt("YT_MaxQuota", 10000);
            _apiKey = PlayerPrefs.GetString("YT_APIKey", "");

            string quitTime = PlayerPrefs.GetString("YT_QuitTime", "");
            //TODO: If quitTime exists, have additional checks to verify if the quota reset time is reached.

        }

        public void SetAPIKey(string key, bool save)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                if (!string.IsNullOrWhiteSpace(_apiKey))
                {
                    CurrentQuota = 0;
                    OnQuotaUpdate?.Invoke(CurrentQuota);
                }

                _apiKey = key;
            }

            if (save && !string.IsNullOrWhiteSpace(_apiKey))
                SaveAPIKey(_apiKey);

            if (!save)
                PlayerPrefs.DeleteKey("YT_APIKey");
        }

        
        //TODO:
        /*

        For this part, 
        In some way keep track on what the latest time requested,
        and possibly have a way to reset local quota usage when UTC+0 07:00 reached? But only if saved

        Quota should be a thing that some UI visual should be able to use.

        */

        internal void AddQuota(int change)
        {
            if (change < 0)
            {
                Debug.LogWarning("Quotas can't go smaller when they're used.");
                return;
            }

            CurrentQuota += change;
            OnQuotaUpdate?.Invoke(CurrentQuota);
        }

        /// <summary> Store the currently used API key to... somewhere (where PlayerPrefs data is stored on the currently used system) </summary>
        /// <param name="key"> The API key value to recall. </param>
        private void SaveAPIKey(string key)
        {
            //TODO: Check if there is a more secure way to keep API keys stored.
            PlayerPrefs.SetString("YT_APIKey", key);
        }

        /// <summary> Remove all currently saved PlayerPrefs information. Should not affect  </summary>
        public void DeleteAllInfo()
        {
            PlayerPrefs.DeleteAll();
        }


        private void OnApplicationQuit()
        {
            if (!string.IsNullOrWhiteSpace(PlayerPrefs.GetString("YT_APIKey", "")))
            {
                PlayerPrefs.SetInt("YT_CurrentQuota", CurrentQuota);
                PlayerPrefs.SetInt("YT_MaxQuota", _maxQuota);
                PlayerPrefs.SetString("YT_QuitTime", DateTime.Now.ToString());
            }
        }
    }
}


