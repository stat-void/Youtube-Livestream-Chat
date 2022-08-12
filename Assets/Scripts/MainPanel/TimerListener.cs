using UnityEngine;
using Void.YoutubeAPI;
using TMPro;
using System;
using UnityEngine.UI;

public class TimerListener : MonoBehaviour
{
    [SerializeField] protected YoutubeAPITimer Timer;
    [SerializeField] protected SlicedFilledImage TimerFill;
    [SerializeField] protected TMP_Text WaitTimeText;

    [SerializeField] protected Button PlayButton;
    [SerializeField] protected Button PauseButton;

    private bool _filling = true;

    private void Start()
    {
        WaitTimeText.text = Timer.APIRequestInterval.ToString("0.0");

        TimerFill.fillAmount    = _filling ? 0 : 1;
        TimerFill.fillDirection = _filling ? SlicedFilledImage.FillDirection.Right : SlicedFilledImage.FillDirection.Left;


        PlayButton.onClick.AddListener(OnPlayButtonPressed);
        PauseButton.onClick.AddListener(OnPauseButtonPressed);
        PlayButton.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);

        Timer.SendCurrentTime += OnTimeUpdate;
        Timer.OnAPIRequestDelayChanged += OnWaitTimeChanged;

        StartupDisplayPresenter.OnStartupFinish += OnStartupFinish;
    }

    private void OnWaitTimeChanged(float value)
    {
        WaitTimeText.text = value.ToString("0.0");
    }

    private void OnTimeUpdate(float currentTime)
    {
        if (_filling)
            TimerFill.fillAmount = Mathf.InverseLerp(0, Timer.APIRequestInterval, currentTime);
        else
            TimerFill.fillAmount = Mathf.InverseLerp(Timer.APIRequestInterval, 0, currentTime);

        if (currentTime >= Timer.APIRequestInterval)
        {
            _filling = !_filling;

            TimerFill.fillDirection = _filling ?
                SlicedFilledImage.FillDirection.Right : SlicedFilledImage.FillDirection.Left;
        }

    }

    private void OnPlayButtonPressed() =>
        Timer.StartTimer();

    private void OnPauseButtonPressed() =>
        Timer.PauseTimer();


    private void OnStartupFinish()
    {
        StartupDisplayPresenter.OnStartupFinish -= OnStartupFinish;

        if (Timer.IsPlaying)
            PauseButton.gameObject.SetActive(true);
        else
            PlayButton.gameObject.SetActive(true);

        Timer.OnTimerPlayUpdate += OnTimerUpdated;
    }


    private void OnTimerUpdated(bool state)
    {
        if (state)
        {
            PlayButton.gameObject.SetActive(false);
            PauseButton.gameObject.SetActive(true);
        }

        else
        {
            PlayButton.gameObject.SetActive(true);
            PauseButton.gameObject.SetActive(false);
        }
    }
}
