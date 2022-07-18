using System;
using System.Collections.Generic;
using UnityEngine;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public class ChatMessageListener : MonoBehaviour
{
    public static event Action<List<YoutubeChatMessage>> ChatMessages;

    public static List<YoutubeChatMessage> MessageList;
    public static readonly int ListMaxSize = 100;

    public static SortedDictionary<string, List<string>> UsernameIDPairs { get; private set; } = new();
    public static Dictionary<string, YoutubeChatMessage> IDLastMessagePairs { get; private set; } = new();

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

            // Add last messages by ID value
            IDLastMessagePairs[messages[i].ChannelID] = messages[i];

            // Add all recorded users by name and list of IDs that may share that name

            // Initialize new username with new ID list
            if (!UsernameIDPairs.ContainsKey(messages[i].Username))
                UsernameIDPairs[messages[i].Username] = new() { messages[i].ChannelID };

            // Alternatively, Add new ID to a username
            else if (!UsernameIDPairs[messages[i].Username].Contains(messages[i].ChannelID))
                UsernameIDPairs[messages[i].Username].Add(messages[i].ChannelID);
            
        }

        ChatMessages?.Invoke(messages);
    }
}
