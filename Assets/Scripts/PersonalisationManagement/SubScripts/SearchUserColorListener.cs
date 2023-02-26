using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Search User prefab updating color listener.
/// </summary>
public class SearchUserColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Name;
    [SerializeField] protected Image FieldBackground;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color objectBGColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out objectBGColor);

        Name.color = mainColor;
        FieldBackground.color = objectBGColor;
    }
}
