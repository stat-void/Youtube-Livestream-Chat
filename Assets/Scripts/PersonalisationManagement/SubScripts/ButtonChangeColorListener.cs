using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Button (menu) updating color listener.
/// </summary>
public class ButtonChangeColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Name;
    [SerializeField] protected Image ButtonBox;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color secondaryColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.SecondaryColor, out secondaryColor);

        Name.color = mainColor;
        ButtonBox.color = secondaryColor;
    }
}
