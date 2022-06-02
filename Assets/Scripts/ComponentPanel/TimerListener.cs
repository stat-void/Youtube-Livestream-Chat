using UnityEngine;
using Void.YoutubeAPI;
using TMPro;
using System;

public class TimerListener : MonoBehaviour
{
    [SerializeField] protected YoutubeAPITimer Timer;
    [SerializeField] protected SlicedFilledImage TimerFill;
    [SerializeField] protected TMP_Text WaitTimeText;

    private bool _filling = true;

    private void Start()
    {
        WaitTimeText.text = Timer.APIRequestDelay.ToString("0.0");

        TimerFill.fillAmount    = _filling ? 0 : 1;
        TimerFill.fillDirection = _filling ? SlicedFilledImage.FillDirection.Right : SlicedFilledImage.FillDirection.Left;

        Timer.SendCurrentTime += OnTimeUpdate;
        Timer.OnAPIRequestDelayChanged += OnWaitTimeChanged;
    }

    private void OnWaitTimeChanged(float obj)
    {
        throw new NotImplementedException();
    }

    private void OnTimeUpdate(float currentTime)
    {
        if (_filling)
            TimerFill.fillAmount = Mathf.InverseLerp(0, Timer.APIRequestDelay, currentTime);
        else
            TimerFill.fillAmount = Mathf.InverseLerp(Timer.APIRequestDelay, 0, currentTime);

        if (currentTime >= Timer.APIRequestDelay)
        {
            _filling = !_filling;

            TimerFill.fillDirection = _filling ?
                SlicedFilledImage.FillDirection.Right : SlicedFilledImage.FillDirection.Left;
        }

    }
}
