using TMPro;
using UnityEngine;

/// <summary>
/// Specialized Time filler updating color listener.
/// </summary>
public class TimeFillColorListener : ColorListener
{
    [SerializeField] protected SlicedFilledImage Base;
    [SerializeField] protected SlicedFilledImage Fill;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color secondaryColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.SecondaryColor, out secondaryColor);

        Fill.color = mainColor;
        Base.color = secondaryColor;
    }
}
