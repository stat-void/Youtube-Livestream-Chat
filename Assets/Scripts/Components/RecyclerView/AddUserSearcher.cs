using System;
using System.Collections.Generic;
using UnityEngine;

public class AddUserSearcher : UserSearcher
{
    public override void SetSelection(object target)
    {
        InvokeSelection(target as SearchUserItem, SelectionType.Add);
    }

    public override void OnDirectButtonClick()
    {
        InvokeSelection(DirectInput.text, SelectionType.DirectAdd);
        DirectInput.text = "";
    }

    protected override void LoopUsers()
    {
        DisplayList.Clear();
        DisplayListHighlights.Clear();

        // Get a list of all the items
        foreach (KeyValuePair<string, List<string>> kvPair in ChatMessageListener.UsernameIDPairs)
        {
            // A completely new username was found
            if (!UsernameIDDict.ContainsKey(kvPair.Key))
            {
                UsernameIDDict[kvPair.Key] = kvPair.Value;

                foreach (string id in kvPair.Value)
                    Users.Add(new(id, kvPair.Key));
            }

            // A new ID was found
            else
            {
                foreach (string id in kvPair.Value)
                {
                    if (!UsernameIDDict[kvPair.Key].Contains(id))
                    {
                        UsernameIDDict[kvPair.Key].Add(id);
                        Users.Add(new(id, kvPair.Key));
                    }
                }
            }
        }

        Users.Sort();
    }

    public UserContainer FindUserByID(string id)
    {
        foreach (UserContainer user in Users)
            if (user.ID == id)
                return user;
        return null;
    }
}
