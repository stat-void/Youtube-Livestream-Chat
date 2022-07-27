using System;

public class UserContainer : IComparable<string>, IComparable<UserContainer>
{
    public string ID = "";
    public string Username = "";

    public bool Highlighted = false;


    public UserContainer(string id, string username)
    {
        ID = id;
        Username = username;
    }

    public UserContainer(string id, string username, bool highlight)
    {
        ID = id;
        Username = username;
        Highlighted = highlight;
    }


    /// <summary> Comparison operator to have messages be sortable through usernames. </summary>
    public int CompareTo(string other)
    {
        return Username.CompareTo(other);
    }

    public int CompareTo(UserContainer other)
    {
        return Username.CompareTo(other.Username);
    }
}
