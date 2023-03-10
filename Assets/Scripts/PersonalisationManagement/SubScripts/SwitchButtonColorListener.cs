using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Switch Button updating color listener.
/// </summary>
public class SwitchButtonColorListener : ColorListener
{
    [SerializeField] protected TMP_Text ButtonText1;
    [SerializeField] protected TMP_Text ButtonText2;
    [SerializeField] protected Image ButtonBG;
    

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color secondaryColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.SecondaryColor, out secondaryColor);

        ButtonText1.color = mainColor;
        ButtonText2.color = mainColor;
        ButtonBG.color = secondaryColor;
    }
}
