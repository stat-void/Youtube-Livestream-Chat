using UnityEngine;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using TMPro;
using System.Collections.Generic;

public class ChatItem : AListItem
{
    [SerializeField] protected RectTransform RectTransform;
    [SerializeField] protected RectTransform Separator;
    [SerializeField] protected TMP_Text Timestamp;
    [SerializeField] protected TMP_Text UserData;

    private const string _regularUserColor  = "#BABABA";
    private const string _memberUserColor   = "#0D9D58";
    private const string _moderatorColor    = "#007FFF";
    private const string _ownerColor        = "#FFD603";

    //0-blue, 1-light blue, 2-yellowgreen, 3-yellow, 4-orange, 5-magenta, 6-red
    private readonly List<string> _darkSuperColors =  new() { "#1665C0", "#00B8D3", "#01BFA5", "#FEB300", "#E65100", "#C1195B", "#D00001" };
    private readonly List<string> _superColors =      new() { "#1665C0", "#00E5FF", "#1EE9B6", "#FFC927", "#F57B02", "#E81E63", "#E62216" };
    private readonly List<string> _lightSuperColors = new() { "#1665C0", "#00E5FF", "#34FFCC", "#FFC927", "#FF850C", "#FF357A", "#FF3B2F" };


    public override void Bind(YoutubeChatMessage node, Transform active)
    {
        gameObject.SetActive(true);
        transform.SetParent(active);

        // Set Timestamp
        Timestamp.text = node.Timestamp.ToString("HH:mm:ss");

        // Prepare visual colors for the chat message
        string userColor = GetUserColor(node);
        string messageColor = GetMessageColor(node);

        // Set contents
        UserData.text = $"<color={userColor}>{node.Username}</color>  <color={messageColor}>{node.Message}</color>";

        // Ensure that the text fits in its preferred height value
        UpdateFit();
    }

    public override void Unbind(Transform pool)
    {
        transform.SetParent(pool);

        Timestamp.text = "";
        UserData.text = "";
        gameObject.SetActive(false);
    }
    
    public override void UpdateFit()
    {
        if (RectTransform)
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, UserData.preferredHeight + 10);
    }

    public override float GetHeight()
    {
        if (RectTransform)
            return RectTransform.sizeDelta.y;
        else
            return 0;
    }

    private string GetUserColor(YoutubeChatMessage node)
    {
        // Superchat override case
        if (node.Type == MessageEventType.SuperChatEvent || node.Type == MessageEventType.SuperStickerEvent)
        {
            return _darkSuperColors[node.SuperEvent.Tier - 1];
        } 
   
        // Regular cases
        if (node.AuthorDetails.IsOwner)
            return _ownerColor;

        else if (node.AuthorDetails.IsModerator)
            return _moderatorColor;

        else if (node.AuthorDetails.IsMember)
            return _memberUserColor;

        else
            return _regularUserColor;
    }

    private string GetMessageColor(YoutubeChatMessage node)
    {
        return node.Type switch
        {
            MessageEventType.SuperChatEvent     => _lightSuperColors[node.SuperEvent.Tier - 1],
            MessageEventType.SuperStickerEvent  => _lightSuperColors[node.SuperEvent.Tier - 1],

            MessageEventType.NewMemberEvent                 => _memberUserColor,
            MessageEventType.MemberMilestoneChatEvent       => _memberUserColor,
            MessageEventType.MembershipGiftingEvent         => _memberUserColor,
            MessageEventType.GiftMembershipReceivedEvent    => _memberUserColor,
            _                                               => "#FDFDFD",
        };
    }


}
