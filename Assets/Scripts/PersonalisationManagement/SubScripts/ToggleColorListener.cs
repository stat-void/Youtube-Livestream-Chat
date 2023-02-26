using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Toggle updating color listener.
/// </summary>
public class ToggleColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Name;

    [SerializeField] protected Image ToggleCheckmark;
    [SerializeField] protected Image ToggleBackground;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color secondaryColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.SecondaryColor, out secondaryColor);

        Name.color = mainColor;
        ToggleCheckmark.color = mainColor;
        ToggleBackground.color = secondaryColor;
    }
}
