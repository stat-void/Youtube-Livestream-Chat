using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Image color updater for basic image elements that are always going to be visible in the app.
/// </summary>
public class PrimaryImageColorListener : ColorListener
{
    [SerializeField] protected Image Image;

    protected override void ColorUpdate()
    {
        Color color;
        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out color);
        Image.color = color;
    }
}
