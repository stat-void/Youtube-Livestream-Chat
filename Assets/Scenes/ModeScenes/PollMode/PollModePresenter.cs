using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Void.YoutubeAPI;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public class PollModePresenter : AModePresenter
{
    [SerializeField] protected Canvas BaseCanvas;
    [SerializeField] protected PollManager PollManager;
    [SerializeField] protected Button ResetButton;
    [SerializeField] protected Button StartButton;
    [SerializeField] protected Button StopButton;

    private YoutubeAPITimer _apiTimer;

    private void Awake()
    {
        BaseCanvas.worldCamera = Camera.main;
        BaseCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        _apiTimer = FindObjectOfType<YoutubeLiveChatMessages>().APITimer;
        NotifyClassReady(this);
    }

    public override string GetName()
    {
        return "Poller";
    }

    public override string GetDescription()
    {
        return "Display the most popular messages that the chat is collectively writing.";
    }

    public override void Open()
    {
        BaseCanvas.gameObject.SetActive(true);
        _apiTimer.PauseTimer();

        StartButton.gameObject.SetActive(true);
        StopButton.gameObject.SetActive(false);

        StartButton.onClick.AddListener(OnStartClicked);
        StopButton.onClick.AddListener(OnStopClicked);
        ResetButton.onClick.AddListener(OnResetClicked);

        PollManager.Open();
    }

    public override void Close()
    {
        _apiTimer.PauseTimer();

        StartButton.onClick.RemoveListener(OnStartClicked);
        StopButton.onClick.RemoveListener(OnStopClicked);
        ResetButton.onClick.RemoveListener(OnResetClicked);

        PollManager.Close();
        BaseCanvas.gameObject.SetActive(false);
    }

    private void OnStartClicked()
    {
        StartButton.gameObject.SetActive(false);
        StopButton.gameObject.SetActive(true);

        _apiTimer.StartTimer();
        PollManager.StartVote();
    }

    private void OnStopClicked()
    {
        StartButton.gameObject.SetActive(true);
        StopButton.gameObject.SetActive(false);

        _apiTimer.PauseTimer();
        PollManager.StopVote();
    }

    private void OnResetClicked()
    {
        PollManager.ResetPolls();
    }
}
