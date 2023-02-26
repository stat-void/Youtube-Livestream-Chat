using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized InputField updating color listener.
/// </summary>
public class InputFieldColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Text;
    [SerializeField] protected TMP_Text Placeholder;
    [SerializeField] protected Image IFBackground;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color secondaryColor;
        Color objectBGColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.SecondaryColor, out secondaryColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out objectBGColor);

        Text.color = mainColor;
        Placeholder.color = secondaryColor;
        IFBackground.color = objectBGColor;
    }
}
