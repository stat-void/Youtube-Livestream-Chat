using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Button updating color listener.
/// </summary>
public class ButtonColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Name;

    [SerializeField] protected Image ButtonBox;
    [SerializeField] protected Image ButtonBackground;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color objectBGColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out objectBGColor);

        Name.color = mainColor;
        ButtonBox.color = mainColor;
        ButtonBackground.color = objectBGColor;
    }
}
