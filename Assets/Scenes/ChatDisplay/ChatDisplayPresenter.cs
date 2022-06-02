using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using UnityEngine.UI;
using Void.YoutubeAPI;

public class ChatDisplayPresenter : AContentPresenter
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

    private void Awake()
    {
        BaseCanvas.worldCamera = Camera.main;
        ScreenResizeListener.OnResize += OnScreenResize;

        BaseCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        // This isn't particularly great... but the multi-scene setup doesn't allow for a direct connection
        _apiTimer = FindObjectOfType<YoutubeAPITimer>();
    }

    public override void Open()
    {
        BaseCanvas.gameObject.SetActive(true);

        // Add all currently recorded elements from oldest to newest
        for (int i = ChatMessageListener.MessageList.Count-1; i >= 0; i--)
        {
            YoutubeChatMessage message = ChatMessageListener.MessageList[i];

            var item = GetPoolItem();
            item.Bind(message, ChatContent);

            _currentActives++;
        }

        ScrollRect.verticalNormalizedPosition = 0f;

        _open = true;
        ChatMessageListener.ChatMessages += OnNewMessages;
    }

    

    public override void Close()
    {
        _open = false;
        _currentActives = 0;
        ChatMessageListener.ChatMessages -= OnNewMessages;

        // Take every item currently active and unbind them into the bool, deactivating them.
        for (int i = _activePool.Count; i >= 0; i--)
        {
            var item = _activePool[i];
            item.Unbind(PoolContent);
        }

        BaseCanvas.gameObject.SetActive(false);
    }


    private void OnScreenResize(Vector2 anchorWorldMin, Vector2 anchorWorldMax)
    {
        if (!_open)
            return;

        foreach (ChatItem item in _activePool)
            item.UpdateFit();
    }

    /// <summary> Display any newly received messages </summary>
    /// <param name="newMessages"> Latest received messages from newest to oldest. </param>
    private void OnNewMessages(List<YoutubeChatMessage> newMessages)
    {
        // async does not have WaitForEndOfFrame, so using Coroutine for smoother scrollbar anchoring.
        StartCoroutine(DisplayMessages(newMessages));
    }

    private IEnumerator DisplayMessages(List<YoutubeChatMessage> newMessages)
    {
        float totalTimeWaitedSeconds = 0;

        for (int i = newMessages.Count - 1; i >= 0; i--)
        {
            // Do (roughly) accurate waiting for messages, but only if the queue is not overflowing from waiting
            if (ChatMessageListener.WaitMessages && i < newMessages.Count - 1)
            {
                float waitTime = (float)newMessages[i].TimeStamp.Subtract(newMessages[i + 1].TimeStamp).TotalSeconds;

                if (totalTimeWaitedSeconds + waitTime < _apiTimer.APIRequestDelay - 0.5f)
                {
                    totalTimeWaitedSeconds += waitTime;
                    yield return new WaitForSeconds(waitTime);
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

            if (ChatMessageListener.WaitMessages)
            {
                yield return new WaitForEndOfFrame();
                ScrollRect.verticalNormalizedPosition = 0f;
            }
        }

        yield return new WaitForEndOfFrame();
        ScrollRect.verticalNormalizedPosition = 0f;
    }

    private AListItem GetPoolItem()
    {
        if (_objectPool.Count > 0)
            return _objectPool.Pop();

        // Create new pooling item
        var item = Instantiate(ContentPrefab, ChatContent);
        return item;
    }
}