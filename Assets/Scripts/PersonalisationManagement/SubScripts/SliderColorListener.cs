using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized slider updating color listener.
/// </summary>
public class SliderColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Name;

    [SerializeField] protected TMP_Text IFText;
    [SerializeField] protected Image IFBackground;
    
    [SerializeField] protected Image SliderBackground;
    [SerializeField] protected Image SliderFill;
    [SerializeField] protected Image SliderHandle;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color secondaryColor;
        Color objectBackgroundColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.SecondaryColor, out secondaryColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out objectBackgroundColor);

        Name.color = mainColor;
        IFText.color = mainColor;
        IFBackground.color = objectBackgroundColor;

        SliderBackground.color = secondaryColor;
        SliderFill.color = mainColor;
        SliderHandle.color = mainColor;
    }
}
