using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Void.YoutubeAPI;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public class VoteModePresenter : AModePresenter
{
    [SerializeField] protected Canvas BaseCanvas;
    [SerializeField] protected VoteManager VoteManager;
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
        _apiTimer = FindObjectOfType<YoutubeDataAPI>().APITimer;
        NotifyClassReady(this);
    }

    public override string GetName()
    {
        return "Voter";
    }

    public override string GetDescription()
    {
        return "Let users vote between a variety of choices.";
    }


    public override void Open()
    {
        BaseCanvas.gameObject.SetActive(true);
        _apiTimer.PauseTimer();

        StartButton.onClick.AddListener(OnStartClicked);
        StopButton.onClick.AddListener(OnStopClicked);
        ResetButton.onClick.AddListener(OnResetClicked);

        VoteManager.Open();
    }

    public override void Close()
    {
        _apiTimer.PauseTimer();

        StartButton.onClick.RemoveListener(OnStartClicked);
        StopButton.onClick.RemoveListener(OnStopClicked);
        ResetButton.onClick.RemoveListener(OnResetClicked);

        VoteManager.Close();
        BaseCanvas.gameObject.SetActive(false);
    }

    private void OnStartClicked()
    {
        StartButton.gameObject.SetActive(false);
        StopButton.gameObject.SetActive(true);
        ResetButton.interactable = false;

        _apiTimer.StartTimer();
        VoteManager.StartVote();
    }

    private void OnStopClicked()
    {
        StartButton.gameObject.SetActive(true);
        StopButton.gameObject.SetActive(false);
        ResetButton.interactable = true;

        _apiTimer.PauseTimer();
        VoteManager.StopVote();
    }

    private void OnResetClicked()
    {
        VoteManager.ResetQuestionAndAnswers();
    }
}
