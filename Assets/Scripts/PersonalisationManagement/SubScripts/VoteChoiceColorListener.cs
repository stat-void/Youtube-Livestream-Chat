using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Specialized Vote Choice updating color listener.
/// </summary>
public class VoteChoiceColorListener : ColorListener
{
    [SerializeField] protected TMP_Text PromptText;
    [SerializeField] protected TMP_Text PromptPlaceholder;
    [SerializeField] protected Image PromptBackground;

    [SerializeField] protected TMP_Text Percentage;
    [SerializeField] protected Image PercentageBackground;

    [SerializeField] protected TMP_Text Number;
    [SerializeField] protected Image NumberBackground;

    [SerializeField] protected TMP_Text Answer;
    [SerializeField] protected TMP_Text AnswerPlaceholder;
    [SerializeField] protected Image AnswerBackground;

    [SerializeField] protected SlicedFilledImage PollFill;
    [SerializeField] protected SlicedFilledImage PollBase;
    
    
    

    protected override void ColorUpdate()
    {
        Color mainColor;
        Color secondaryColor;
        Color objectBGColor;

        ColorUtility.TryParseHtmlString("#" + ColorSettings.MainColor, out mainColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.SecondaryColor, out secondaryColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out objectBGColor);

        PromptText.color = mainColor;
        PromptPlaceholder.color = secondaryColor;
        PromptBackground.color = objectBGColor;

        Percentage.color = mainColor;
        PercentageBackground.color = objectBGColor;

        Number.color = mainColor;
        NumberBackground.color = objectBGColor;

        Answer.color = mainColor;
        AnswerPlaceholder.color = secondaryColor;
        AnswerBackground.color = objectBGColor;

        PollFill.color = mainColor;
        PollBase.color = objectBGColor;
        
        
        
    }
}
