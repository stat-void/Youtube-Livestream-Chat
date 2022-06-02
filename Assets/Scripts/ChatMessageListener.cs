using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public class ChatMessageListener : MonoBehaviour
{
    public static event Action<List<YoutubeChatMessage>> ChatMessages;

    public static List<YoutubeChatMessage> MessageList;

    public static bool WaitMessages = true;
    public static readonly int ListMaxSize = 100;

    private void Awake()
    {
        if (MessageList != null)
            Debug.LogWarning("There is more than 1 instance of ChatMessageListener! MessageList will likely break!");

        MessageList = new List<YoutubeChatMessage>(ListMaxSize);
        YoutubeLiveChatMessages.ChatMessages += ChatMessageUpdate;
    }

    /// <summary> In order of newest to oldest, update the list of stored messages, up to a maximum size. </summary>
    /// <param name="messages"> Messages received whenever a query update for messages is made. </param>
    private void ChatMessageUpdate(List<YoutubeChatMessage> messages)
    {
        for (int i = messages.Count-1; i >= 0; i--)
        {
            if (MessageList.Count >= ListMaxSize)
                MessageList.RemoveAt(MessageList.Count - 1);

            MessageList.Insert(0, messages[i]);
        }

        ChatMessages?.Invoke(messages);
    }
}
