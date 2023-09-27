using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Void.YoutubeAPI;
using Void.YoutubeAPI.LiveStreamChat.Messages;

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
    private YoutubeKeyManager _keyManager;
    private Settings _settings;

    private void Awake()
    {
        BaseCanvas.worldCamera = Camera.main;
        BaseCanvas.gameObject.SetActive(false);
    }
    
    private void Start()
    {
        _apiTimer = FindObjectOfType<YoutubeDataAPI>().APITimer;
        _keyManager = FindObjectOfType<YoutubeDataAPI>().KeyManager;
        _settings = FindObjectOfType<Settings>();

        RealTime.isOn = Settings.RealTime;
        Animations.isOn = Settings.Animations;
        UseYTInterval.isOn = _apiTimer.UseYoutubeInterval;

        MaxQuotaInput.text = _keyManager.MaxQuota.ToString();
        RequestIntervalInput.text = _apiTimer.APIRequestInterval.ToString();

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

        RealTime.onValueChanged.AddListener(OnRealTimeUpdate);
        Animations.onValueChanged.AddListener(OnAnimationsUpdate);
        UseYTInterval.onValueChanged.AddListener(OnUseYTIntervalUpdate);

        MaxQuotaInput.onEndEdit.AddListener(OnMaxQuotaUpdate);
        RequestIntervalInput.onEndEdit.AddListener(OnRequestDelayUpdate);

        DeleteAPIKeyButton.onClick.AddListener(OnAPIKeyDeleteRequest);
        DeleteAllInfoButton.onClick.AddListener(OnDeleteAllRequest);
    }

    public override void Close()
    {
        BaseCanvas.gameObject.SetActive(false);

        RealTime.onValueChanged.RemoveListener(OnRealTimeUpdate);
        Animations.onValueChanged.RemoveListener(OnAnimationsUpdate);
        UseYTInterval.onValueChanged.RemoveListener(OnUseYTIntervalUpdate);

        MaxQuotaInput.onEndEdit.RemoveListener(OnMaxQuotaUpdate);
        RequestIntervalInput.onEndEdit.RemoveListener(OnRequestDelayUpdate);

        DeleteAPIKeyButton.onClick.RemoveListener(OnAPIKeyDeleteRequest);
        DeleteAllInfoButton.onClick.RemoveListener(OnDeleteAllRequest);
    }

    private void OnRealTimeUpdate(bool value) =>
        _settings.SetRealTime(value);

    private void OnAnimationsUpdate(bool value) =>
        _settings.SetAnimations(value);

    private void OnUseYTIntervalUpdate(bool value)
    {
        _apiTimer.UseYoutubeInterval = value;
    }
        

    private void OnRequestDelayUpdate(string value)
    {
        if (value == "")
        {
            _apiTimer.SetAPIRequestInterval(3f);
            RequestIntervalInput.text = _apiTimer.APIRequestInterval.ToString();
        }
        else
        {
            _apiTimer.SetAPIRequestInterval(float.Parse(value));
            RequestIntervalInput.text = _apiTimer.APIRequestInterval.ToString();
        }
    }
        

    private void OnMaxQuotaUpdate(string value)
    {
        int endValue = Mathf.Max(int.Parse(value), 10000);
        MaxQuotaInput.text = endValue.ToString();
        _keyManager.SetMaxQuota(endValue);
    }


    private void OnAPIKeyDeleteRequest() =>
        YoutubeSaveData.GetData()["YT"]["apiKey"] = "";

    private void OnDeleteAllRequest() =>
        YoutubeSaveData.DeleteData();
    
        
}
