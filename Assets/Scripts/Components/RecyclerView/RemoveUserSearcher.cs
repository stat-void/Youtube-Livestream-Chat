using UnityEngine;

public class RemoveUserSearcher : UserSearcher
{
    public override void SetSelection(object target)
    {
        InvokeSelection(target as SearchUserItem, SelectionType.Remove);
    }

    public override void OnDirectButtonClick()
    {
        InvokeSelection(DirectInput.text, SelectionType.DirectRemove);
        DirectInput.text = "";
    }

    protected override void LoopUsers()
    {
        // Remover relies directly on the user list, so only clear display lists.
        DisplayList.Clear();
        DisplayListHighlights.Clear();

        Users.Sort();
    }

    public void AddUser(UserContainer user)
    {
        // If no matching username exists, initialize and fill new dict key
        if (!UsernameIDDict.ContainsKey(user.Username))
        {
            Users.Add(user);
            UsernameIDDict[user.Username] = new() { user.ID };
            RefreshUsers();
            return;
        }

        // Username exists, check if a currently ID'less user exists.
        UserContainer container = FindUnselectedUser(user.Username);
        if (container != null)
        {
            container.ID = user.ID;
            container.Highlighted = true;
            UsernameIDDict[user.Username].Add(user.ID);
            RefreshUsers();
            RefreshHighlights();
            return;
        }

        // Otherwise, only add if a matching ID is not already in the list
        if (!UsernameIDDict[user.Username].Contains(user.ID))
        {
            Users.Add(user);
            UsernameIDDict[user.Username].Add(user.ID);
            RefreshUsers();
            return;
        }
            
    }

    public void AddUser(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return;

        Users.Add(new("", username, false));
        RefreshUsers();

        if (!UsernameIDDict.ContainsKey(username))
            UsernameIDDict[username] = new();
    }

    public void RemoveUser(UserContainer container)
    {
        int toRemove = -1;
        for (int i = 0; i < Users.Count; i++)
        {
            // Compare ID's if not current checkable ID is not empty.
            bool matchID = !string.IsNullOrEmpty(container.ID) && Users[i].ID == container.ID;

            // Compare usernames, if the data being compared against also does not have an ID. 
            bool matchUsername = string.IsNullOrEmpty(Users[i].ID) && Users[i].Username == container.Username;

            if (matchID || matchUsername)
            {
                toRemove = i;
                break;
            }
        }

        if (toRemove >= 0)
        {
            Users.RemoveAt(toRemove);
            RefreshUsers();

            UsernameIDDict[container.Username].Remove(container.ID);
        }
    }

    public void RemoveUser(string name)
    {
        int toRemove = -1;
        for (int i = 0; i < Users.Count; i++)
        {
            if (Users[i].Username == name)
            {
                toRemove = i;
                break;
            }
        }

        if (toRemove >= 0)
        {
            string id = Users[toRemove].ID;
            Users.RemoveAt(toRemove);
            RefreshUsers();

            UsernameIDDict[name].Remove(id);
        }
    }
}
