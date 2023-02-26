using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Toggle updating color listener.
/// </summary>
public class ColorEditFieldListener : ColorListener
{
    [SerializeField] protected TMP_Text Name;

    [SerializeField] protected TMP_Text RedText;
    [SerializeField] protected Image RedBackground;
    [SerializeField] protected TMP_Text GreenText;
    [SerializeField] protected Image GreenBackground;
    [SerializeField] protected TMP_Text BlueText;
    [SerializeField] protected Image BlueBackground;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color objectBGColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out objectBGColor);

        Name.color = mainColor;
        RedText.color = mainColor;
        GreenText.color = mainColor;
        BlueText.color = mainColor;
        RedBackground.color = objectBGColor;
        GreenBackground.color = objectBGColor;
        BlueBackground.color = objectBGColor;
    }
}
