using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Note to self - This list system has a bug. 
If scrolling too aggressively, the first one/two/maybe more names end up disappearing, possibly the bottom of the list too?
This is a purely visual bug, using the search list reveals them again and refreshing reveals them again.
From what I've noticed, the reason is affected by these 2 lines:

float windowTop = Mathf.Lerp(-(_listHeight - _viewAreaHeight), 0, scrollPosition.y);
float windowBottom = windowTop - _viewAreaHeight;

it seems that some value in these goes out of sync, and the from the data list, something with the index 1 or 2 ends up getting
larger values, causing the most important check to never reach the start of the display list.

If element 0 is not visible, and the size of each element is 50, then in order for it to be revealed, a threshold of -25 must be passed.
(center is 0, size is 50, bottom would then be 25 and top -25)

But, in the process of something going out of sync, the blocking elements gets a threshold of 25 instead.

*/

public abstract class ARecyclerList : MonoBehaviour, IRecyclerListPresenter
{

    [Header("Vertical Recycling List")]
    [SerializeField] protected RectTransform ContentParent;
    [SerializeField] protected RectTransform PoolParent;
    [SerializeField] protected ScrollRect ScrollRect;
    [SerializeField] protected ARecyclerItem ItemPrefab;

    private readonly List<IRecyclerItem> _visibleItems = new();
    private readonly Stack<IRecyclerItem> _pooledItems = new();

    private float _viewAreaHeight;
    private float _itemHeight;
    private float _listHeight;

    private int _nextTopContentIndex;
    private int _nextBottomContentIndex;

    private LayoutElement _topPadding;
    private LayoutElement _bottomPadding;

    RectTransform IRecyclerListPresenter.ContentParent => ContentParent;
    RectTransform IRecyclerListPresenter.PoolParent => PoolParent;

    private bool _initialSetupDone = false;

    protected virtual void OnDestroy()
    {
        if (!ScrollRect)
            return;

        ScrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
    }

    /// <summary>
    /// Initializes and updates configuration of the list.
    /// Call at start or with data/layout changes.
    /// </summary>
    /// <param name="scrollToStart"> Should be current scroll state be reset? </param>
    protected void SetupRecyclerList(bool scrollToStart)
    {
        if (!_initialSetupDone)
        {
            CreatePaddingElements();
            ScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            _initialSetupDone = true;
        }

        // Prefab is not initialized. Look up height by finding the transform comp
        _itemHeight = ItemPrefab.GetComponent<RectTransform>().rect.height;

        // Presented content or panel layout may have changed
        _listHeight = GetDataCount() * _itemHeight;
        _viewAreaHeight = ScrollRect.GetComponent<RectTransform>().rect.height;

        // Set scroll area back to top
        if (scrollToStart)
            ScrollRect.verticalNormalizedPosition = 1f;
        
    }

    protected void ClearVisibleItems()
    {
        while (_visibleItems.Count > 0)
        {
            var removable = _visibleItems[0];
            _visibleItems.RemoveAt(0);
            _pooledItems.Push(removable);
            removable.Unbind();
        }
        SetupRecyclerList(true);
    }

    /// <summary> Go through the current list of items and add/remove pooling items from top/bottom as needed. </summary>
    private void OnScrollValueChanged(Vector2 scrollPosition)
    {
        // There is no content outside the 0 to 1 range
        if (scrollPosition.y < -0.01f || scrollPosition.y > 1.01f)
            return;

        // Remap scrollbar values to RectTransform space
        float windowTop = Mathf.Lerp(-(_listHeight - _viewAreaHeight), 0, scrollPosition.y);
        float windowBottom = windowTop - _viewAreaHeight;

        // Remove out of view items from top
        while (_visibleItems.Count > 0 && _visibleItems[0].BottomEdge > windowTop)
            RemoveFromTop();
            
        // Remove out of view items from bottom
        while (_visibleItems.Count > 0 && _visibleItems[^1].TopEdge < windowBottom)
            RemoveFromBottom();

        // When no items are present, the size of the padding elements needs an update
        if (_visibleItems.Count == 0)
            RescalePadding(windowTop);

        while (IsTopEdgeMissingContent(windowTop))
            AddToTop();

        while (IsBottomEdgeMissingContent(windowBottom))
            AddToBottom();
    }

    protected void ForceScrollReset() =>
        OnScrollValueChanged(Vector2.up);

    private void AddToBottom()
    {
        var item = GetItem();
        _visibleItems.Add(item);

        // Padding size is reduced to make room for a new item
        _bottomPadding.preferredHeight -= _itemHeight;

        // Item is inserted before bottom padding element
        item.Bind(GetDataAt(_nextBottomContentIndex), GetHighlightAt(_nextBottomContentIndex));

        item.Transform.SetAsLastSibling();
        _bottomPadding.transform.SetAsLastSibling();

        _nextBottomContentIndex++;

        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentParent);
    }

    private void AddToTop()
    {
        var item = GetItem();
        _visibleItems.Insert(0, item);

        // Padding size is reduced to make room for a new item
        _topPadding.preferredHeight -= _itemHeight;
        _topPadding.preferredHeight = Mathf.Max(_topPadding.preferredHeight, 0);

        // Item is inserted after top padding element
        item.Bind(GetDataAt(_nextTopContentIndex), GetHighlightAt(_nextTopContentIndex));

        item.Transform.SetAsFirstSibling();
        _topPadding.transform.SetAsFirstSibling();
        _nextTopContentIndex--;

        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentParent);
    }

    private void RemoveFromBottom()
    {
        // Pool the item locally
        var item = _visibleItems[^1];
        _visibleItems.RemoveAt(_visibleItems.Count - 1);
        _pooledItems.Push(item);
        _nextBottomContentIndex--;

        // Unbind item from parent and hide it.
        item.Unbind();

        // Change padding
        _bottomPadding.preferredHeight += _itemHeight;
        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentParent);
    }

    private void RemoveFromTop()
    {
        // Pool the item locally
        var item = _visibleItems[0];
        _visibleItems.RemoveAt(0);
        _pooledItems.Push(item);
        _nextTopContentIndex++;

        // Unbind item from parent and hide it.
        item.Unbind();

        // Change the padding
        _topPadding.preferredHeight += _itemHeight;
        _topPadding.transform.SetAsFirstSibling();
        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentParent);
    }

    private IRecyclerItem GetItem()
    {
        if (_pooledItems.Count == 0)
            CreateItem();

        // Take a pooled item and bind it to the top of the list
        var item = _pooledItems.Pop();
        return item;
    }

    /// <summary> Instantiates an item and puts it into the pool of items to use. </summary>
    private void CreateItem()
    {
        var item = Instantiate(ItemPrefab, ContentParent);
        item.Initialize(this);
        _pooledItems.Push(item);
    }

    private void CreatePaddingElements()
    {
        // Create an empty layout padding at the beginning of the list with preferredHeight 0.
        var topPad = new GameObject("TopPadding", typeof(RectTransform), typeof(LayoutElement));
        _topPadding = topPad.GetComponent<LayoutElement>();
        _topPadding.preferredHeight = 0;
        topPad.transform.SetParent(ContentParent);

        // Add padding to the bottom. Enables bottom items to scroll onto the screen, plus acts as the overall size container
        var botPad = new GameObject("BottomPadding", typeof(RectTransform), typeof(LayoutElement));
        _bottomPadding = botPad.GetComponent<LayoutElement>();
        _bottomPadding.preferredHeight = 0;
        botPad.transform.SetParent(ContentParent);
        botPad.transform.SetAsLastSibling();
    }

    bool IsBottomEdgeMissingContent(float edgeOfWindow)
    {
        // Is there any content at all to be shown
        if (GetDataCount() == 0)
            return false;

        // Is there any content left to display
        if (_nextBottomContentIndex >= GetDataCount())
            return false;

        // No items are shown yet
        if (_visibleItems.Count == 0)
            return true;

        // Bottom is missing content when bottom edge of bottom item is visible
        return _visibleItems[^1].BottomEdge > edgeOfWindow;
    }

    bool IsTopEdgeMissingContent(float edgeOfWindow)
    {
        // Is there any content at all to be shown
        if (GetDataCount() == 0)
            return false;

        // Is there any content left to display
        if (_nextTopContentIndex < 0)
            return false;

        // No items are shown yet
        if (_visibleItems.Count == 0)
            return true;

        //Debug.Log($"{_visibleItems[0].TopEdge} < {edgeOfWindow} ? {_visibleItems[0].TopEdge < edgeOfWindow}; Padding value: {_topPadding.preferredHeight}");
        return _visibleItems[0].TopEdge < edgeOfWindow;
    }

    void RescalePadding(float visibleAreaTopEdge)
    {
        // Padding scaling can only take place where there is no visible content
        // Otherwise it gets shifted out of position
        if (_visibleItems.Count != 0)
            return;

        var invertedPosition = -visibleAreaTopEdge;
        int numberOfHiddenItems = Mathf.FloorToInt(invertedPosition / _itemHeight);
        numberOfHiddenItems = Mathf.Max(numberOfHiddenItems, 0);

        // Top padding should be up to the edge of visible area
        _topPadding.preferredHeight = numberOfHiddenItems * _itemHeight;
        _bottomPadding.preferredHeight = _listHeight - _topPadding.preferredHeight;

        // Set the index for next content that should be bound
        _nextTopContentIndex = numberOfHiddenItems;
        _nextBottomContentIndex = numberOfHiddenItems + 1;

        //LayoutRebuilder.ForceRebuildLayoutImmediate(ContentParent);
    }

    public abstract void SetSelection(object selected);
    protected abstract object GetDataAt(int i);
    protected abstract (int, int) GetHighlightAt(int i);
    protected abstract int GetDataCount();

}
