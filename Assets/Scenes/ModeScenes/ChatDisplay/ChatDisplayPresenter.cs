using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using UnityEngine.UI;
using Void.YoutubeAPI;
using System;

public class ChatDisplayPresenter : AModePresenter
{
    [SerializeField] protected Canvas BaseCanvas;

    [Header("Chat Display")]
    [SerializeField] protected ScrollRect ScrollRect;
    [SerializeField] protected ChatItem ContentPrefab;
    [SerializeField] protected Transform ChatContent;
    [SerializeField] protected Transform PoolContent;

    private YoutubeAPITimer _apiTimer;
    private List<AListItem> _activePool = new();
    private Stack<AListItem> _objectPool = new();
    private int _currentActives = 0;

    private bool _open = false;
    private IEnumerator _currentDisplay;
    private HashSet<IEnumerator> _disposables = new();

    private void Awake()
    {
        BaseCanvas.worldCamera = Camera.main;
        ScreenResizeListener.OnResize += OnScreenResize;

        BaseCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        _apiTimer = FindObjectOfType<YoutubeAPITimer>();
        NotifyClassReady(this);
    }

    public override string GetName()
    {
        return "Chat";
    }

    public override string GetDescription()
    {
        return "Show the regular chat with no unique modifications.";
    }

    public override void Open()
    {
        BaseCanvas.gameObject.SetActive(true);

        //TODO:  Wait time increases to 0.05 seconds, or 3 skipped frames with 100 elements
        // Pretty much requires visual object pooling to preload chat here

        ScrollRect.verticalNormalizedPosition = 0f;

        _open = true;
        ChatMessageListener.ChatMessages += OnNewMessages;

        _apiTimer.StartTimer();
    }

    public override void Close()
    {
        _open = false;
        _currentActives = 0;
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

    /// <summary> Display any newly received messages </summary>
    /// <param name="newMessages"> Latest received messages from newest to oldest. </param>
    private void OnNewMessages(List<YoutubeChatMessage> newMessages)
    {
        // async does not have WaitForEndOfFrame, so using Coroutine for smoother scrollbar anchoring.
        _currentDisplay = DisplayMessages(newMessages);
        if (_currentDisplay != null)
            StartCoroutine(_currentDisplay);
    }

    private IEnumerator DisplayMessages(List<YoutubeChatMessage> newMessages)
    {
        IEnumerator thisCoroutine = _currentDisplay;
        float totalTimeWaitedSeconds = 0;
        bool overflowRisk = false;

        for (int i = newMessages.Count - 1; i >= 0; i--)
        {
            // Do (roughly) accurate waiting for messages, but only if the queue is not overflowing from waiting
            if (Settings.RealTime && !overflowRisk && i < newMessages.Count - 1)
            {
                float waitTime = (float)newMessages[i].Timestamp.Subtract(newMessages[i + 1].Timestamp).TotalSeconds;

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
            if (_currentActives < ChatMessageListener.ListMaxSize)
            {
                assignable = GetPoolItem();
                _currentActives++;
            }

            // Otherwise replace oldest data
            else
            {
                assignable = _activePool[0];
                _activePool.RemoveAt(0);
            }

            _activePool.Add(assignable);
            assignable.transform.SetAsLastSibling();
            assignable.Bind(newMessages[i], ChatContent);

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

    private void OnScreenResize(Vector2 anchorWorldMin, Vector2 anchorWorldMax)
    {
        if (!_open)
            return;

        foreach (ChatItem item in _activePool)
            item.UpdateFit();
    }
}
