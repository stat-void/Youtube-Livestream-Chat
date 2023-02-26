using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Setting InputField updating color listener.
/// </summary>
public class SettingIFColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Name;

    [SerializeField] protected TMP_Text IFText;
    [SerializeField] protected Image IFBackground;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color objectBGColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out objectBGColor);

        Name.color = mainColor;
        IFText.color = mainColor;
        IFBackground.color = objectBGColor;
    }
}
