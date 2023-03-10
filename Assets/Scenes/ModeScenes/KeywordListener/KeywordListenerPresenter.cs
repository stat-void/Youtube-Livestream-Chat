using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using Void.YoutubeAPI;
using System.Drawing;

public class KeywordListenerPresenter : AModePresenter
{
    [SerializeField] protected Canvas BaseCanvas;

    [Header("Keyword Window")]
    [SerializeField] protected Button AddRemoveKeywordsButton;
    [SerializeField] protected Transform KeywordChangerBox;
    [SerializeField] protected KeywordChangeField KeywordField;

    [Header("Chat Display")]
    [SerializeField] protected ScrollRect ScrollRect;
    [SerializeField] protected ChatItem ContentPrefab;
    [SerializeField] protected Transform ChatContent;
    [SerializeField] protected Transform PoolContent;
    
    private YoutubeAPITimer _apiTimer;
    private List<AListItem> _activePool = new();
    private Stack<AListItem> _objectPool = new();
    private int _visibleChatMessages = 0;

    private IEnumerator _currentDisplay;
    private HashSet<IEnumerator> _disposables = new();

    private bool _userChangerOpen = false;

    private void Awake()
    {
        BaseCanvas.worldCamera = Camera.main;
        BaseCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        _apiTimer = FindObjectOfType<YoutubeLiveChatMessages>().APITimer;
        NotifyClassReady(this);
    }

    private void OnEnable()
    {
        AddRemoveKeywordsButton.onClick.AddListener(UpdateKeywordChangerBox);
    }

    private void OnDisable()
    {
        AddRemoveKeywordsButton.onClick.RemoveListener(UpdateKeywordChangerBox);
    }

    public override string GetName()
    {
        return "Keyword Listener";
    }

    public override string GetDescription()
    {
        return "Filter messages by listening to specific keywords.";
    }

    public override void Open()
    {
        BaseCanvas.gameObject.SetActive(true);
        ScrollRect.verticalNormalizedPosition = 0f;
        KeywordField.OpenRefresh();

        ScreenResizeListener.OnResize += OnScreenResize;
        ChatMessageListener.ChatMessages += OnNewMessages;


        _apiTimer.StartTimer();
    }

    public override void Close()
    {
        _visibleChatMessages = 0;
        KeywordField.CloseRefresh();

        ScreenResizeListener.OnResize -= OnScreenResize;
        ChatMessageListener.ChatMessages -= OnNewMessages;

        // Take every item currently active and unbind them into the pool, deactivating them.
        for (int i = _activePool.Count - 1; i >= 0; i--)
        {
            var item = _activePool[i];
            _objectPool.Push(item);
            item.Unbind(PoolContent);
        }

        if (_currentDisplay != null)
        {
            _disposables.Add(_currentDisplay);
            _currentDisplay = null;
        }

        _activePool.Clear();
        _apiTimer.PauseTimer();
        BaseCanvas.gameObject.SetActive(false);
    }


    /// <summary> Depending on _userChangerOpen, open or close the search field window. </summary>
    private async void UpdateKeywordChangerBox()
    {
        AddRemoveKeywordsButton.interactable = false;

        _userChangerOpen = !_userChangerOpen;

        Vector3 currentPos = KeywordChangerBox.localPosition;

        if (!Settings.Animations)
            KeywordChangerBox.localPosition =
                _userChangerOpen ? currentPos - new Vector3(320,0,0) : currentPos + new Vector3(320, 0, 0);

        else
            await Vector3Lerp(
                KeywordChangerBox,
                 currentPos,
                 _userChangerOpen ? currentPos - new Vector3(320, 0, 0) : currentPos + new Vector3(320, 0, 0),
                0.5f);

        AddRemoveKeywordsButton.interactable = true;
    }

    private async Task Vector3Lerp(Transform item, Vector3 from, Vector3 to, float duration)
    {
        float time = 0;


        while (time < duration)
        {
            await Task.Yield();
            time += Time.unscaledDeltaTime;

            // Smooth Step Lerp
            float t = time / duration;
            t = t * t * (3f - 2f * t);

            item.localPosition = Vector3.Lerp(from, to, t);
        }

        item.localPosition = to;
    }


    /*
    ---
    ---
    ChatDisplayPresenter related content (With some parts slightly modified)
    ---
    ---
    */

    /// <summary> Display any newly received messages </summary>
    /// <param name="newMessages"> Latest received messages from newest to oldest. </param>
    private void OnNewMessages(List<YoutubeChatMessage> newMessages)
    {
        _currentDisplay = DisplayMessages(newMessages);
        StartCoroutine(_currentDisplay);
    }

    private IEnumerator DisplayMessages(List<YoutubeChatMessage> newMessages)
    {
        IEnumerator thisCoroutine = _currentDisplay;
        float totalTimeWaitedSeconds = 0;
        bool overflowRisk = false;

        bool isExcludeMode = KeywordField.GetSwitchButton().GetCurrentChoice() == "Only Match";

        for (int i = newMessages.Count - 1; i >= 0; i--)
        {
            YoutubeChatMessage msg = newMessages[i];

            (bool isValid, MultiRange ranges) = KeywordField.IsKeywordValid(msg);

            if (isExcludeMode && !isValid)
                continue;

            // Get dictionary keys in reversed order
            SortedDictionary<int, int> data = ranges.GetIndexLengthPairs();
            foreach (int key in data.Keys)
            {
                int index = key;
                int length = data[key];
                int end = index + length;
                string highlight = $"<color=#{ColorSettings.HighlightMessageColor}>{msg.Message.Substring(index, length)}</color>";
                msg.Message = msg.Message[..index] + highlight + msg.Message[end..];
            }

            // Do (roughly) accurate waiting for messages, but only if the queue is not overflowing from waiting
            if (Settings.RealTime && !overflowRisk && i < newMessages.Count - 1)
            {
                float waitTime = (float)msg.Timestamp.Subtract(newMessages[i + 1].Timestamp).TotalSeconds;

                if (totalTimeWaitedSeconds + waitTime < _apiTimer.APIRequestInterval - 0.5f)
                {
                    totalTimeWaitedSeconds += waitTime;
                    yield return new WaitForSeconds(waitTime);

                    // Verify if this view was closed and stop if needed. Note that there will likely be null exceptions if this ever occurs in this view.
                    if (_disposables.Contains(thisCoroutine))
                    {
                        _disposables.Remove(thisCoroutine);
                        yield break;
                    }
                }
                else
                {
                    overflowRisk = true;
                }
            }

            AListItem assignable;

            // if not full, add a new pooling item
            if (_visibleChatMessages < ChatMessageListener.ListMaxSize)
            {
                assignable = GetPoolItem();
                _visibleChatMessages++;
            }

            // Otherwise replace oldest data
            else
            {
                assignable = _activePool[0];
                _activePool.RemoveAt(0);
            }

            _activePool.Add(assignable);
            assignable.transform.SetAsLastSibling();
            assignable.Bind(msg, ChatContent);

            if (Settings.RealTime)
            {
                yield return new WaitForEndOfFrame();
                ScrollRect.verticalNormalizedPosition = 0f;
            }
        }

        yield return new WaitForEndOfFrame();
        ScrollRect.verticalNormalizedPosition = 0f;

        _currentDisplay = null;
    }

    private AListItem GetPoolItem()
    {
        if (_objectPool.Count > 0)
            return _objectPool.Pop();

        // Create new pooling item
        var item = Instantiate(ContentPrefab, ChatContent);
        return item;
    }

    private void OnScreenResize()
    {
        foreach (ChatItem item in _activePool)
            item.UpdateFit();
    }
}
