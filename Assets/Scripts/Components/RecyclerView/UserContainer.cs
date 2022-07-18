using System;

public class UserContainer : IComparable<string>
{
    public string ID = "";
    public string Username = "";


    public UserContainer(string id, string username)
    {
        ID = id;
        Username = username;
    }


    /// <summary> Comparison operator to have messages be sortable through usernames. </summary>
    public int CompareTo(string other)
    {
        return Username.CompareTo(other);
    }
}
