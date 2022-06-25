using UnityEngine;

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


    public void SetRealTime(bool value)
    {
        _realTime = value;
        PlayerPrefs.SetInt("YT_RealTime", _realTime ? 1 : 0);
    }

    public void SetAnimations(bool value)
    {
        _animations = value;
        PlayerPrefs.SetInt("YT_Animations", _animations ? 1 : 0);
    }

    private void Awake()
    {
        _realTime = PlayerPrefs.GetInt("YT_RealTime", 1) == 1;
        _animations = PlayerPrefs.GetInt("YT_Animations", 1) == 1;
    }
}
