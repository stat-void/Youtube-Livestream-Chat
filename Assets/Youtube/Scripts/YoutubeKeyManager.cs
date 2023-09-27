using SimpleJSON;
using System;
using System.Globalization;
using System.Threading;

namespace Void.YoutubeAPI
{
    /// <summary> Class that handles the API key as well as quota usage status. </summary>
    public class YoutubeKeyManager
    {
        private static YoutubeKeyManager Instance;

        public event Action<int> OnQuotaUpdate;
        
        public int CurrentQuota { get; private set; }
        public int MaxQuota     { get; private set; }
        internal string APIKey  { get; private set; }

        private Timer _newDayTimer;
        private readonly JSONNode _apiData;

        public YoutubeKeyManager()
        {
            if (Instance == null)
                Instance = this;
            else
                throw new ArgumentException("There should not be more than 1 instance of YoutubeKeyManager. Have you only instantiated one YoutubeDataAPI?");

            _apiData = YoutubeSaveData.GetData();

            // If JSON data exists, use it, otherwise set default values.
            CurrentQuota = !string.IsNullOrEmpty(_apiData["YT"]["currentQuota"]) ? _apiData["YT"]["currentQuota"].AsInt : 0;
            MaxQuota =  !string.IsNullOrEmpty(_apiData["YT"]["maxQuota"]) ? _apiData["YT"]["maxQuota"].AsInt : 10000;
            APIKey = _apiData["YT"]["apiKey"];

            // Check if PST midnight has arrived from quit time (Google quota reset)
            VerifyNewDayPST(_apiData["YT"]["quitTime"]);

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
                _apiData["YT"]["apiKey"] = "";
        }

        /// <summary> Store the currently used API key to the data JSON file. </summary>
        /// <param name="key"> The API key value to recall. </param>
        private void SaveAPIKey(string key) =>
            _apiData["YT"]["apiKey"] = key;

        /// <summary> Remove all currently saved information related to the stored API key. </summary>
        public void DeleteAPIKeyInfo()
        {
            _apiData["YT"]["apiKey"] = "";
            _apiData["YT"]["currentQuota"] = "";
            _apiData["YT"]["maxQuota"] = "";
        }


        /*
        -----------------------
        -----------------------
        Quota related functions
        -----------------------
        -----------------------
        */

        public static void AddQuota(int change)
        {
            if (change < 0)
                return;

            Instance.CurrentQuota += change;
            Instance.OnQuotaUpdate?.Invoke(Instance.CurrentQuota);
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

        public void QuitCalled()
        {
            if (!string.IsNullOrWhiteSpace(_apiData["YT"]["apiKey"]))
            {
                _apiData["YT"]["currentQuota"] = CurrentQuota.ToString();
                _apiData["YT"]["maxQuota"] = MaxQuota.ToString();
            }

            _apiData["YT"]["quitTime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.FF", CultureInfo.InvariantCulture);
            DisposeQuotaResetTimer();
        }
    }
}


