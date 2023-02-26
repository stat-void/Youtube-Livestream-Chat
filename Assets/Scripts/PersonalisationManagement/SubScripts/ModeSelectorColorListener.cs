using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Mode Selector prefab updating color listener.
/// </summary>
public class ModeSelectorColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Name;

    [SerializeField] protected Image ButtonBG;
    [SerializeField] protected Image SeparationLine;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color secondaryColor;
        Color objectBGColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.SecondaryColor, out secondaryColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out objectBGColor);

        Name.color = mainColor;
        SeparationLine.color = secondaryColor;
        ButtonBG.color = objectBGColor;
    }
}
