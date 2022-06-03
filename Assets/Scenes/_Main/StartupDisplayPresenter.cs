using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Void.YoutubeAPI;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public class StartupDisplayPresenter : AContentPresenter
{
    [SerializeField] protected TMP_InputField API_InputField;
    [SerializeField] protected TMP_InputField VideoID_InputField;
    [SerializeField] protected Toggle SaveAPIKeyToggle;
    

    [SerializeField] protected TMP_Text FeedbackField;
    [SerializeField] protected TMP_Text FeedbackPlaceHolder;
    [SerializeField] protected Button ConnectionButton;

    [SerializeField] protected YoutubeLiveChatMessages Chatter;
    [SerializeField] protected YoutubeQuotaManager QuotaManager;
    

    public override void Open()
    {
        gameObject.SetActive(true);

        ConnectionButton.onClick.AddListener(AttemptInit);
        YoutubeLiveChatMessages.Feedback += OnFeedback;

        if (!string.IsNullOrWhiteSpace(QuotaManager.APIKey))
        {
            API_InputField.placeholder.GetComponent<TMP_Text>().text = "Saved API Key found. Write new to override (Hidden)";
        }
    }

    public override void Close()
    {
        ConnectionButton.onClick.RemoveListener(AttemptInit);
        YoutubeLiveChatMessages.Feedback -= OnFeedback;

        gameObject.SetActive(false);
    }


    //TODO: You could also make this a combined startup/settings screen?
    private async void AttemptInit()
    {
        QuotaManager.SetAPIKey(API_InputField.text, SaveAPIKeyToggle.isOn);

        if (!await Chatter.GetChatIDAsync(VideoID_InputField.text))
            return;

        var res = await Chatter.InitializeChatAsync();

        if (res)
        {
            // TODO: Temporary, remove this once the general system is in place
            ChatDisplayPresenter presenter = FindObjectOfType<ChatDisplayPresenter>();
            presenter.Open();

            Close();
        }
    }

    private void OnFeedback(string msg)
    {
        FeedbackField.text = msg;
        FeedbackPlaceHolder.gameObject.SetActive(false);
    }

}
