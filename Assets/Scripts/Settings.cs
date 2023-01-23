using SimpleJSON;
using UnityEngine;
using Void.YoutubeAPI;

/// <summary> All app specific settings that are not being monitored by other classes. </summary>
public class Settings : MonoBehaviour
{
    /// <summary> Should any post-request visual updates be performed in "real time" or updated instantly? </summary>
    public static bool RealTime
    {
        get { return _realTime; }
    }

    /// <summary> Should visual updates be animated where possible, or not (instantly changed)? </summary>
    public static bool Animations
    {
        get { return _animations; }
    }

    private static bool _realTime;
    private static bool _animations;

    private void Awake()
    {
        JSONNode apiData = YoutubeData.GetData();

        _realTime = !string.IsNullOrEmpty(apiData["Settings"]["RealTime"]) ? apiData["Settings"]["RealTime"].AsBool : true;
        _animations = !string.IsNullOrEmpty(apiData["Settings"]["Animations"]) ? apiData["Settings"]["Animations"].AsBool : true;
    }

    public void SetRealTime(bool value)
    {
        _realTime = value;
        YoutubeData.GetData()["Settings"]["RealTime"] = _realTime ? "true" : "false";
    }

    public void SetAnimations(bool value)
    {
        _animations = value;
        YoutubeData.GetData()["Settings"]["Animations"] = _animations ? "true" : "false";
    }
}
