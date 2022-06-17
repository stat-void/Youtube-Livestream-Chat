using System;
using System.Globalization;
using System.Threading;
using UnityEngine;


namespace Void.YoutubeAPI
{
    /// <summary> Class that handles the API key as well as quota usage status. </summary>
    public class YoutubeQuotaManager : MonoBehaviour
    {

        public event Action<int> OnQuotaUpdate;
        
        public int CurrentQuota { get; private set; }
        public int MaxQuota     { get; private set; }
        internal string APIKey  { get; private set; }

        private Timer _newDayTimer;


        private void Awake()
        {
            CurrentQuota = PlayerPrefs.GetInt("YT_CurrentQuota", 0);
            MaxQuota = PlayerPrefs.GetInt("YT_MaxQuota", 10000);
            APIKey = PlayerPrefs.GetString("YT_APIKey", "");

            // Check if PST midnight has arrived from quit time (Google quota reset)
            VerifyNewDayPST(PlayerPrefs.GetString("YT_QuitTime", ""));

            // Start internal timer to also reset quota during runtime when PST midnight arrives.
            StartQuotaResetTimer();
        }

       /*
       -------------------------
       -------------------------
       API key related functions
       -------------------------
       -------------------------
       */

        /// <summary> Set the API Key used for fetching messages. </summary>
        /// <param name="key"> The string received from Credentials in the Google Cloud Service. </param>
        /// <param name="save"> Should this key be remembered for future runs? </param>
        public void SetAPIKey(string key, bool save)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                if (!string.IsNullOrWhiteSpace(APIKey))
                    ResetQuota();

                APIKey = key;
            }

            if (save && !string.IsNullOrWhiteSpace(APIKey))
                SaveAPIKey(APIKey);

            if (!save)
                PlayerPrefs.DeleteKey("YT_APIKey");
        }

        /// <summary> Store the currently used API key to... somewhere (whereever PlayerPrefs data is stored on the currently used system) </summary>
        /// <param name="key"> The API key value to recall. </param>
        private void SaveAPIKey(string key) =>
            PlayerPrefs.SetString("YT_APIKey", key);
        

        /// <summary> Remove all currently saved PlayerPrefs information related to the stored API key. </summary>
        public void DeleteAPIKeyInfo()
        {
            PlayerPrefs.DeleteKey("YT_APIKey");
            PlayerPrefs.DeleteKey("YT_CurrentQuota");
            PlayerPrefs.DeleteKey("YT_MaxQuota");
        }


        /*
        -----------------------
        -----------------------
        Quota related functions
        -----------------------
        -----------------------
        */

        internal void AddQuota(int change)
        {
            if (change < 0)
            {
                Debug.LogWarning("AddQuota - Quotas can't go smaller when they're used.");
                return;
            }

            CurrentQuota += change;
            OnQuotaUpdate?.Invoke(CurrentQuota);
        }

        private void ResetQuota()
        {
            CurrentQuota = 0;
            OnQuotaUpdate?.Invoke(CurrentQuota);
        }

        internal void SetMaxQuota(int value)
        {
            MaxQuota = value;
        }


        /*
        ------------------------------------------
        ------------------------------------------
        Quota internal resetting related functions
        ------------------------------------------
        ------------------------------------------
        */

        private void VerifyNewDayPST(string quitTimeUTC)
        {
            if (!string.IsNullOrWhiteSpace(quitTimeUTC))
            {
                // Quit time was saved as UTC, so is also recovered in it.
                DateTime quitDateUTC = DateTime.ParseExact(quitTimeUTC, "yyyy-MM-ddTHH:mm:ss.FF", CultureInfo.InvariantCulture);
                DateTime quitDatePST = quitDateUTC.AddHours(-8);
                DateTime nowDatePST = DateTime.UtcNow.AddHours(-8);

                if (nowDatePST.Date != quitDatePST.Date)
                    ResetQuota();
            }
        }

        private void RuntimeNewDayPST(object state) =>
            ResetQuota();
        

        private void StartQuotaResetTimer()
        {
            if (_newDayTimer != null)
                DisposeQuotaResetTimer();

            DateTime nowDatePST = DateTime.UtcNow.AddHours(-8);
            DateTime nextDatePST = nowDatePST.Date.AddDays(1);
            TimeSpan timeToNewDayPST = nextDatePST.Subtract(nowDatePST);

            _newDayTimer = new Timer(RuntimeNewDayPST, null, timeToNewDayPST, TimeSpan.FromDays(1));
        }

        private void DisposeQuotaResetTimer()
        {
            if (_newDayTimer != null)
            {
                _newDayTimer.Dispose();
                _newDayTimer = null;
            }
        }


        /*
        ------------------------
        ------------------------
        Post-application cleanup
        ------------------------
        ------------------------
        */

        private void OnApplicationQuit()
        {
            if (!string.IsNullOrWhiteSpace(PlayerPrefs.GetString("YT_APIKey", "")))
            {
                PlayerPrefs.SetInt("YT_CurrentQuota", CurrentQuota);
                PlayerPrefs.SetInt("YT_MaxQuota", MaxQuota);
            }

            PlayerPrefs.SetString("YT_QuitTime", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.FF", CultureInfo.InvariantCulture));

            DisposeQuotaResetTimer();
        }
    }
}


