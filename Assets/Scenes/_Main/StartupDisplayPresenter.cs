using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Void.YoutubeAPI;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public class StartupDisplayPresenter : AModePresenter
{
    [SerializeField] protected ModeManagerPresenter ModeManager;
    [SerializeField] protected APIGuidePresenter APIPresenter;

    [Header("Inputs")]
    [SerializeField] protected TMP_InputField API_InputField;
    [SerializeField] protected TMP_InputField VideoID_InputField;
    [SerializeField] protected Toggle SaveAPIKeyToggle;
    [SerializeField] protected Toggle AutostartChatToggle;
    [SerializeField] protected Button ConnectionButton;

    [Header("Feedback")]
    [SerializeField] protected TMP_Text FeedbackField;
    [SerializeField] protected TMP_Text FeedbackPlaceHolder;

    [Header("Youtube Chat Connectors")]
    [SerializeField] protected YoutubeLiveChatMessages Chatter;
    [SerializeField] protected YoutubeQuotaManager QuotaManager;

    public override string GetName()
    {
        return "Startup Display Presenter";
    }

    public override string GetDescription()
    {
        return "";
    }

    public override void Open()
    {
        gameObject.SetActive(true);

        ConnectionButton.onClick.AddListener(AttemptInit);
        YoutubeLiveChatMessages.Feedback += OnFeedback;

        if (!string.IsNullOrWhiteSpace(QuotaManager.APIKey))
        {
            API_InputField.placeholder.GetComponent<TMP_Text>().text = "API Key found. Write new if you want to override (Hidden)";
        }
    }

    public override void Close()
    {
        ConnectionButton.onClick.RemoveListener(AttemptInit);
        YoutubeLiveChatMessages.Feedback -= OnFeedback;

        APIPresenter.Finish();
        gameObject.SetActive(false);
    }

    private async void AttemptInit()
    {
        QuotaManager.SetAPIKey(API_InputField.text, SaveAPIKeyToggle.isOn);

        if (!await Chatter.GetChatIDAsync(VideoID_InputField.text))
            return;

        var res = await Chatter.InitializeChatAsync();

        if (res)
        {
            ModeManager.ShowAddModeButton();

            if (AutostartChatToggle.isOn)
                ModeManager.OpenByName("Chat");

            Close();
        }
    }

    private void OnFeedback(string msg)
    {
        FeedbackField.text = msg;
        FeedbackPlaceHolder.gameObject.SetActive(false);
    }

}
