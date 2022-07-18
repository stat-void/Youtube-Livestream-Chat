using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using System.Threading.Tasks;

public class AddUserSearcher : ARecyclerList
{
    [Header("UI Components")]
    [SerializeField] protected TMP_InputField SearchField;
    [SerializeField] protected Button RefreshButton;
    [SerializeField] protected Image RefreshImage;

    //[SerializeField] protected TMP_InputField DirectInput;
    //[SerializeField] protected Button DirectButton;


    // Fields related to the content lists and the actual displayed list
    //private SortedDictionary<string, List<string>> _usernameIDPairs = new();
    private List<UserContainer> _users = new();        // Data list
    private List<UserContainer> _displayList = new();          // Search affected list of users
    private List<(int, int)> _displayListHighlights = new();    // Search highlights for users

    private string _currentSearch = "";
    

    private void Start()
    {
        // Size of the virtual list depends on sheet size from SetupSheet
        SetupRecyclerList(true);

        //AddDirectButton.onClick.AddListener(OnAddDirectClicked);
        SearchField.onValueChanged.AddListener(OnSearchUpdate);
        RefreshButton.onClick.AddListener(OnAddRefreshClicked);
    }


    public async override void SetSelection(object target)
    {
        var item = target as UserContainer;

        Debug.Log($"user {item.Username} was clicked.");
        //TODO: Depending on if this is for adding or removing, the main component tying the 2 recyclerviews
        //TODO: Should react accordingly... So send an event?

        // Then, the main component decides if the given thing is to be highlighted or removed or added to a list.
    }

    
    /// <summary> Called on search field updates. </summary>
    private void OnSearchUpdate(string value) =>
        SearchList(value);
    

    /// <summary> Clear the current list and refill it by seeing if usernames contain the given value. </summary>
    /// <param name="value"> The keyword to compare names against. </param>
    private void SearchList(string value)
    {
        string valueClean = value.Trim().ToLower();
        _currentSearch = valueClean;

        _displayList.Clear();
        _displayListHighlights.Clear();

        foreach (UserContainer user in _users)
        {
            int idx = user.Username.ToLower().IndexOf(valueClean);
            if (idx != -1 || valueClean.Length == 0)
            {
                _displayList.Add(user);
                _displayListHighlights.Add((idx, valueClean.Length));
            }
        }

        ClearVisibleLines();
        ForceScrollReset();
    }

    public void RefreshData() =>
        RefreshUsers();

    /// <summary> Called whenever the refresh button is manually clicked on. </summary>
    private void OnAddRefreshClicked() =>
        RefreshUsers();

    private async void RefreshUsers()
    {
        RefreshButton.interactable = false;
        //TODO: Start rotating the button icon


        //TODO: 1. Remove all visible object poolable elements in AddUsers? Maybe RemoveUsers just in case too?

        //YTMessages = ChatMessageListener.IDLastMessagePairs;

        //TODO: Switch icon to something animated and non-interactable for the refresh icon
        await Task.Run(LoopUsers);

        //_availableUsers.Sort(); // Since you're now a SortedDictionary, I don't have to worry?
        //_addedUsers.Sort();

        RefreshButton.interactable = true;
        //TODO: Stop rotating the button icon. Maybe reset rotation too?
        ClearVisibleLines();
        SearchList(_currentSearch);
    }

    private void LoopUsers()
    {
        _users.Clear();
        _displayList.Clear();
        _displayListHighlights.Clear();

        // Get a list of all the items
        foreach (KeyValuePair<string, List<string>> kvPair in ChatMessageListener.UsernameIDPairs)
        {
            foreach (string id in kvPair.Value)
            {
                _users.Add(new(id, kvPair.Key));
            }
        }
    }

    protected override object GetDataAt(int i)
    {
        if (0 < _displayList.Count && _displayList.Count > i)
            return _displayList[i];
        return null;
    }

    protected override (int, int) GetHighlightAt(int i)
    {
        if (0 < _displayList.Count && _displayList.Count > i)
            return _displayListHighlights[i];
        return (0, 0);
    }

    protected override int GetDataCount()
    {
        return _displayList.Count;
    }
}
