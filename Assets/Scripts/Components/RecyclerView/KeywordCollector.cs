using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeywordCollector : ARecyclerList
{
    [Header("Keyword Collector UI Components")]
    [SerializeField] protected Button RefreshButton;
    [SerializeField] protected Image RefreshImage;

    [SerializeField] protected TMP_InputField DirectInput;
    [SerializeField] protected Button AddButton;

    /// <summary> Verification Hashset of keyword strings. </summary>
    public HashSet<string> KeywordSet { get; protected set; } = new(); 

    /// <summary> The list that is used simultaneously used for data collection and displaying. </summary>
    protected readonly List<KeywordContainer> KeywordDisplayList = new();

    /// <summary> The list used to hold internal keyword data. </summary>
    protected readonly List<KeywordContainer> Keywords = new();                 


    private bool _refreshingList = false;
    private bool _quitting = false;
    private IEnumerator _currentResize = null;
    private float _resizeWaitSeconds = 0f;


    public static event Action<string, SelectionType> OnDirectKeywordSelection;
    public static event Action<KeywordItem, SelectionType> OnKeywordSelection;


    private void Start()
    {
        // Size of the virtual list depends on sheet size from SetupSheet
        SetupRecyclerList(true);
    }

    private void OnApplicationQuit()
    {
        _quitting = true;
    }

    public void Open()
    {
        RefreshButton.onClick.AddListener(OnRefreshButtonClick);
        AddButton.onClick.AddListener(OnAddButtonClick);
        ScreenResizeListener.OnResize += OnScreenResize;
    }

    public void Close()
    {
        RefreshButton.onClick.RemoveListener(OnRefreshButtonClick);
        AddButton.onClick.RemoveListener(OnAddButtonClick);
        ScreenResizeListener.OnResize -= OnScreenResize;
    }

    public List<KeywordContainer> GetKeywords()
        => Keywords;

    /// <summary> A keyword was selected. In the case of KeywordCollector, this means removing the item from the list. </summary>
    public override void SetSelection(object target)
    {
        InvokeSelection(target as KeywordItem, SelectionType.Remove);
    }

    /// <summary> Remove a keyword whose clickable element was tapped on </summary>
    /// <param name="container">The clicked container.</param>
    public void RemoveKeyword(KeywordContainer container)
    {
        int toRemove = -1;
        for (int i = 0; i < Keywords.Count; i++)
        {
            if (container.Keyword == Keywords[i].Keyword)
            {
                toRemove = i;
                break;
            }
        }

        if (toRemove >= 0)
        {
            Keywords.RemoveAt(toRemove);
            RefreshKeywords();
            KeywordSet.Remove(container.Keyword);
        }
    }

    /// <summary> The button to convert written text into a recycler item was clicked.</summary>
    public void OnAddButtonClick()
    {
        InvokeSelection(DirectInput.text, SelectionType.DirectAdd);
        DirectInput.text = "";
    }

    public void AddKeyword(string keyword)
    {
        keyword = keyword.Trim();
        keyword = keyword.ToLower();

        if (string.IsNullOrWhiteSpace(keyword))
            return;

        if (KeywordSet.Contains(keyword))
            return;

        Keywords.Add(new(keyword));
        RefreshKeywords();

        KeywordSet.Add(keyword);
    }

    /// <summary> Called whenever the refresh button is manually clicked on. </summary>
    private void OnRefreshButtonClick() 
        => RefreshKeywords();

    public void RefreshData() 
        => RefreshKeywords();

    protected void InvokeSelection(KeywordItem item, SelectionType type) =>
        OnKeywordSelection?.Invoke(item, type);

    protected void InvokeSelection(string keyword, SelectionType type) =>
        OnDirectKeywordSelection?.Invoke(keyword, type);

    protected override object GetDataAt(int i)
    {
        if (KeywordDisplayList.Count > 0 && KeywordDisplayList.Count > i)
            return KeywordDisplayList[i];
        return null;
    }

    protected override (int, int) GetHighlightAt(int i)
    {
        // Keyword Collector does not need highlighting, since there is no searcher being used.
        return (0, 0);
    }

    protected override int GetDataCount()
        => KeywordDisplayList.Count;

    public enum SelectionType
    {
        None,
        DirectAdd,
        Remove
    }


    //
    // Commands related to whenever screen resizing is involved
    //
    private void OnScreenResize()
    {
        if (_quitting)
            return;

        _resizeWaitSeconds = 0.1f;

        if (_currentResize == null)
        {
            _currentResize = RefreshFit();
            StartCoroutine(_currentResize);
        }
    }

    private IEnumerator RefreshFit()
    {
        while (_resizeWaitSeconds > 0)
        {
            yield return null;
            _resizeWaitSeconds -= Time.unscaledDeltaTime;
        }

        _resizeWaitSeconds = 0;

        RefreshKeywords();

        _currentResize = null;
        yield break;
    }

    protected async void RefreshKeywords()
    {
        RefreshButton.interactable = false;
        _refreshingList = true;

        _ = RotateRefreshIcon();
        await Task.Run(LoopUsers);

        if (RefreshButton != null)
            RefreshButton.interactable = true;
        _refreshingList = false;

        ClearVisibleItems();
        CleanList();
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

    /// <summary> Clear the current keyword display list and refill it. </summary>
    /// <param name="value"> The keyword to compare names against. </param>
    protected void CleanList()
    {
        KeywordDisplayList.Clear();

        foreach (KeywordContainer keyword in Keywords)
        {
            KeywordDisplayList.Add(keyword);
        }

        // With the newly updated list, refresh content visible to the user.
        ClearVisibleItems();
        ForceScrollReset();
    }

    protected void LoopUsers()
    {
        KeywordDisplayList.Clear();
        Keywords.Sort();
    }
}
