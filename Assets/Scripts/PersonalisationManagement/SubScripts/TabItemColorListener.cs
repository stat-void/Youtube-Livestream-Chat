using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Tab Ttem updating color listener.
/// </summary>
public class TabItemColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Name;
    [SerializeField] protected TMP_Text CloseButtonText;

    [SerializeField] protected Image NameBackground;
    [SerializeField] protected Image CloseButtonBackground;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color objectBGColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out objectBGColor);

        Name.color = mainColor;
        CloseButtonText.color = mainColor;

        NameBackground.color = objectBGColor;
        CloseButtonBackground.color = objectBGColor;
    }
}
