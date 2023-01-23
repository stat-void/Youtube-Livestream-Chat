using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using Void.YoutubeAPI;


/*
Possible future implementation idea.

What if I add additional functionality to extract users that gave specific vote results,
and be able to hold a vote where only they can vote?


First step - Prevent clearing all data when voting is stopped.
Instead, that should be done whenever started.

Second step - Have a method and an additional... Set? A way to store users.
This list should also be separately cleared in some manner?

*/

public class VoteManager : MonoBehaviour
{
    [SerializeField] protected VoteItem VoteItemPrefab;
    [SerializeField] protected Transform VoteContent;
    [SerializeField] protected Transform PoolContent;

    [SerializeField] protected Button AddButton;
    [SerializeField] protected Button RemoveButton;
    [SerializeField] protected TMP_InputField VoteAnswerCountInputField;
    [SerializeField] protected TMP_InputField QuestionInputField;
    [SerializeField] protected Toggle MultiVotePermission;
    [SerializeField] protected Toggle FirstQuerySkip;

    // Pooling and display related to vote Answers
    private List<VoteItem> _voteItems = new();
    private Stack<VoteItem> _objectPool = new();
    private const int _minAnswers = 2;
    private const int _maxAnswers = 10;

    // Various things used during voting
    private readonly Dictionary<string, string> _userVotePairs = new();
    private readonly Dictionary<string, int> _promptIndexPairs = new();
    private List<int> _voteCounts = new();
    private bool _voting = false;
    private bool _firstQuerySkipped = false;
    private YoutubeAPITimer _apiTimer;

    private IEnumerator _animatedVoting;

    /*
    -------------------------
    -------------------------
    Manager init (open/close)
    -------------------------
    -------------------------
    */

    private void Start()
    {
        _apiTimer = FindObjectOfType<YoutubeLiveChatMessages>().APITimer;
    }

    public void Open()
    {
        // Add the minimum amount of Answers.
        for (int i = 0; i < _minAnswers; i++)
        {
            VoteItem item = GetPoolItem();

            item.gameObject.SetActive(true);
            item.Initialize(VoteContent, i);
            _voteItems.Add(item);
        }

        // Add all listeners
        AddButton.onClick.AddListener(OnAddClicked);
        RemoveButton.onClick.AddListener(OnRemoveClicked);
        VoteAnswerCountInputField.onEndEdit.AddListener(OnAnswerCountChanged);

    }

    public void Close()
    {
        // Stop any active voting and in extent, reset booleans + dict
        if (_voting)
            StopVote();

        if (_animatedVoting != null)
        {
            StopCoroutine(_animatedVoting);
            _animatedVoting = null;
        }

        // Remove all currently visible answers.
        for (int i = _voteItems.Count - 1; i >= 0; i--)
        {
            VoteItem item = _voteItems[i];

            _objectPool.Push(item);
            _voteItems.RemoveAt(i);
            item.transform.SetParent(PoolContent);
            item.gameObject.SetActive(false);
        }

        // Reset input fields
        VoteAnswerCountInputField.text = "";
        QuestionInputField.text = "";


        // Remove all listeners
        AddButton.onClick.RemoveListener(OnAddClicked);
        RemoveButton.onClick.RemoveListener(OnRemoveClicked);
        VoteAnswerCountInputField.onEndEdit.RemoveListener(OnAnswerCountChanged);
    }


    /*
    ------------------------
    ------------------------
    Answer Count Changes
    ------------------------
    ------------------------
    */


    private void OnAddClicked() =>
        AddAnswer();

    private void OnRemoveClicked() =>
        RemoveAnswer();

    private void OnAnswerCountChanged(string value)
    {
        if (value == "")
            return;

        // Due to constraints, the result should always be an integer
        int numVal = int.Parse(value);

        // First off, clamp the actual text value
        if (numVal < _minAnswers)
            numVal = _minAnswers;

        else if (numVal > _maxAnswers)
            numVal = _maxAnswers;

        // Now, check if and how many Answers need to be added or removed
        while (_voteItems.Count < numVal)
            AddAnswer();

        while (_voteItems.Count > numVal)
            RemoveAnswer();
    }


    /// <summary> Add another fillable answer. Don't do anything if max answers already available. </summary>
    private void AddAnswer()
    {
        if (_voteItems.Count >= _maxAnswers)
        {
            VoteAnswerCountInputField.text = $"{_maxAnswers}";
            return;
        } 

        VoteItem item = GetPoolItem();
        _voteItems.Add(item);
        item.Initialize(VoteContent, _voteItems.Count - 1);
        item.gameObject.SetActive(true);

        VoteAnswerCountInputField.text = $"{_voteItems.Count}";
    }

    /// <summary> Remove a fillable answer. Don't do anything if there are only min answers left. </summary>
    private void RemoveAnswer()
    {
        if (_voteItems.Count <= _minAnswers)
        {
            VoteAnswerCountInputField.text = $"{_minAnswers}";
            return;
        }

        VoteItem item = _voteItems[_voteItems.Count - 1];

        _objectPool.Push(item);
        _voteItems.RemoveAt(_voteItems.Count - 1);
        item.transform.SetParent(PoolContent);
        item.gameObject.SetActive(false);

        VoteAnswerCountInputField.text = $"{_voteItems.Count}";
    }

    /// <summary> Should be triggered whenever the Reset button is pressed. Resets Question and Answers to initial state. </summary>
    public void ResetQuestionAndAnswers()
    {
        while (_voteItems.Count > _minAnswers)
            RemoveAnswer();

        foreach (VoteItem item in _voteItems)
            item.ResetData();

        QuestionInputField.text = "";
        VoteAnswerCountInputField.text = "";
    }

    private VoteItem GetPoolItem()
    {
        if (_objectPool.Count > 0)
            return _objectPool.Pop();

        // Create new pooling item
        var item = Instantiate(VoteItemPrefab, VoteContent);
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
        if (VoteAnswerCountInputField.text == "")
            VoteAnswerCountInputField.text = $"{_minAnswers}";

        if (QuestionInputField.text == "")
            QuestionInputField.text = "Question?";

        if (_animatedVoting != null)
        {
            StopCoroutine(_animatedVoting);
            _animatedVoting = null;
        }

        SetInteractiveState(false);

        // Set up all Answers and their prompts for listening
        for (int i = 0; i < _voteItems.Count; i++)
        {
            VoteItem item = _voteItems[i];
            item.StartVote();

            // Special check to prevent issues with duplicate prompts
            if (_promptIndexPairs.ContainsKey(item.GetPrompt().ToLower()))
                item.MakePromptUnique();

            _promptIndexPairs[item.GetPrompt().ToLower()] = i;

            _voteCounts.Add(0);
        }


        _voting = true;
        _firstQuerySkipped = !FirstQuerySkip.isOn;
        ChatMessageListener.ChatMessages += OnNewMessages;
    }

    public void StopVote()
    {
        SetInteractiveState(true);

        for (int i = 0; i < _voteItems.Count; i++)
        {
            VoteItem item = _voteItems[i];
            item.StopVote();
        }

        _userVotePairs.Clear();
        _promptIndexPairs.Clear();
        _voteCounts.Clear();
        _voting = false;
        _firstQuerySkipped = false;
        ChatMessageListener.ChatMessages -= OnNewMessages;
    }

    private void SetInteractiveState(bool state)
    {
        AddButton.interactable = state;
        RemoveButton.interactable = state;
        FirstQuerySkip.interactable = state;
        MultiVotePermission.interactable = state;
        QuestionInputField.interactable = state;
        VoteAnswerCountInputField.interactable = state;
        
    }

    private void OnNewMessages(List<YoutubeChatMessage> messages)
    {
        if (!_firstQuerySkipped)
        {
            _firstQuerySkipped = true;
            return;
        }

        List<int> oldVoteCounts = new(_voteCounts);
        List<int> newVoteCounts = new(_voteCounts);

        foreach (YoutubeChatMessage message in messages)
        {
            if (message.Message == null)
                continue;

            string id = message.ChannelID;
            string prompt = message.Message.ToLower();

            // Verify that the message is a matching Answer prompt.
            if (_promptIndexPairs.ContainsKey(prompt))
            {
                // Condition 1 - A brand new vote from a user
                if (!_userVotePairs.ContainsKey(id))
                {
                    newVoteCounts[_promptIndexPairs[prompt]] += 1;
                    _userVotePairs[id] = prompt;
                }

                // Condition 2 - An existing user does a revote and revoting is turned on.
                else if (_userVotePairs.ContainsKey(id) && MultiVotePermission.isOn)
                {
                    newVoteCounts[_promptIndexPairs[_userVotePairs[id]]] -= 1;
                    newVoteCounts[_promptIndexPairs[prompt]] += 1;
                    _userVotePairs[message.ChannelID] = prompt;
                }
            }
        }

        // Afterwork - with the new count, depending on animation status, change visible counts
        _voteCounts = new(newVoteCounts);

        List<float> percentages = GetPercentages(_voteCounts);
        List<int> approxPercentages = ApproximatePercentages(percentages);


        if (!Settings.Animations)
            for (int i = 0; i < _voteItems.Count; i++)
                _voteItems[i].UpdateVote(_voteCounts[i], percentages[i], approxPercentages[i]);
            

        else
        {
            List<float> oldPercentages = new();
            List<int> oldApproxPercentages = new();

            foreach(VoteItem item in _voteItems)
            {
                oldPercentages.Add(item.GetPercent());
                oldApproxPercentages.Add(item.GetApproxPercent());
            }

            _animatedVoting = AnimateVote(_voteItems, oldVoteCounts, newVoteCounts, oldPercentages, percentages, oldApproxPercentages, approxPercentages);
            StartCoroutine(_animatedVoting);
        }
    }

    /// <summary> Convert a list of integers into percentages, assuming their sum is 1 </summary>
    private List<float> GetPercentages(List<int> values)
    {
        List<float> percentages = new();

        int sum = 0;
        foreach (int num in values)
        {
            sum += num;
            percentages.Add(0);
        }

        // Prevent division by 0
        if (sum == 0)
            return percentages;

        // Now calculate roughly accurate values
        for (int i = 0; i < values.Count; i++)
            percentages[i] = (float) values[i] / sum;
        
        return percentages;
    }

    /// <summary> Approximate a list of percentages (whose sum is "1" or 0) to integers (sum to 100 or 0) </summary>
    private List<int> ApproximatePercentages(List<float> values)
    {
        List<int> percentages = new();

        float cumulPercent = 0;
        int lastVal = 0;

        for (int i = 0; i < values.Count; i++)
        {
            // If 0, add a 0 and prevent percent difference calculation from messing up
            if (values[i] == 0)
            {
                percentages.Add(0);
                continue;
            }

            cumulPercent += values[i] * 100f;

            int cumulRound = Mathf.RoundToInt(cumulPercent);
            int percent = cumulRound - lastVal;
            lastVal = cumulRound;

            percentages.Add(percent);
        }

        return percentages;
    }

    private IEnumerator AnimateVote(List<VoteItem> items,
        List<int>   oldVotes,           List<int>   newVotes,
        List<float> oldPercents,        List<float> newPercents,
        List<int>   oldApproxPercents,  List<int>   newApproxPercents)
    {
        float duration = _apiTimer.APIRequestInterval / 2f;
        float time = 0;

        while (time < duration)
        {
            if (_animatedVoting == null)
                yield break;

            time += Time.unscaledDeltaTime;

            float t = time / duration;
            t = t * t * (3f - 2f * t);

            // Give all voting items lerped values
            for(int i = 0; i < items.Count; i++)
            {
                VoteItem item = items[i];
                int lerpedVoteCount = Mathf.RoundToInt(Mathf.Lerp((float)oldVotes[i], (float)newVotes[i], t));
                float lerpedPercent = Mathf.Lerp(oldPercents[i], newPercents[i], t);
                int lerpedApproxPercent = Mathf.RoundToInt(Mathf.Lerp((float)oldApproxPercents[i], (float)newApproxPercents[i], t));

                item.UpdateVote(lerpedVoteCount, lerpedPercent, lerpedApproxPercent);
            }

            yield return null;

        }

        if (_animatedVoting == null)
            yield break;

        for (int i = 0; i < _voteItems.Count; i++)
            _voteItems[i].UpdateVote(newVotes[i], newPercents[i], newApproxPercents[i]);

        _animatedVoting = null;
        yield break;
    }

}
