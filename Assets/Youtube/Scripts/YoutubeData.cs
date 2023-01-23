using System;
using System.IO;
using SimpleJSON;
using UnityEngine;

namespace Void.YoutubeAPI
{
    /// <summary>
    /// Class to recall the location where data should be stored.
    /// </summary>
    public static class YoutubeData
    {
        private static string basePath = "";
        private const string _dataName = "YT_API_Data.json";
        private static JSONNode _data;

        private static bool _initialized = false;

        public static JSONNode GetData()
        {
            if (!_initialized)
                Initialize();
            return _data;
        }

        public static void SaveData()
        {
            if (!_initialized)
                return;

            _data.SaveToFile(basePath + $"\\{_dataName}");
        }

        public static void ResetData()
        {
            _data = "";
        }


        private static void Initialize()
        {
            try
            {
                Debug.Log("Youtube data path initialization started.");

                SetDirectory(AppContext.BaseDirectory);
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogWarning($"Unable to create or load JSON data file - {e.Message}");
                Debug.LogWarning($"Using Unity persistent data path.");
                
                SetDirectory(Application.persistentDataPath);
            }

            _initialized = true;

            Debug.Log($"Save path set as {basePath + $"\\{_dataName}"}");
        }

        private static void SetDirectory(string baseDirectory)
        {
            if (!File.Exists(baseDirectory + $"\\{_dataName}"))
            {
                _data = JSON.Parse("{}");
                _data.SaveToFile(baseDirectory + $"\\{_dataName}");
            }
            else
            {
                _data = JSONNode.LoadFromFile(baseDirectory + $"\\{_dataName}");
            }

            basePath = baseDirectory;
        }
    }
}

