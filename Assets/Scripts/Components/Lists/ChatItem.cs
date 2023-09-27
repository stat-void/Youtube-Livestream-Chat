using UnityEngine;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using TMPro;

public class ChatItem : AListItem
{
    [SerializeField] protected RectTransform RectTransform;
    [SerializeField] protected RectTransform Separator;
    [SerializeField] protected TMP_Text Timestamp;
    [SerializeField] protected TMP_Text UserData;

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
        UserData.text = $"<color=#{userColor}>{node.Username}</color>  <color=#{messageColor}>{node.Message}</color>";

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
        if (node.Type == MessageType.SuperChatEvent || node.Type == MessageType.SuperStickerEvent)
        {
            return ColorSettings.UserSuperColors[node.SuperEvent.Tier - 1];
        } 
   
        // Regular cases
        if (node.AuthorDetails.IsOwner)
            return ColorSettings.OwnerColor;

        else if (node.AuthorDetails.IsModerator)
            return ColorSettings.ModeratorColor;

        else if (node.AuthorDetails.IsMember)
            return ColorSettings.MemberColor;

        else
            return ColorSettings.RegularUserColor;
    }

    private string GetMessageColor(YoutubeChatMessage node)
    {
        return node.Type switch
        {
            MessageType.SuperChatEvent     => ColorSettings.MessageSuperColors[node.SuperEvent.Tier - 1],
            MessageType.SuperStickerEvent  => ColorSettings.MessageSuperColors[node.SuperEvent.Tier - 1],

            MessageType.NewMemberEvent                 => ColorSettings.MemberColor,
            MessageType.MemberMilestoneChatEvent       => ColorSettings.MemberColor,
            MessageType.MembershipGiftingEvent         => ColorSettings.MemberColor,
            MessageType.GiftMembershipReceivedEvent    => ColorSettings.MemberColor,
            _                                               => ColorSettings.RegularMessageColor,
        };
    }
}
