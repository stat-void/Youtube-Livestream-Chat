using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Void.YoutubeAPI;
using Void.YoutubeAPI.LiveStreamChat.Messages;

public class PollManager : MonoBehaviour
{
    [SerializeField] protected PollItem PollItemPrefab;
    [SerializeField] protected Transform PollContent;
    [SerializeField] protected Transform PoolContent;

    [SerializeField] protected Toggle FirstQuerySkip;
    [SerializeField] protected Toggle CaseSensitive;
    [SerializeField] protected Toggle SkipPunctuations;
    [SerializeField] protected Toggle SkipWhitespace;
    [SerializeField] protected TMP_InputField DecayField;
    [SerializeField] protected TMP_Text UniqueMessageText;
    [SerializeField] protected TMP_Text UniqueMessageCount;


    // Pooling and display related to vote Answers
    private readonly List<PollItem> _pollItems = new();
    private readonly Stack<PollItem> _objectPool = new();
    private const int _maxPollers = 10;

    // Various things used during polling
    private readonly Dictionary<string, string> _userMessagePairs = new();  // For checking what poll to "-" when changing
    private readonly Dictionary<string, int> _messageCountPairs = new();    // To fetch the most polled messages
    private int _pollSum = 0;

    private readonly Queue<List<(string, string)>> _decay = new();   // Tuples of ID and MSG
    private int _defaultDecay = 20;
    private int _currentDecay;

    private bool _polling = false;
    private bool _firstQuerySkipped = false;
    private YoutubeAPITimer _apiTimer;

    private IEnumerator _animatedPolling;

    /*
    -------------------------
    -------------------------
    Manager init (open/close)
    -------------------------
    -------------------------
    */

    private void Start()
    {
        _apiTimer = FindObjectOfType<YoutubeDataAPI>().APITimer;
    }

    public void Open()
    {
        for (int i = 0; i < _maxPollers; i++)
            AddPoller();

        UniqueMessageText.gameObject.SetActive(false);
        UniqueMessageCount.gameObject.SetActive(false);
    }

    public void Close()
    {
        // Stop any active voting and in extent, reset booleans + dict
        if (_polling)
            StopVote();

        if (_animatedPolling != null)
        {
            StopCoroutine(_animatedPolling);
            _animatedPolling = null;
        }

        // Remove all currently visible pollings.
        while (_pollItems.Count > 0)
            RemovePoller();
    }


    /*
    ------------------------
    ------------------------
    Poller Addition
    ------------------------
    ------------------------
    */

    /// <summary> Add another poller, up until max. </summary>
    private void AddPoller()
    {
        if (_pollItems.Count >= _maxPollers)
            return;

        PollItem item = GetPollItem();
        _pollItems.Add(item);
        item.Initialize(PollContent);
        item.gameObject.SetActive(true);
    }

    /// <summary> Remove a polling item. </summary>
    private void RemovePoller()
    {
        if (_pollItems.Count <= 0)
            return;

        PollItem item = _pollItems[_pollItems.Count - 1];

        _objectPool.Push(item);
        _pollItems.RemoveAt(_pollItems.Count - 1);
        item.transform.SetParent(PoolContent);
        item.gameObject.SetActive(false);

    }

    /// <summary> Remove all current polling results. </summary>
    public void ResetPolls()
    {
        // Make sure to stop any running animations
        if (_animatedPolling != null)
        {
            StopCoroutine(_animatedPolling);
            _animatedPolling = null;
        }

        // Remove all visualizations
        foreach (PollItem item in _pollItems)
            item.ResetData();

        _pollSum = 0;
        UniqueMessageCount.text = "0";
        _userMessagePairs.Clear();
        _messageCountPairs.Clear();
        _decay.Clear();
    }

    private PollItem GetPollItem()
    {
        if (_objectPool.Count > 0)
            return _objectPool.Pop();

        // Create new pooling item
        var item = Instantiate(PollItemPrefab, PollContent);
        return item;
    }


    /*
    ------------------------
    ------------------------
    Active Voting
    ------------------------
    ------------------------
    */

    public void StartVote()
    {
        if (_animatedPolling != null)
        {
            StopCoroutine(_animatedPolling);
            _animatedPolling = null;
        }

        foreach (PollItem item in _pollItems)
            item.ResetData();

        SetInteractiveState(false);

        _polling = true;
        _firstQuerySkipped = !FirstQuerySkip.isOn;
        _pollSum = 0;

        UniqueMessageText.gameObject.SetActive(true);
        UniqueMessageCount.gameObject.SetActive(true);
        UniqueMessageCount.text = "0";

        if (DecayField.text == "0" || string.IsNullOrWhiteSpace(DecayField.text))
            DecayField.text = "";

        _currentDecay = !string.IsNullOrEmpty(DecayField.text) ?
            int.Parse(DecayField.text) : _defaultDecay;

        ChatMessageListener.ChatMessages += OnNewMessages;
    }

    public void StopVote()
    {
        SetInteractiveState(true);

        _userMessagePairs.Clear();
        _messageCountPairs.Clear();
        _decay.Clear();
        _polling = false;
        _firstQuerySkipped = false;
        ChatMessageListener.ChatMessages -= OnNewMessages;
    }

    private void SetInteractiveState(bool state)
    {
        FirstQuerySkip.interactable = state;
        CaseSensitive.interactable = state;
        SkipPunctuations.interactable = state;
        SkipWhitespace.interactable = state;
        DecayField.interactable = state;
    }

    private void OnNewMessages(List<YoutubeChatMessage> messages)
    {
        if (!_firstQuerySkipped)
        {
            _firstQuerySkipped = true;
            return;
        }

        int oldSum = _pollSum;

        /*
        ---
        Part 1 - Iterating all new messages and updating dictionary values
        ---
        */
        List<(string, string)> decayList = new();

        foreach (YoutubeChatMessage message in messages)
        {
            if (message.Message == null || message.ChannelID == null)
                continue;

            string id = message.ChannelID;
            string poll = message.Message.Trim();

            // If any special toggles on, trim the message
            if (!CaseSensitive.isOn || SkipPunctuations.isOn || SkipWhitespace.isOn)
            {
                StringBuilder sb = new();

                foreach (char c in poll)
                {
                    if (SkipPunctuations.isOn && char.IsPunctuation(c))
                        continue;

                    if (SkipWhitespace.isOn && char.IsWhiteSpace(c))
                        continue;

                    if (CaseSensitive.isOn)
                        sb.Append(c);

                    else
                        sb.Append(char.ToLower(c));
                }

                poll = sb.ToString();
            }

            // If the resulting fetched/modified message is empty, skip it.
            if (string.IsNullOrWhiteSpace(poll))
                continue;


            // Check Condition 1 - A brand new poll from a user
            if (!_userMessagePairs.ContainsKey(id))
            {
                if (!_messageCountPairs.ContainsKey(poll))
                    _messageCountPairs[poll] = 1;
                else
                    _messageCountPairs[poll]++;

                _userMessagePairs[id] = poll;
                _pollSum++;
            }

            // Check Condition 2 - The user has done a poll before.
            else
            {
                _messageCountPairs[_userMessagePairs[id]]--;

                if (!_messageCountPairs.ContainsKey(poll))
                    _messageCountPairs[poll] = 1;
                else
                    _messageCountPairs[poll]++;

                _userMessagePairs[id] = poll;
            }

            decayList.Add((id, poll));
        }

        /*
        ---
        Part 2 - Decay: removing old messages if users have not said something new.
        ---
        */
        _decay.Enqueue(decayList);

        if (_decay.Count > _currentDecay)
        {
            List<(string, string)> removeDecay = _decay.Dequeue();

            foreach((string, string) idMessagePair in removeDecay)
            {
                if (_userMessagePairs.ContainsKey(idMessagePair.Item1) &&
                    _userMessagePairs[idMessagePair.Item1] == idMessagePair.Item2)
                {
                    _userMessagePairs.Remove(idMessagePair.Item1);

                    if (_messageCountPairs[idMessagePair.Item2] > 1)
                        _messageCountPairs[idMessagePair.Item2]--;
                    else
                        _messageCountPairs.Remove(idMessagePair.Item2);

                    _pollSum--;
                }
            }
        }

        /*
        ---
        Part 3 - Visualization of results (Linq)
        ---
        */

        var sortedItems = (from entry in _messageCountPairs
                           orderby entry.Value descending select entry)
                           .Take(_maxPollers).ToArray();

        string[] sortedMessages = new string[_maxPollers];
        int[] sortedCounts = new int[_maxPollers];
        for (int i = 0; i < _maxPollers; i++)
        {
            if (i < sortedItems.Length)
            {
                sortedMessages[i] = sortedItems[i].Key;
                sortedCounts[i] = sortedItems[i].Value;
            }
            else
            {
                sortedMessages[i] = "";
                sortedCounts[i] = 0;
            }
        }

        float[] percentages = GetPercentages(sortedCounts, _pollSum);
        int[] approxPercentages = ApproximatePercentages(percentages);


        if (!Settings.Animations)
        {
            for (int i = 0; i < _pollItems.Count; i++)
            {
                _pollItems[i].UpdatePoll(sortedCounts[i], percentages[i], approxPercentages[i]);
                _pollItems[i].UpdateMessage(sortedMessages[i]);
            }
            UniqueMessageCount.text = $"{_pollSum}";
        }  

        else
        {
            int[] oldPollCounts = new int[_maxPollers];
            float[] oldPercentages = new float[_maxPollers];
            int[] oldApproxPercentages = new int[_maxPollers];

            for (int i = 0; i < _pollItems.Count; i++)
            {
                oldPollCounts[i] = _pollItems[i].GetCount();
                oldPercentages[i] = _pollItems[i].GetPercent();
                oldApproxPercentages[i] = _pollItems[i].GetApproxPercent();

                // Instantly change messages because animating this alongside everything else is too much of a hassle.
                _pollItems[i].UpdateMessage(sortedMessages[i]);
            }
            UniqueMessageCount.text = $"{_pollSum}";

            _animatedPolling = AnimatePoll(_pollItems, oldSum, oldPollCounts, sortedCounts, oldPercentages, percentages, oldApproxPercentages, approxPercentages);
            StartCoroutine(_animatedPolling);
        }
    }

    /// <summary> Convert a list of integers into percentages. </summary>
    private float[] GetPercentages(int[] values, int sum)
    {
        float[] percentages = new float[values.Length];

        if (sum == 0)
            for (int i = 0; i < values.Length; i++)
                percentages[i] = 0;
        else
            for (int i = 0; i < values.Length; i++)
                percentages[i] = (float)values[i] / sum;

        return percentages;
    }

    /// <summary> Approximate a list of percentages to integers. </summary>
    private int[] ApproximatePercentages(float[] values)
    {
        int[] percentages = new int[values.Length];

        float cumulPercent = 0;
        int lastVal = 0;

        for (int i = 0; i < values.Length; i++)
        {
            // If 0, add a 0 and prevent percent difference calculation from messing up
            if (values[i] == 0)
            {
                percentages[i] = 0;
                continue;
            }

            cumulPercent += values[i] * 100f;

            int cumulRound = Mathf.RoundToInt(cumulPercent);
            int percent = cumulRound - lastVal;
            lastVal = cumulRound;

            percentages[i] = percent;
        }

        return percentages;
    }

    private IEnumerator AnimatePoll(
        List<PollItem> items,
        int oldSum,
        int[] oldVotes, int[] newVotes,
        float[] oldPercents, float[] newPercents,
        int[] oldApproxPercents, int[] newApproxPercents)
    {
        float duration = _apiTimer.APIRequestInterval / 2f;
        float time = 0;

        while (time < duration)
        {
            if (_animatedPolling == null)
                yield break;

            time += Time.unscaledDeltaTime;

            float t = time / duration;
            t = t * t * (3f - 2f * t);

            UniqueMessageCount.text = $"{Mathf.RoundToInt(Mathf.Lerp((float)oldSum, (float)_pollSum, t))}";

            // Give all voting items lerped values
            for (int i = 0; i < items.Count; i++)
            {
                PollItem item = items[i];
                int lerpedVoteCount = Mathf.RoundToInt(Mathf.Lerp((float)oldVotes[i], (float)newVotes[i], t));
                float lerpedPercent = Mathf.Lerp(oldPercents[i], newPercents[i], t);
                int lerpedApproxPercent = Mathf.RoundToInt(Mathf.Lerp((float)oldApproxPercents[i], (float)newApproxPercents[i], t));

                item.UpdatePoll(lerpedVoteCount, lerpedPercent, lerpedApproxPercent);
            }

            yield return null;

        }

        if (_animatedPolling == null)
            yield break;

        for (int i = 0; i < _pollItems.Count; i++)
            _pollItems[i].UpdatePoll(newVotes[i], newPercents[i], newApproxPercents[i]);
        

        _animatedPolling = null;
        yield break;
    }
}
