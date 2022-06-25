using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using UnityEngine.UI;
using Void.YoutubeAPI;
using TMPro;
using System;

public class SettingsPresenter : AModePresenter
{
    [SerializeField] protected Canvas BaseCanvas;

    [Header("Settings - APITimer Functions")]
    [SerializeField] protected TMP_InputField RequestIntervalInput;
    [SerializeField] protected Toggle UseYTInterval;

    [Header("Settings - QuotaManager Functions")]
    [SerializeField] protected TMP_InputField MaxQuotaInput;
    [SerializeField] protected Button DeleteAPIKeyButton;

    [Header("Settings - Other")]
    [SerializeField] protected Toggle RealTime;
    [SerializeField] protected Toggle Animations;
    [SerializeField] protected Button DeleteAllInfoButton;

    // Have a textbox area with select/deselect for all settings? For descriptions?
    //[SerializeField] protected TMP_Text Feedback;

    private YoutubeAPITimer _apiTimer;
    private YoutubeQuotaManager _quotaManager;
    private Settings _settings;

    private void Awake()
    {
        BaseCanvas.worldCamera = Camera.main;
        BaseCanvas.gameObject.SetActive(false);
    }
    
    private void Start()
    {
        _apiTimer = FindObjectOfType<YoutubeAPITimer>();
        _quotaManager = FindObjectOfType<YoutubeQuotaManager>();
        _settings = FindObjectOfType<Settings>();

        RealTime.isOn = Settings.RealTime;
        Animations.isOn = Settings.Animations;
        UseYTInterval.isOn = _apiTimer.UseYoutubeInterval;

        MaxQuotaInput.text = _quotaManager.MaxQuota.ToString();
        RequestIntervalInput.text = _apiTimer.APIRequestInterval.ToString();

        RealTime.onValueChanged.AddListener(OnRealTimeUpdate);
        Animations.onValueChanged.AddListener(OnAnimationsUpdate);
        UseYTInterval.onValueChanged.AddListener(OnUseYTIntervalUpdate);

        MaxQuotaInput.onEndEdit.AddListener(OnMaxQuotaUpdate);
        RequestIntervalInput.onEndEdit.AddListener(OnRequestDelayUpdate);

        DeleteAPIKeyButton.onClick.AddListener(OnAPIKeyDeleteRequest);

        NotifyClassReady(this);
    }

    public override string GetName()
    {
        return "Settings";
    }

    public override string GetDescription()
    {
        return "Edit runtime operations.";
    }

    public override void Open()
    {
        BaseCanvas.gameObject.SetActive(true);
    }

    public override void Close()
    {
        BaseCanvas.gameObject.SetActive(false);
    }

    private void OnRealTimeUpdate(bool value) =>
        _settings.SetRealTime(value);

    private void OnAnimationsUpdate(bool value) =>
        _settings.SetAnimations(value);

    private void OnUseYTIntervalUpdate(bool value)
    {
        _apiTimer.UseYoutubeInterval = value;

        //RequestIntervalInput.interactable = !value;
        //TODO: Should affect coloring of the text in addition of interactable changing image?
    }
        

    private void OnRequestDelayUpdate(string value)
    {
        RequestIntervalInput.text = string.Format(value, "0.00");
        _apiTimer.SetAPIRequestInterval(float.Parse(value));
        RequestIntervalInput.text = _apiTimer.APIRequestInterval.ToString();
    }
        

    private void OnMaxQuotaUpdate(string value)
    {
        int endValue = Mathf.Max(int.Parse(value), 10000);
        MaxQuotaInput.text = endValue.ToString();
        _quotaManager.SetMaxQuota(endValue);
    }


    private void OnAPIKeyDeleteRequest() =>
        PlayerPrefs.DeleteKey("YT_APIKey");
    

    private void OnDeleteAllRequest() =>
        PlayerPrefs.DeleteAll();

}
