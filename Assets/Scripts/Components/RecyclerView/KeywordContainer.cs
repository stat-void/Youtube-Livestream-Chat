using System;

public class KeywordContainer : IComparable<string>, IComparable<KeywordContainer>
{
    public string Keyword = "";
    public bool Highlighted = false;

    public KeywordContainer(string keyword)
    {
        Keyword = keyword;
    }

    public KeywordContainer(string keyword, bool highlight)
    {
        Keyword = keyword;
        Highlighted = highlight;
    }

    /// <summary> Comparison operator to have messages be sortable through usernames. </summary>
    public int CompareTo(string other)
    {
        return Keyword.CompareTo(other);
    }

    public int CompareTo(KeywordContainer other)
    {
        return Keyword.CompareTo(other.Keyword);
    }
}
