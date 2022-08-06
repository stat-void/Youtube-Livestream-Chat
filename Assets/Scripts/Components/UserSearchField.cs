using UnityEngine;
using UnityEngine.UI;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public class UserSearchField : MonoBehaviour
{
    [SerializeField] protected AddUserSearcher AddSearcher;
    [SerializeField] protected RemoveUserSearcher RemoveSearcher;
    [SerializeField] protected Toggle ModToggle;
    [SerializeField] protected Toggle MemberToggle;

    private void Start()
    {
        UserSearcher.OnUserSelection += OnUserClick;
        UserSearcher.OnDirectUserSelection += OnDirectSearchClick;
    }

    public void OpenRefresh()
    {
        AddSearcher.Open();
        RemoveSearcher.Open();
        AddSearcher.RefreshData();
        RemoveSearcher.RefreshData();
    }

    public void CloseRefresh()
    {
        AddSearcher.Close();
        RemoveSearcher.Close();
    }

    public bool IsUserValid(YoutubeChatMessage message)
    {
        if (ModToggle.isOn      && (message.AuthorDetails.IsModerator || message.AuthorDetails.IsOwner) ||
            MemberToggle.isOn   &&  message.AuthorDetails.IsMember)
            return true;

        string username = message.Username;
        string id = message.ChannelID;

        // If no username exists, return false.
        if (!RemoveSearcher.UsernameIDDict.ContainsKey(username))
            return false;

        // Username exists, check if matching ID exists too
        if (RemoveSearcher.UsernameIDDict[username].Contains(id))
            return true;

        // Matching ID does not exist, check if username with no ID exists
        UserContainer user = RemoveSearcher.FindUnselectedUser(username);
        if (user != null)
        {
            RemoveSearcher.UsernameIDDict[username].Add(id);
            user.ID = id;
            user.Highlighted = true;
            RemoveSearcher.RefreshHighlights();
            return true;
        }

        return false;
    }

    /// <summary> Cases where add/remove was done through clicking on a SearchUserItem element. </summary>
    private void OnUserClick(SearchUserItem item, UserSearcher.SelectionType type)
    {
        if (type == UserSearcher.SelectionType.Add)
        {
            // Adder - Highlight the item just clicked
            item.UpdateHighlightState(true);

            // Remover - Add directly to list of users in a highlighted state
            RemoveSearcher.AddUser(item.User);
        }

        else if (type == UserSearcher.SelectionType.Remove)
        {
            // Adder - Delight from list, also check if there is a visible element that uses this
            UserContainer container = AddSearcher.FindUserByID(item.User.ID);
            if (container != null)
            {
                container.Highlighted = false;
                AddSearcher.RefreshHighlights();
            }

            // Remover
            RemoveSearcher.RemoveUser(item.User);
        }

        
    }
    /// <summary> Cases where add/remove is from writing an username directly. </summary>
    private void OnDirectSearchClick(string username, UserSearcher.SelectionType type)
    {
        if (type == UserSearcher.SelectionType.DirectAdd)
        {
            // Data setup
            UserContainer container = AddSearcher.FindUnselectedUser(username);

            if (container != null)
            {
                // Adder - Data exists, update highlight and list visibility
                container.Highlighted = true;
                AddSearcher.RefreshHighlights();

                // Remover - Add directly to list of users in a highlighted state
                RemoveSearcher.AddUser(container);
            }

            else
            {
                // Remover - Add blank ID user container
                RemoveSearcher.AddUser(username);
            }

        }

        else if (type == UserSearcher.SelectionType.DirectRemove)
        {
            // Data setup
            UserContainer container = AddSearcher.FindUser(username);

            if (container != null)
            {
                // Adder - Data exists, update highlight and list visibility
                container.Highlighted = false;
                AddSearcher.RefreshHighlights();

                // Remover - Remove directly from list of users
                RemoveSearcher.RemoveUser(container);
            }

            else
            {
                // Remover - Attempt to find and remove by username
                RemoveSearcher.RemoveUser(username);
            }
        }
    }
}
