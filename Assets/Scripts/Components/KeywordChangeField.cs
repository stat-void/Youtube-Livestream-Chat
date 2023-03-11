using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public class KeywordChangeField : MonoBehaviour
{
    [SerializeField] protected KeywordCollector KeywordCollector;
    [SerializeField] protected Toggle SuperChatToggle;
    [SerializeField] protected Toggle MembershipToggle;

    public void OpenRefresh()
    {
        KeywordCollector.Open();
        KeywordCollector.RefreshData();

        KeywordCollector.OnDirectKeywordSelection += OnKeywordClick;
        KeywordCollector.OnKeywordSelection += OnKeywordSelect;
    }

    public void CloseRefresh()
    {
        KeywordCollector.Close();

        KeywordCollector.OnDirectKeywordSelection -= OnKeywordClick;
        KeywordCollector.OnKeywordSelection -= OnKeywordSelect;
    }

    /// <summary> Confirm if the given YoutubeChatMessage matches the requirements for the Keyword Listener. </summary>
    /// <param name="message"> The given YoutubeChatMessage to check for validity. </param>
    /// <returns> bool to determine if valid, and a MultiRange list for highlighting - indexes and lengths.</returns>
    public (bool, MultiRange) IsKeywordValid(YoutubeChatMessage message)
    {
        if (SuperChatToggle.isOn && message.SuperEvent != null)
            return (true, new());

        if (MembershipToggle.isOn && message.MemberUpdate != null)
            return (true, new());

        MultiRange ranger = new();

        string contents = message.Message;
        contents = contents.ToLower();

        foreach (string keyword in KeywordCollector.KeywordSet)
        {
            Regex kwRegex = new(keyword);
            MatchCollection matches = kwRegex.Matches(contents);

            foreach (Match match in matches.Cast<Match>())
                ranger.Add(match.Index, match.Length);
        }

        if (ranger.Count > 0)
            return (true, ranger);

        return (false, ranger);
    }

    /// <summary> Cases where add/remove was done through clicking on a SearchUserItem element. </summary>
    private void OnKeywordClick(string keyword, KeywordCollector.SelectionType type)
    {
        // Add to list of keywords in lowercase. Highlighted state is unneeded.
        // Internally does not add it if identical. 
        if (type == KeywordCollector.SelectionType.DirectAdd)
        {
            KeywordCollector.AddKeyword(keyword);
        }
    }
    /// <summary> Cases where add/remove is from writing an username directly. </summary>
    private void OnKeywordSelect(KeywordItem item, KeywordCollector.SelectionType type)
    {
        // Remove from list of keywords.
        if (type == KeywordCollector.SelectionType.Remove)
        {
            KeywordCollector.RemoveKeyword(item.Keyword);
        }
    }
}
