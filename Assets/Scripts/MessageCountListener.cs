using System.Collections.Generic;
using UnityEngine;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using TMPro;

public class MessageCountListener : MonoBehaviour
{
    [SerializeField] protected TMP_Text MessagesReceivedCounter;

    void Awake()
    {
        YoutubeLiveChatMessages.ChatMessages += OnMessages;
        MessagesReceivedCounter.text = "0";
    }

    private void OnMessages(List<YoutubeChatMessage> messages)
    {
        MessagesReceivedCounter.text = messages.Count.ToString();
    }
}
