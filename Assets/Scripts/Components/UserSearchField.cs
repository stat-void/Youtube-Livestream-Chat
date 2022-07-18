using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using System.Threading.Tasks;

public class UserSearchField : MonoBehaviour
{

    //[SerializeField] protected SearchUserItem UserPrefab;
    //[SerializeField] protected Transform ObjectPoolVisual;  // The spot where unused pool items are stored on the visual side
    //private List<SearchUserItem> _objectPool = new();       // And the spot from where unused items are accessed from.


    [Header("Adding users")]
    [SerializeField] protected AddUserSearcher AddSearcher;
    

    [Header("Removing users")]
    //[SerializeField] protected TMP_InputField RemoveDirectInput;
    [SerializeField] protected TMP_InputField RemoveSearchInput;
    //[SerializeField] protected Button RemoveDirectButton;
    [SerializeField] protected Button RemoveRefreshButton;
    // TODO: A RecyclerView list
    // TODO: And a good way to access its contents



    

    //private SortedDictionary<string, List<SearchUserItem>> _availableUsers = new();
    private List<SearchUserItem> _addedUsers = new();


    public void Start()
    {
        //RemoveDirectButton.onClick.AddListener(OnRemoveDirectClicked);
        //RemoveDirectInput.onValueChanged.AddListener(UpdateRemoveSearch);
    }

    public void OpenRefresh()
    {
        AddSearcher.RefreshData();
    }


    /*private void OnAddDirectClicked()
    {
        //TODO: Take the text from DirectInput and attempt to add a user through that,
        //TODO: If none exist, then some form of half-state where it searches for user with the matching name, to extract their ID?
        throw new NotImplementedException();
    }*/

    /*private void OnRemoveDirectClicked()
    {
        throw new NotImplementedException();
    }*/

    

    /*private void UpdateRemoveSearch(string input)
    {
        throw new NotImplementedException();
    }*/

    


    /*
    ---
    ---
    RecyclerView visualization related content
    ---
    ---
    */


    

    /// <summary> Clear the current list and refills it by seeing if the names of the assets contain the given value </summary>
    /*private void AddSearchList(string value)
    {
        string valueClean = value.Trim().ToLower();

        displayedList.Clear();
        displayedListHighlights.Clear();

        foreach (Asset asset in selectedList)
        {
            int idx = asset.FriendlyName.ToLower().IndexOf(valueClean);
            if (idx != -1 || valueClean.Length == 0)
            {
                displayedList.Add(asset);
                displayedListHighlights.Add((idx, valueClean.Length));
            }
        }

        ClearVisibleLines();
        ForceScrollReset();
    }*/

}
