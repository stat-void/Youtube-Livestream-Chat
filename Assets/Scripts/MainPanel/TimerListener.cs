using UnityEngine;
using UnityEngine.UI;
using Void.YoutubeAPI;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using TMPro;

public class TimerListener : MonoBehaviour
{
    [SerializeField] protected YoutubeLiveChatMessages Messager;
    [SerializeField] protected SlicedFilledImage TimerFill;
    [SerializeField] protected TMP_Text WaitTimeText;

    [SerializeField] protected Button PlayButton;
    [SerializeField] protected Button PauseButton;

    private YoutubeAPITimer _apiTimer;
    private bool _filling = true;


    private void Start()
    {
        _apiTimer = Messager.APITimer;

        WaitTimeText.text = _apiTimer.APIRequestInterval.ToString("0.0");

        TimerFill.fillAmount    = _filling ? 0 : 1;
        TimerFill.fillDirection = _filling ? SlicedFilledImage.FillDirection.Right : SlicedFilledImage.FillDirection.Left;


        PlayButton.onClick.AddListener(OnPlayButtonPressed);
        PauseButton.onClick.AddListener(OnPauseButtonPressed);
        PlayButton.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);

        _apiTimer.SendCurrentTime += OnTimeUpdate;
        _apiTimer.OnAPIRequestDelayChanged += OnWaitTimeChanged;

        StartupDisplayPresenter.OnStartupFinish += OnStartupFinish;
    }

    private void OnWaitTimeChanged(float value)
    {
        WaitTimeText.text = value.ToString("0.0");
    }

    private void OnTimeUpdate(float currentTime)
    {
        if (_filling)
            TimerFill.fillAmount = Mathf.InverseLerp(0, _apiTimer.APIRequestInterval, currentTime);
        else
            TimerFill.fillAmount = Mathf.InverseLerp(_apiTimer.APIRequestInterval, 0, currentTime);

        if (currentTime >= _apiTimer.APIRequestInterval)
        {
            _filling = !_filling;

            TimerFill.fillDirection = _filling ?
                SlicedFilledImage.FillDirection.Right : SlicedFilledImage.FillDirection.Left;
        }

    }

    private void OnPlayButtonPressed() =>
        _apiTimer.StartTimer();

    private void OnPauseButtonPressed() =>
        _apiTimer.PauseTimer();


    private void OnStartupFinish()
    {
        StartupDisplayPresenter.OnStartupFinish -= OnStartupFinish;

        if (_apiTimer.IsPlaying)
            PauseButton.gameObject.SetActive(true);
        else
            PlayButton.gameObject.SetActive(true);

        _apiTimer.OnTimerPlayUpdate += OnTimerUpdated;
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
