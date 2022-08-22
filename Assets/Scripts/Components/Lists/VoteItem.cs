using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VoteItem : MonoBehaviour
{
    [SerializeField] protected TMP_InputField VotePrompt;
    [SerializeField] protected TMP_InputField VoteAnswer;
    [SerializeField] protected TMP_Text VotePercentage;
    [SerializeField] protected TMP_Text VoteCount;
    [SerializeField] protected SlicedFilledImage VoteBar;

    private int _index = -1;
    private int _approxPercent = 0;


    public void Initialize(Transform parent, int index)
    {
        ResetData();
        _index = index;
        transform.SetParent(parent);
    }

    public void ResetData()
    {
        VotePrompt.text = "";
        VoteAnswer.text = "";
        VotePercentage.text = "";
        VoteCount.text = "";
        VoteBar.fillAmount = 0;
    }

    public void StartVote()
    {
        VoteBar.fillAmount = 0;
        VotePercentage.text = "0%";
        VoteCount.text = "0";
        _approxPercent = 0;

        if (string.IsNullOrWhiteSpace(VotePrompt.text))
            VotePrompt.text = $"{_index + 1}";

        if (string.IsNullOrWhiteSpace(VoteAnswer.text))
            VoteAnswer.text = $"Answer {_index + 1}";

        VotePrompt.interactable = false;
        VoteAnswer.interactable = false;
    }

    public void StopVote()
    {
        VotePrompt.interactable = true;
        VoteAnswer.interactable = true;
    }

    public void UpdateVote(int count, float percent, int approxPercent)
    {
        VoteCount.text = count.ToString();
        VoteBar.fillAmount = percent;
        VotePercentage.text = $"{approxPercent}%";
        _approxPercent = approxPercent;

    }

    public string GetPrompt()
    {
        return VotePrompt.text;
    }

    public float GetPercent()
    {
        return VoteBar.fillAmount;
    }

    public int GetApproxPercent()
    {
        //return Mathf.RoundToInt(VoteBar.fillAmount * 100);
        return _approxPercent;

    }

    public void MakePromptUnique()
    {
        VotePrompt.text += $"{_index + 1}";
    }
}
