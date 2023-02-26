using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Chat Message updating color listener.
/// </summary>
public class ChatMessageColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Timestamp;
    [SerializeField] protected TMP_Text Contents;
    [SerializeField] protected Image Divider;

    protected override void ColorUpdate()
    {
        Color mainColor;
        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);

        Timestamp.color = mainColor;
        Contents.color = mainColor;
        Divider.color = mainColor;
    }
}
