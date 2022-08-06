using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserSearcher : ARecyclerList
{
    [Header("User Searcher UI Components")]
    [SerializeField] protected TMP_InputField SearchField;
    [SerializeField] protected Button RefreshButton;
    [SerializeField] protected Image RefreshImage;

    [SerializeField] protected TMP_InputField DirectInput;
    [SerializeField] protected Button DirectButton;


    // Fields related to the content lists and the actual displayed list
    public Dictionary<string, List<string>> UsernameIDDict { get; protected set; } = new(); // Verification list
    protected readonly List<UserContainer> Users = new();                 // Data list
    protected readonly List<UserContainer> DisplayList = new();           // Search affected visible list of users
    protected readonly List<(int, int)> DisplayListHighlights = new();    // Search highlights for users

    private string _currentSearch = "";
    private bool _refreshingList = false;

    public static event Action<SearchUserItem, SelectionType> OnUserSelection;
    public static event Action<string, SelectionType> OnDirectUserSelection;


    private void Start()
    {
        // Size of the virtual list depends on sheet size from SetupSheet
        SetupRecyclerList(true);

        SearchField.onValueChanged.AddListener(OnSearchUpdate);
        RefreshButton.onClick.AddListener(OnAddRefreshClicked);
        DirectButton.onClick.AddListener(OnDirectButtonClick);
        ScreenResizeListener.OnResize += OnScreenResize;
    }


    public override void SetSelection(object target)
    {
        InvokeSelection(target as SearchUserItem, SelectionType.None);
    }
    
    public virtual void OnDirectButtonClick()
    {
        InvokeSelection(DirectInput.text, SelectionType.None);
        DirectInput.text = "";
    }

    protected void InvokeSelection(SearchUserItem item, SelectionType type) =>
        OnUserSelection?.Invoke(item, type);

    protected void InvokeSelection(string username, SelectionType type) =>
        OnDirectUserSelection?.Invoke(username, type);

    protected virtual void LoopUsers()
    {
    }
    
    /// <summary> Called on search field updates. </summary>
    private void OnSearchUpdate(string value) =>
        SearchList(value);
    

    /// <summary> Clear the current list and refill it by seeing if usernames contain the given value. </summary>
    /// <param name="value"> The keyword to compare names against. </param>
    protected void SearchList(string value)
    {
        string valueClean = value.Trim().ToLower();
        _currentSearch = valueClean;

        DisplayList.Clear();
        DisplayListHighlights.Clear();

        foreach (UserContainer user in Users)
        {
            int idx = user.Username.ToLower().IndexOf(valueClean);
            if (idx != -1 || valueClean.Length == 0)
            {
                DisplayList.Add(user);
                DisplayListHighlights.Add((idx, valueClean.Length));
            }
        }

        // With the newly updated list, refresh content visible to the user.
        ClearVisibleItems();
        ForceScrollReset();
    }

    public void RefreshData() =>
        RefreshUsers();

    /// <summary> Called whenever the refresh button is manually clicked on. </summary>
    private void OnAddRefreshClicked() =>
        RefreshUsers();

    private void OnScreenResize(Vector2 arg1, Vector2 arg2) =>
        // TODO: Maybe make this a timer based wait before rushing ahead, any resize resets timer, and then once passed, calls refresh.
        RefreshUsers();
    

    protected async void RefreshUsers()
    {
        if (ChatMessageListener.UsernameIDPairs.Count == 0)
            return;

        RefreshButton.interactable = false;
        _refreshingList = true;

        _ = RotateRefreshIcon();
        await Task.Run(LoopUsers);

        if (RefreshButton != null)
            RefreshButton.interactable = true;
        _refreshingList = false;

        ClearVisibleItems();
        SearchList(_currentSearch);
    }

    public void RefreshHighlights()
    {
        // Refresh highlights, but ignore top and bottom recycler paddings.
        for (int i = 1; i < ContentParent.childCount - 1; i++)
            ContentParent.GetChild(i).GetComponent<SearchUserItem>().RefreshHighlight();
    }

    public UserContainer FindUnselectedUser(string username)
    {
        bool passed = false;

        for (int i = 0; i < Users.Count; i++)
        {
            bool match = false;
            UserContainer user = Users[i];

            if (user.Username == username)
            {
                passed = true;
                match = true;

                if (!user.Highlighted)
                    return user;
            }

            if (passed && !match)
                break;
        }
        return null;
    }

    public UserContainer FindUser(string username)
    {
        for (int i = 0; i < Users.Count; i++)
        {
            if (Users[i].Username == username)
                return Users[i];
        }
        return null;
    }

    protected async Task RotateRefreshIcon()
    {
        while (_refreshingList)
        {
            RefreshImage.transform.Rotate(new Vector3(0, 0, Time.unscaledDeltaTime * 360));
            await Task.Yield();
        }

        RefreshImage.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    protected override object GetDataAt(int i)
    {
        if (0 < DisplayList.Count && DisplayList.Count > i)
            return DisplayList[i];
        return null;
    }

    protected override (int, int) GetHighlightAt(int i)
    {
        if (0 < DisplayList.Count && DisplayList.Count > i)
            return DisplayListHighlights[i];
        return (0, 0);
    }

    protected override int GetDataCount()
    {
        return DisplayList.Count;
    }

    public enum SelectionType
    {
        None,
        Add,
        Remove,
        DirectAdd,
        DirectRemove
    }
}
