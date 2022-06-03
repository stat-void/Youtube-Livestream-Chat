using UnityEngine;
using SimpleJSON;
using System;

namespace Void.YoutubeAPI.LiveStreamChat.Messages
{
    public class YoutubeChatMessage
    {
        /// <summary> Message type, such as regular text message, Super Chat, Membership etc. </summary>
        public MessageEventType Type; 

        // Default parameters available for every message
        public string ChannelID;
        public string Username;
        public string ProfileImageURL;
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
                case MessageEventType.TextMessageEvent:
                    Message = item["snippet"]["displayMessage"];
                    break;

                case MessageEventType.SuperChatEvent:
                    SetupSuperEvent(item, "superChatDetails");
                    break;

                case MessageEventType.SuperStickerEvent:
                    SetupSuperEvent(item, "superStickerDetails");
                    break;

                case MessageEventType.NewMemberEvent:
                    SetupMemberUpdateEvent(item, "newSponsorDetails");
                    break;

                case MessageEventType.MemberMilestoneChatEvent:
                    SetupMemberUpdateEvent(item, "memberMilestoneChatDetails");
                    break;

                case MessageEventType.MembershipGiftingEvent:
                    SetupMemberUpdateEvent(item, "membershipGiftingDetails");
                    break;

                case MessageEventType.GiftMembershipReceivedEvent:
                    SetupMemberUpdateEvent(item, "giftMembershipReceivedDetails");
                    break;

                default:
                    Debug.Log($"Unchecked type specific field, any messages? - | {item["snippet"]["displayMessage"]} |");
                    break;
            }
        }

        private MessageEventType ParseTypeString(string type)
        {
            return type switch
            {
                "textMessageEvent"              => MessageEventType.TextMessageEvent,
                "superChatEvent"                => MessageEventType.SuperChatEvent,
                "superStickerEvent"             => MessageEventType.SuperStickerEvent,
                "newSponsorEvent"               => MessageEventType.NewMemberEvent,
                "memberMilestoneChatEvent"      => MessageEventType.MemberMilestoneChatEvent,
                "membershipGiftingEvent"        => MessageEventType.MembershipGiftingEvent,
                "giftMembershipReceivedEvent"   => MessageEventType.GiftMembershipReceivedEvent,
                "sponsorOnlyModeStartedEvent"   => MessageEventType.MemberOnlyModeStartedEvent,
                "sponsorOnlyModeEndedEvent"     => MessageEventType.MemberOnlyModeEndedEvent,
                "messageDeletedEvent"           => MessageEventType.MessageDeletedEvent,
                "userBannedEvent"               => MessageEventType.UserBannedEvent,
                "chatEndedEvent"                => MessageEventType.ChatEndedEvent,
                "tombstone"                     => MessageEventType.Tombstone,
                _                               => MessageEventType.Unknown
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
                    Message = item["snippet"]["superChatDetails"]["userComment"];
                    break;

                case "superStickerDetails":
                    Debug.Log($"Sticker event, any preset messages? - | {item["snippet"]["displayMessage"]} |");
                    Message = $"Sent a super sticker! {SuperEvent.AmountDisplayString}";
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

                    Message = item["snippet"]["memberMilestoneChatDetails"]["userComment"];
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

                    Debug.Log($"member gifting event, any preset messages? - | {item["snippet"]["displayMessage"]} |");
                    Message = $"Gifted memberships - {MemberUpdate.MemberLevelName} x{MemberUpdate.MembershipsGifted}!";

                    break;

                case "giftMembershipReceivedDetails":
                    MemberUpdate.MemberType = MembershipType.Received;
                    MemberUpdate.MemberLevelName = item["snippet"][nodeName]["memberLevelName"];

                    Debug.Log($"Gift receival event, any preset messages? - | {item["snippet"]["displayMessage"]} |");
                    Message = $"Has been gifted a membership - {MemberUpdate.MemberLevelName}!";
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
        public int Tier;
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

    public enum MessageEventType
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
https://developers.google.com/youtube/v3/live/docs/liveChatMessages#snippet.type

----------------------------------------------------------------------

Direct access to polls from Youtube, so you can visualize its results either here, or build your own custom polling
The main question is, where do you get information on the actual voting contents (question, poll strings) here?

Also note that this information is not available in Youtube's own data set

These should be prompts to recognize that these events were started... But, does the "owner" do these?
items[].snippet.pollOpenedDetails 	        OBJECT 	
items[].snippet.pollOpenedDetails.id 	    STRING 	
items[].snippet.pollOpenedDetails.prompt 	STRING 	
items[].snippet.pollClosedDetails 	        OBJECT 	
items[].snippet.pollClosedDetails.pollId 	STRING 	The id of the poll that was closed


These events are to directly see what users voted on, or if they edited results... is that possible?
items[].snippet.pollEditedDetails 	        OBJECT 	
items[].snippet.pollEditedDetails.id 	    STRING 	
items[].snippet.pollEditedDetails.prompt 	STRING

items[].snippet.pollVotedDetails 	        OBJECT 	
items[].snippet.pollVotedDetails.itemId 	STRING 	 The poll item the user chose
items[].snippet.pollVotedDetails.pollId 	STRING 	 The poll the user voted on


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
