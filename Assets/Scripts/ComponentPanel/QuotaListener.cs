using UnityEngine;
using Void.YoutubeAPI;
using TMPro;

public class QuotaListener : MonoBehaviour
{
    [SerializeField] protected YoutubeQuotaManager QuotaManager;
    [SerializeField] protected TMP_Text QuotaUsed;


    private void Start()
    {
        QuotaUsed.text = QuotaManager.CurrentQuota.ToString();
        QuotaManager.OnQuotaUpdate += OnQuotaUpdate;
    }

    private void OnQuotaUpdate(int newTotal)
    {
        QuotaUsed.text = newTotal.ToString();
    }
}
