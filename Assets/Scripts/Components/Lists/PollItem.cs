using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class PollItem : MonoBehaviour
{
    [SerializeField] protected TMP_Text PollMessage;
    [SerializeField] protected TMP_Text PollPercentage;
    [SerializeField] protected TMP_Text PollCount;
    [SerializeField] protected SlicedFilledImage PollBar;

    private int _approxPercent = 0;
    private int _pollCount = 0;


    public void Initialize(Transform parent)
    {
        ResetData();
        transform.SetParent(parent);
    }

    public void ResetData()
    {
        PollMessage.text = "";
        PollPercentage.text = "";
        PollCount.text = "";
        PollBar.fillAmount = 0;
        _approxPercent = 0;
        _pollCount = 0;
    }

    public void UpdatePoll(int count, float percent, int approxPercent)
    {
        if (percent == 0)
        {
            ResetData();
        }

        else
        {
            PollCount.text = count.ToString();
            PollBar.fillAmount = percent;
            PollPercentage.text = $"{approxPercent}%";

            _pollCount = count;
            _approxPercent = approxPercent;
        }
        
    }

    public void UpdateMessage(string message)
    {
        PollMessage.text = message;
    }

    public string GetMessage()
    {
        return PollMessage.text;
    }

    public int GetCount()
    {
        return _pollCount;
    }

    public float GetPercent()
    {
        return PollBar.fillAmount;
    }

    public int GetApproxPercent()
    {
        return _approxPercent;
    }
}
