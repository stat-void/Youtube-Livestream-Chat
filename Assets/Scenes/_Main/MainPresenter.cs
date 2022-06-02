using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Void.YoutubeAPI;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public class MainPresenter : MonoBehaviour
{
    // All of this could be separatable as the "startup screen"
    [SerializeField] protected GameObject OpeningScreen;
    [SerializeField] protected TMP_InputField API_InputField;
    [SerializeField] protected TMP_InputField VideoID_InputField;
    [SerializeField] protected TMP_Text FeedbackField;
    [SerializeField] protected TMP_Text FeedbackPlaceHolder;
    [SerializeField] protected Button ConnectionButton;

    [SerializeField] protected YoutubeLiveChatMessages Chatter;
    [SerializeField] protected YoutubeAPITimer YoutubeTimer;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        //PlayerSettings.resizableWindow = true;

        PreloadViews();

        ConnectionButton.onClick.AddListener(AttemptInit);
        YoutubeLiveChatMessages.Feedback += OnFeedback;
    }

    //TODO: You could also make this a combined startup/settings screen?
    private async void AttemptInit()
    {
        Chatter.SetAPIKey(API_InputField.text);

        if (!await Chatter.GetChatIDAsync(VideoID_InputField.text))
            return;

        var res = await Chatter.InitializeChatAsync();

        if (res)
        {
            YoutubeTimer.StartTimer();

            // TODO: Temporary, remove this once the general system is in place
            ChatDisplayPresenter presenter = FindObjectOfType<ChatDisplayPresenter>();
            presenter.Open();

            YoutubeLiveChatMessages.Feedback -= OnFeedback;
            OpeningScreen.SetActive(false);
        }
    }

    private void OnFeedback(string msg)
    {
        FeedbackField.text = msg;
        FeedbackPlaceHolder.gameObject.SetActive(false);
    }

    private void PreloadViews()
    {
        ViewSystem.Open("ChatDisplay/ChatDisplay");
    }


    /*
    TODO List

    Update Unity LTS whenever possible until those random crashes stop.

    Check out the IEnumerator for chat display in build
        Part of an issue with async is the lack of a WaitForEndOfFrame option, but even with this, the editor build can throw
            some of the errors I'm trying to avoid when losing focus. Check if full build does that?
        ChatDisplayPresenter - line 156, ArgumentOutOfRangeException


    API Key saving (alongside other data like restoring Query state and resetting that)
        To make this entire app far less annoying to start using after the first session
        Just make sure to add options to permanently delete the key as well (Settings),
            but for now, having a checkmark next to it should be enough?


    Do I need a disclaimer for API Key usage guidelines?
        Youtube mentioned it somewhere, but where should I?


    In-build guide on how to get an API Key.
        So people wouldn't have to read the GitLab/Hub page.

    Text cleanup?
        Some people can send messages later than others, but due to a better connection, their message will come up first, causing
            the resulting text to pop up multiple times. Should I add some minor checks for the previous message bulk to ignore repeating ID / message content?
        I would have to implement the checking condition this needs with Focus Mode anyway.


    Screen Manager
        All main window views should have a shared abstract class that is remembered in this new class.
        Has a general "Start/Open" that MainPresenter will call once ready. From there,
            ChatDisplayPresenter will be opened up as the first submenu.


    Settings screen
        Modifiable duration value, with the option to pick the "recommended" dynamic wait time from Youtube as well
        Options to delete saved data


    Resizing
        Fix some of the elements with janky resizing so you can enable freeform resizing. (mainly query panel)
        Resized value is remembered to recall into later. (This class, Awake)


    Text
        More language support? Any way to reduce memory on it?


    Focus Mode
        The first Screen Manager extra function. You already know how this should go.
        Also, maybe document all the other ideas here at some point?


    RecyclerView lists with resizable elements.
        So ChatMessageListener can increase the amount of messages to 500/1000 remembered with trivial performance hits.
        This could also make the movement of the chat display smoother?
        What about a RecyclerGrid for some other areas?


    Long term - Android Support?
        I'd have to make sure that touch inputs work okay, but other than that, Would I benefit from having this as a portable solution?
        What about frame rates? Would having a selectable 30/60 fps mode help?
    */

}
