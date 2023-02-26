using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Toggle updating color listener.
/// </summary>
public class ScrollbarColorListener : ColorListener
{
    [SerializeField] protected Image ScrollHandle;
    [SerializeField] protected Image ScrollBackground;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color secondaryColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.SecondaryColor, out secondaryColor);

        ScrollHandle.color = mainColor;
        ScrollBackground.color = secondaryColor;
    }
}
