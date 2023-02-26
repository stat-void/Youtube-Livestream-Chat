using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Poll Item updating color listener.
/// </summary>
public class PollItemColorListener : ColorListener
{
    [SerializeField] protected TMP_Text Percentage;
    [SerializeField] protected TMP_Text Number;
    [SerializeField] protected TMP_Text Answer;

    [SerializeField] protected SlicedFilledImage PollFill;
    [SerializeField] protected SlicedFilledImage PollBase;
    [SerializeField] protected Image PercentageBackground;
    [SerializeField] protected Image NumberBackground;
    [SerializeField] protected Image AnswerBackground;

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color objectBGColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out objectBGColor);

        Percentage.color = mainColor;
        Number.color = mainColor;
        Answer.color = mainColor;
        PollFill.color = mainColor;

        PollBase.color = objectBGColor;
        PercentageBackground.color = objectBGColor;
        NumberBackground.color = objectBGColor;
        AnswerBackground.color = objectBGColor;
    }
}
