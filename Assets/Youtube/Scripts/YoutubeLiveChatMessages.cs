using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Threading.Tasks;

namespace Void.YoutubeAPI.LiveStreamChat.Messages
{
    /// <summary>
    /// Main class that handles API initialization and
    /// sending web queries to fetch the latest messages from a given chat.
    ///
    /// Does not require OAuth 2.0 authentication.
    /// </summary>
    [RequireComponent(typeof(YoutubeQuotaManager))]
    public class YoutubeLiveChatMessages : MonoBehaviour
    {
        public static event Action<List<YoutubeChatMessage>> ChatMessages;
        public static event Action<int> RecommendedWaitTimeMilliseconds;
        public static event Action<string> Feedback;

        // Quota and APIKey handler
        private YoutubeQuotaManager _quotaManager;

        // Chat ID collected from the video ID and tokens to keep updating chat polls
        private string _chatID = "";
        private string _nextPageToken = "";

        // Temporarily store the newest publishing time, _publishedAt inherits this at the end of each chat message check
        private DateTime _publishedAt;
        private DateTime _newPublishedAt;

        // Usernames/Messages to invoke when extracted from polling.
        private List<YoutubeChatMessage> _fullMessages = new List<YoutubeChatMessage>();

        private void Awake()
        {
            _quotaManager = GetComponent<YoutubeQuotaManager>();
            YoutubeAPITimer.OnAPIMessagesRequested += GetChatMessages;
        }

        /// <summary>
        /// Get the livestream Chat ID based on the given Video ID.
        /// If successful, Chat ID is stored locally in this class.
        /// Youtube API Query - 1 point.
        /// </summary>
        /// <returns> bool -  Was the call successful? </returns>
        public async Task<bool> GetChatIDAsync(string videoID)
        {
            _chatID = "";

            Uri URL = new Uri($"https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&key={_quotaManager.APIKey}&id={videoID}");
            using (UnityWebRequest www = UnityWebRequest.Get(URL))
            {
                var request = www.SendWebRequest();
                while (!request.isDone)
                    await Task.Yield();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    LogError(www);
                    return false;
                }

                _quotaManager.AddQuota(1);

                JSONNode data = JSON.Parse(www.downloadHandler.text);
                _chatID = data["items"][0]["liveStreamingDetails"]["activeLiveChatId"];

                if (string.IsNullOrWhiteSpace(_chatID))
                {
                    Log("No livestream chat was found on the given video ID. Check if you typed it in correctly.", true);
                    return false;
                }
            }
            Log("Youtube Chat ID successfully found and stored.", false);
            return true;
        }

        /// <summary>
        /// Make the first Youtube Chat API call to initialize the most recent timestamp and
        /// wait the needed amount of time before next page tokens start functioning with any set delay.
        /// Youtube API Query - 5 points.
        /// </summary>
        /// <returns> bool - Was the call successful? </returns>
        public async Task<bool> InitializeChatAsync()
        {
            if (string.IsNullOrWhiteSpace(_chatID))
            {
                Log("No Chat ID detected to initialize. use YoutubeLiveStreamChat.GetChatID first.", true);
                return false;
            }

            _nextPageToken = "";

            Uri URL = new Uri($"https://www.googleapis.com/youtube/v3/liveChat/messages?part=snippet,authorDetails&liveChatId={_chatID}&key={_quotaManager.APIKey}");
            using (UnityWebRequest www = UnityWebRequest.Get(URL))
            {
                var request = www.SendWebRequest();
                while (!request.isDone)
                    await Task.Yield();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    LogError(www);
                    return false;
                }

                _quotaManager.AddQuota(5);

                JSONNode data = JSON.Parse(www.downloadHandler.text);
                JSONNode content = data["items"].AsArray[^1];           // arg[arg.Count - 1]

                if (content.Count == 0)
                    _publishedAt = DateTime.ParseExact("1970-01-01T00:00:00.00", "yyyy-MM-ddTHH:mm:ss.FF", System.Globalization.CultureInfo.InvariantCulture);
                else
                    _publishedAt = DateTime.ParseExact(content["snippet"]["publishedAt"].Value.Substring(0, 22), "yyyy-MM-ddTHH:mm:ss.FF", System.Globalization.CultureInfo.InvariantCulture);

                _newPublishedAt = _publishedAt;
                _nextPageToken = data["nextPageToken"];

                int waitTimeMilliseconds = int.Parse(data["pollingIntervalMillis"]);
                RecommendedWaitTimeMilliseconds?.Invoke(waitTimeMilliseconds);
                Log($"Youtube chat initialization successful, waiting required amount of polling interval - {waitTimeMilliseconds}ms.", false);
                await Task.Delay(waitTimeMilliseconds);

                Log("Youtube chat initialization done.", false);
            }
            return true;
        }

        /// <summary>
        /// Make a Youtube Chat API call to fetch and invoke newest messages up until the currently saved publishing time is passed.
        /// Invoked list order is newest to oldest, so be wary of iterating in reverse when using chat displays.
        /// Youtube API Query - 5 points.
        /// </summary>
        public async void GetChatMessages()
        {
            if (string.IsNullOrWhiteSpace(_nextPageToken))
            {
                Log("Chat ID has not been initialized", true);
                return;
            }

            Uri URL = new Uri($"https://www.googleapis.com/youtube/v3/liveChat/messages?part=snippet,authorDetails&pageToken={_nextPageToken}&liveChatId={_chatID}&key={_quotaManager.APIKey}");
            using (UnityWebRequest www = UnityWebRequest.Get(URL))
            {
                var request = www.SendWebRequest();
                while (!request.isDone)
                    await Task.Yield();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    LogError(www);
                    return;
                }

                _quotaManager.AddQuota(5);

                JSONNode data = JSON.Parse(www.downloadHandler.text);
                JSONArray arg = data["items"].AsArray;

                _nextPageToken = data["nextPageToken"]; //Use this to go to the next page
                RecommendedWaitTimeMilliseconds?.Invoke(int.Parse(data["pollingIntervalMillis"]));

                // Invoke and send all new messages
                PrepareAndInvokeMessages(ref arg);
                _publishedAt = _newPublishedAt;
            }
        }

        private void PrepareAndInvokeMessages(ref JSONArray arg)
        {
            _fullMessages.Clear();

            int i = arg.Count - 1;
            for (; i >= 0; i--)
            {
                JSONNode content = arg[i];
                DateTime published = DateTime.ParseExact(content["snippet"]["publishedAt"].Value.Substring(0, 22), "yyyy-MM-ddTHH:mm:ss.FF", System.Globalization.CultureInfo.InvariantCulture);

                if (i == arg.Count - 1) // Special instance to record the most newest message publishing string
                    _newPublishedAt = published;

                if (DateTime.Compare(_publishedAt, published) >= 0)
                    break;
 
                _fullMessages.Add(new YoutubeChatMessage(content, published));
            }

            ChatMessages?.Invoke(_fullMessages);
        }

        private void Log(string message, bool error)
        {
            if (error)
                Debug.LogError(message);
            else
                Debug.Log(message);

            Feedback?.Invoke(message);
        }

        private void LogError(UnityWebRequest www)
        {
            UnityWebRequest.Result error = www.result;

            string answer = "";

            if (error == UnityWebRequest.Result.ConnectionError)
                answer = "The request failed to communicate with the server";
            
            else if (error == UnityWebRequest.Result.DataProcessingError)
                answer = "Server communication successful, but error processing received data";

            else if (error == UnityWebRequest.Result.InProgress)
                answer = "The request is still in progress, which should not be possible here.";

            else if (error == UnityWebRequest.Result.ProtocolError)
            {
                JSONNode data = JSON.Parse(www.downloadHandler.text);
                string cause = data["error"]["errors"].AsArray[0]["message"];

                if (cause.Contains("The request was sent too soon after the previous one."))
                    return;

                answer = $"{www.error} - {cause}";
            }
            Debug.LogError(answer);
            Feedback?.Invoke(answer);
        }
    }
}

