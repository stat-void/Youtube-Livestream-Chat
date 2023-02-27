using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Image color updater.
/// </summary>
public class ImageColorListener : ColorListener
{
    [SerializeField] protected Image Image;
    [SerializeField] protected ColorTypeEnum Type;

    protected override void ColorUpdate()
    {
        string colorChoice = Type switch
        {
            ColorTypeEnum.Secondary => ColorSettings.SecondaryColor,
            ColorTypeEnum.ObjectBackground => ColorSettings.ObjectBackgroundColor,
            ColorTypeEnum.Background => ColorSettings.BackgroundColor,
            ColorTypeEnum.Highlight => ColorSettings.HighlightUserColor,
            _ => ColorSettings.MainColor
        };

        Color color;
        ColorUtility.TryParseHtmlString("#" + colorChoice, out color);
        Image.color = color;
    }
}
