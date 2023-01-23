using UnityEngine;
using Void.YoutubeAPI;
using Void.YoutubeAPI.LiveStreamChat.Messages;
using TMPro;
using System.Threading.Tasks;

public class QuotaListener : MonoBehaviour
{
    [SerializeField] protected TMP_Text QuotaUsed;

    private int _warningLimit;
    private bool _warningReached;
    private bool _limitReached;

    private YoutubeKeyManager _keyManager;

    private void Start()
    {
        _keyManager = FindObjectOfType<YoutubeLiveChatMessages>().KeyManager;

        QuotaUsed.text = _keyManager.CurrentQuota.ToString();
        _keyManager.OnQuotaUpdate += OnQuotaUpdate;

        _warningLimit = Mathf.FloorToInt(_keyManager.MaxQuota * 0.9f);
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

        if (!_limitReached && newTotal >= _keyManager.MaxQuota)
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
