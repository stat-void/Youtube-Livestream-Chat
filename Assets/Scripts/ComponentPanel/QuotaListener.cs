using UnityEngine;
using Void.YoutubeAPI;
using TMPro;
using System.Threading.Tasks;

public class QuotaListener : MonoBehaviour
{
    [SerializeField] protected YoutubeQuotaManager QuotaManager;
    [SerializeField] protected TMP_Text QuotaUsed;

    private int _warningLimit;
    private bool _warningReached;
    private bool _limitReached;


    private void Start()
    {
        QuotaUsed.text = QuotaManager.CurrentQuota.ToString();
        QuotaManager.OnQuotaUpdate += OnQuotaUpdate;

        _warningLimit = Mathf.FloorToInt(QuotaManager.MaxQuota * 0.9f);
        _warningReached = false;
        _limitReached = false;
    }

    private void OnQuotaUpdate(int newTotal)
    {
        QuotaUsed.text = newTotal.ToString();

        if (!_warningReached && newTotal >= _warningLimit)
        {
            _warningReached = true;
            ColorLerp(QuotaUsed.color, new Color32(255,210,0,255), 2);

        }

        if (!_limitReached && newTotal >= QuotaManager.MaxQuota)
        {
            _limitReached = true;
            ColorLerp(QuotaUsed.color, new Color32(255, 0, 65, 255), 2);
        }
    }

    private async void ColorLerp(Color from, Color to, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            QuotaUsed.color = Color.Lerp(from, to, time/duration);
            await Task.Yield();
        }

        QuotaUsed.color = to;
    }
}
