using UnityEngine;
using SimpleJSON;
using System;

namespace Void.YoutubeAPI.LiveStreamChat.Messages
{
    /// <summary> Container class for the contents of a Youtube chat message. </summary>
    public class YoutubeChatMessage
    {
        /// <summary> What kind of message was found, such as regular text message, Super Chat, Membership etc. </summary>
        public MessageType Type; 

        /// <summary> The ID value of the given user. </summary>
        public string ChannelID;

        /// <summary> Written name of the current user. </summary>
        public string Username;

        /// <summary> Webpage leading to the display icon used by this user. </summary>
        public string ProfileImageURL;

        /// <summary> UTC timestamp on when this message was sent. </summary>
        public DateTime Timestamp;

        /// <summary> User display message or relevant event notification. </summary>
        public string Message = string.Empty;

        /// <summary> Special privilege booleans, always available. </summary>
        public AuthorDetails AuthorDetails = new();

        /// <summary> Super Chat/Sticker related details, if valid type. Otherwise null. </summary>
        public SuperEvent SuperEvent;

        /// <summary> Membership related details - Self, Gifting, Receiving, if valid type. Otherwise null. </summary>
        public MemberUpdate MemberUpdate;


        public YoutubeChatMessage(JSONNode item, DateTime time)
        {
            Timestamp = time;
            Initialize(item);
        }

        public YoutubeChatMessage(JSONNode item)
        {
            Initialize(item);   
        }

        /// <summary> Set up this class depending on the type of message received. </summary>
        private void Initialize(JSONNode item)
        {
            // Message type
            Type = ParseTypeString(item["snippet"]["type"]);

            // Message author ID
            ChannelID                   = item["authorDetails"]["channelId"];
            Username                    = item["authorDetails"]["displayName"];
            ProfileImageURL             = item["authorDetails"]["profileImageUrl"];

            // Message author privileges
            AuthorDetails.IsVerified    = item["authorDetails"]["isVerified"].AsBool;
            AuthorDetails.IsOwner       = item["authorDetails"]["isChatOwner"].AsBool;
            AuthorDetails.IsMember      = item["authorDetails"]["isChatSponsor"].AsBool;
            AuthorDetails.IsModerator   = item["authorDetails"]["isChatModerator"].AsBool;

            // Message timestamp
            if (Timestamp == null)
                Timestamp = DateTime.ParseExact(item["snippet"]["publishedAt"].Value.Substring(0, 22), "yyyy-MM-ddTHH:mm:ss.FF", System.Globalization.CultureInfo.InvariantCulture);


            // Initialization of type-specific fields:
            switch (Type)
            {
                case MessageType.TextMessageEvent:
                    Message = item["snippet"]["displayMessage"];
                    break;

                case MessageType.SuperChatEvent:
                    SetupSuperEvent(item, "superChatDetails");
                    break;

                case MessageType.SuperStickerEvent:
                    SetupSuperEvent(item, "superStickerDetails");
                    break;

                case MessageType.NewMemberEvent:
                    SetupMemberUpdateEvent(item, "newSponsorDetails");
                    break;

                case MessageType.MemberMilestoneChatEvent:
                    SetupMemberUpdateEvent(item, "memberMilestoneChatDetails");
                    break;

                case MessageType.MembershipGiftingEvent:
                    SetupMemberUpdateEvent(item, "membershipGiftingDetails");
                    break;

                case MessageType.GiftMembershipReceivedEvent:
                    SetupMemberUpdateEvent(item, "giftMembershipReceivedDetails");
                    break;

                default:
                    Debug.LogWarning($"Unchecked type specific field: {Type}, any messages? - | {item["snippet"]["displayMessage"]} |");
                    break;
            }
        }

        private MessageType ParseTypeString(string type)
        {
            return type switch
            {
                "textMessageEvent"              => MessageType.TextMessageEvent,
                "superChatEvent"                => MessageType.SuperChatEvent,
                "superStickerEvent"             => MessageType.SuperStickerEvent,
                "newSponsorEvent"               => MessageType.NewMemberEvent,
                "memberMilestoneChatEvent"      => MessageType.MemberMilestoneChatEvent,
                "membershipGiftingEvent"        => MessageType.MembershipGiftingEvent,
                "giftMembershipReceivedEvent"   => MessageType.GiftMembershipReceivedEvent,
                "sponsorOnlyModeStartedEvent"   => MessageType.MemberOnlyModeStartedEvent,
                "sponsorOnlyModeEndedEvent"     => MessageType.MemberOnlyModeEndedEvent,
                "messageDeletedEvent"           => MessageType.MessageDeletedEvent,
                "userBannedEvent"               => MessageType.UserBannedEvent,
                "chatEndedEvent"                => MessageType.ChatEndedEvent,
                "tombstone"                     => MessageType.Tombstone,
                _                               => MessageType.Unknown
            };
        }

        private void SetupSuperEvent(JSONNode item, string nodeName)
        {
            SuperEvent = new();
            SuperEvent.AmountDisplayString  = item["snippet"][nodeName]["amountDisplayString"];
            SuperEvent.Currency             = item["snippet"][nodeName]["currency"];
            SuperEvent.Tier                 = item["snippet"][nodeName]["tier"].AsInt;
            SuperEvent.AmountMicros         = ulong.Parse(item["snippet"][nodeName]["amountMicros"]);


            switch (nodeName)
            {
                case "superChatDetails":
                    Message = $"{SuperEvent.AmountDisplayString} - {item["snippet"]["superChatDetails"]["userComment"]}"; 
                    break;

                case "superStickerDetails":
                    Message = $"{item["snippet"]["displayMessage"]}!"; // {Currency} {value} from {username}: \"Shiba dog shouting good luck in a megaphone\"
                    break;
            }
        }

        private void SetupMemberUpdateEvent(JSONNode item, string nodeName)
        {
            MemberUpdate = new();

            switch (nodeName)
            {
                case "memberMilestoneChatDetails":
                    MemberUpdate.MemberType = MembershipType.Self;
                    MemberUpdate.MemberLevelName = item["snippet"][nodeName]["memberLevelName"];

                    MemberUpdate.MemberMonth = uint.Parse(item["snippet"][nodeName]["memberMonth"]);
                    Message = item["snippet"]["displayMessage"];
                    break;

                case "newSponsorDetails":
                    MemberUpdate.MemberType = MembershipType.Self;
                    MemberUpdate.MemberLevelName = item["snippet"][nodeName]["memberLevelName"];

                    MemberUpdate.MembershipUpgraded = item["snippet"][nodeName]["isUpgrade"].AsBool;
                    Message = item["snippet"]["displayMessage"];
                    break;

                case "membershipGiftingDetails":
                    MemberUpdate.MemberType = MembershipType.Gifted;
                    MemberUpdate.MemberLevelName = item["snippet"][nodeName]["giftMembershipsLevelName"];

                    MemberUpdate.MembershipsGifted = item["snippet"][nodeName]["giftMembershipsCount"].AsInt;

                    Message = $"{item["snippet"]["displayMessage"]}! - {MemberUpdate.MemberLevelName}";

                    break;

                case "giftMembershipReceivedDetails":
                    MemberUpdate.MemberType = MembershipType.Received;
                    MemberUpdate.MemberLevelName = item["snippet"][nodeName]["memberLevelName"];

                    Message = $"{item["snippet"]["displayMessage"]}! - {MemberUpdate.MemberLevelName}";
                    break;
            }
        }
    }

    /*
    ---
    ---
    Extension classes and enums
    ---
    ---
    */

    /// <summary> Representation of special privilege parameters, which should always be available for each message. </summary>
    public class AuthorDetails
    {
        public bool IsVerified;         // Checkmark
        public bool IsOwner;            // Livestreamer
        public bool IsMember;           // Member of the channel
        public bool IsModerator;        // Moderator of the channel
    }

    /// <summary> Representation of SuperChat and SuperSticker events. </summary>
    public class SuperEvent
    {
        public string AmountDisplayString;
        public string Currency;
        public ulong AmountMicros;
        public int Tier;                    // Tier color, goes from 1 to 7
    }

    /// <summary> Representation of membership related events. </summary>
    public class MemberUpdate
    {
        public MembershipType MemberType;
        public string MemberLevelName;
        public uint MemberMonth = 1;
        public int MembershipsGifted = 0;
        public bool MembershipUpgraded = false;
    }

    public enum MessageType
    {
        TextMessageEvent,               // A user has sent a text message.

        SuperChatEvent,                 // A user has purchased a Super Chat.
        SuperStickerEvent,              // A user has purchased a Super Sticker.

        NewMemberEvent,                 // A user has become a new member or upgraded.
        MemberMilestoneChatEvent,       // A member has renewed their membership.
        MembershipGiftingEvent,         // A user has purchased memberships for other viewers,
        GiftMembershipReceivedEvent,    // A user has received a gift membership.

        MemberOnlyModeStartedEvent,     //TODO: Members-only mode enabled; only members can send messages. This event has no display content.
        MemberOnlyModeEndedEvent,       //TODO: Members-only mode disabled; everyone can send messages. This event does not have any display content.

        MessageDeletedEvent,            //TODO: A message has been deleted by a moderator. The author field contains the moderator's details. This event does not have any display content.
        UserBannedEvent,                //TODO: A user has been banned by a moderator. The author field contains the moderator's details.
        ChatEndedEvent,                 //TODO: The chat has ended and no more messages can be inserted after this one. This will occur naturally a little while after a broadcast ends. Note that this type of message is not currently sent for live chats on a channel's default broadcast.
        Tombstone,                      //TODO: A message that used to exist with this id and publish time, but has been deleted. It is not sent upon deletion of a message, but rather is shown to signify where the message used to be before deletion. Only the snippet.liveChatId, snippet.type, and snippet.publishedAt fields are present in this type of message.

        Unknown                         // A new type may have been added that is currently not handled.
    }

    public enum MembershipType
    {
        Self,
        Gifted,
        Received
    }

}

/*
Chat things that I have never seen, but could be used here:
----------------------------------------------------------------------

Direct access to polls from Youtube, so you can visualize its results either here, or build your own custom polling
{
  "kind": "youtube#liveChatMessage",
  "etag": etag,
  "id": string,
  "snippet":
   {
    "type": string,                     USED
    "liveChatId": string,               USED
    "authorChannelId": string,          USED
    "publishedAt": datetime,            USED
    "hasDisplayContent": boolean,       UNUSED
    "displayMessage": string,           USED
    "textMessageDetails":
    {
      "messageText": string             UNNEEDED
    },
    "messageDeletedDetails":
    {
      "deletedMessageId": string        UNUSED
    },
    "userBannedDetails":
    {
      "bannedUserDetails":
      {
        "channelId": string,            UNUSED
        "channelUrl": string,           UNUSED
        "displayName": string,          UNUSED
        "profileImageUrl": string       UNUSED
      },
      "banType": string,                        UNUSED
      "banDurationSeconds": unsigned long       UNUSED
    },
    "memberMilestoneChatDetails":
    {
      "userComment": string,                    USED
      "memberMonth": unsigned integer,          USED
      "memberLevelName": string                 USED
    },
    "newSponsorDetails":
    {
      "memberLevelName": string,        USED
      "isUpgrade": bool                 USED 
    },
    "superChatDetails":
    {
      "amountMicros": unsigned long,        USED
      "currency": string,                   USED
      "amountDisplayString": string,        USED
      "userComment": string,                USED
      "tier": unsigned integer              USED
    },
    "superStickerDetails":
    {
      "superStickerMetadata":
      {
        "stickerId": string,        UNUSED
        "altText": string,          UNUSED
        "language": string          UNUSED
      },
      "amountMicros": unsigned long,        USED
      "currency": string,                   USED
      "amountDisplayString": string,        USED
      "tier": unsigned integer              USED
    },
    "membershipGiftingDetails":
    {
      "giftMembershipsCount": integer,          USED
      "giftMembershipsLevelName": string        USED
    },
    "giftMembershipReceivedDetails":
    {
      "memberLevelName": string,                            USED
      "gifterChannelId": string,                            UNUSED
      "associatedMembershipGiftingMessageId": string        UNUSED
    }
  },
  "authorDetails":
  {
    "channelId": string,            USED
    "channelUrl": string,           UNUSED
    "displayName": string,          USED
    "profileImageUrl": string,      USED
    "isVerified": boolean,          USED
    "isChatOwner": boolean,         USED
    "isChatSponsor": boolean,       USED
    "isChatModerator": boolean      USED
  }
}
*/
