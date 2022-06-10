using UnityEngine;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using TMPro;

public class ChatItem : AListItem
{
    [SerializeField] protected RectTransform RectTransform;
    [SerializeField] protected RectTransform Separator;
    [SerializeField] protected TMP_Text Timestamp;
    [SerializeField] protected TMP_Text UserData;

    private const string _regularUserColor = "#FFFFFF";
    private const string _memberUserColor = "#00E624";
    private const string _moderatorColor = "#007FFF";
    private const string _ownerColor = "#FFE000";

    public override void Bind(YoutubeChatMessage node, Transform active)
    {
        gameObject.SetActive(true);

        transform.SetParent(active);

        // Set Timestamp
        Timestamp.text = node.Timestamp.ToString("HH:mm:ss");

        // Prepare user color
        string userColor;

        if (node.AuthorDetails.IsOwner)
            userColor = _ownerColor;

        else if (node.AuthorDetails.IsModerator)
            userColor = _moderatorColor;
        

        else if (node.AuthorDetails.IsMember)
            userColor = _memberUserColor;

        else
            userColor = _regularUserColor;

        // Set contents
        UserData.text = $"<color={userColor}>{node.Username}</color>  {node.Message}";

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


}
