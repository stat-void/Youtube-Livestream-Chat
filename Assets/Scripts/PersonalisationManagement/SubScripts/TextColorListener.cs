using UnityEngine;
using TMPro;

/// <summary>
/// Text color updater.
/// </summary>
public class TextColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Text;
    [SerializeField] protected ColorTypeEnum Type;

    protected override void ColorUpdate()
    {
        string colorChoice = Type switch
        {
            ColorTypeEnum.Secondary => ColorSettings.SecondaryColor,
            ColorTypeEnum.ObjectBackground => ColorSettings.ObjectBackgroundColor,
            ColorTypeEnum.Background => ColorSettings.BackgroundColor,
            ColorTypeEnum.Highlight => ColorSettings.HighlightMessageColor,
            _ => ColorSettings.MainColor
        };

        Color color;
        ColorUtility.TryParseHtmlString("#" + colorChoice, out color);
        Text.color = color;
    }
}
