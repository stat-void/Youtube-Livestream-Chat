using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;
using Void.YoutubeAPI;

public class ColorSettings : MonoBehaviour
{
    public static event Action OnColorsUpdated;

    /// <summary> Main color. Default is cyan. </summary>
    public static string MainColor { get; private set; } = "00FFFF";

    /// <summary> Secondary color. Default is half-cyan. </summary>
    public static string SecondaryColor { get; private set; } = "008080";

    /// <summary> Object background. Default is a very dark 0,20,20 cyan </summary>
    public static string ObjectBackgroundColor { get; private set; } = "001414";

    /// <summary> Background color. Default is near black (15,15,15). </summary>
    public static string BackgroundColor { get; private set; } = "0F0F0F";

    /// <summary> Regular user display color. Default is grayish. </summary>
    public static string RegularUserColor { get; private set; } = "BABABA";

    /// <summary> Member user display color. Default is a darkish green. </summary>
    public static string MemberColor { get; private set; } = "0D9D58";

    /// <summary> Moderator display color. Default is blue. </summary>
    public static string ModeratorColor { get; private set; } = "007FFF";

    /// <summary> Color of the streamer. Default is yellow. </summary>
    public static string OwnerColor { get; private set; } = "FFD603";

    /// <summary> Color for highlighted users. Default is yellow. </summary>
    public static string HighlightUserColor { get; private set; } = "FFD603";

    /// <summary> Color for regular messages. Default is white. </summary>
    public static string RegularMessageColor { get; private set; } = "FDFDFD";

    /// <summary> Color for highlighted messages. Default is yellow. </summary>
    public static string HighlightMessageColor { get; private set; } = "FFD603";

    //0-blue, 1-light blue, 2-yellowgreen, 3-yellow, 4-orange, 5-magenta, 6-red
    //private readonly List<string> _superColors =      new() { "1665C0", "00E5FF", "1EE9B6", "FFC927", "F57B02", "E81E63", "E62216" };
    /// <summary> Superchat colors. Defaults are a darker versions of original colors in ascending order - blue, light blue, yellowgreen, yellow, orange, magenta, red. </summary>
    public static List<string> UserSuperColors { get; private set; } = new() { "1665C0", "00B8D3", "01BFA5", "FEB300", "E65100", "C1195B", "D00001" };

    /// <summary> Superchat colors. Defaults are a lighter versions of original colors in ascending order - blue, light blue, yellowgreen, yellow, orange, magenta, red. </summary>
    public static List<string> MessageSuperColors { get; private set; } = new() { "1665C0", "00E5FF", "34FFCC", "FFC927", "FF850C", "FF357A", "FF3B2F" };


    private readonly string _defaultMainColor = "00FFFF";
    private readonly string _defaultSecondaryColor = "008080";
    private readonly string _defaultObjectBackgroundColor = "001414";
    private readonly string _defaultBackgroundColor = "0F0F0F";
    private readonly string _defaultRegularUserColor = "BABABA";
    private readonly string _defaultMemberUserColor = "0D9D58";
    private readonly string _defaultModeratorColor = "007FFF";
    private readonly string _defaultOwnerColor = "FFD603";
    private readonly string _defaultHighlightUserColor = "FFD603";
    private readonly string _defaultRegularMessageColor = "FDFDFD";
    private readonly string _defaultHighlightMessageColor = "FFD603";
    private readonly List<string> _defaultUserSuperColors = new() { "1665C0", "00B8D3", "01BFA5", "FEB300", "E65100", "C1195B", "D00001" };
    private readonly List<string> _defaultMessageSuperColors = new() { "1665C0", "00E5FF", "34FFCC", "FFC927", "FF850C", "FF357A", "FF3B2F" };


    public void RefreshColors()
    {
        JSONNode data = YoutubeData.GetData();
        JSONNode colorNode = data["personalisation"]["colors"];

        MainColor = !string.IsNullOrEmpty(colorNode["main"])
                ? colorNode["main"] : _defaultMainColor;
        SecondaryColor = !string.IsNullOrEmpty(colorNode["secondary"])
                ? colorNode["secondary"] : _defaultSecondaryColor;
        ObjectBackgroundColor = !string.IsNullOrEmpty(colorNode["objectBackground"])
                ? colorNode["objectBackground"] : _defaultObjectBackgroundColor;
        BackgroundColor = !string.IsNullOrEmpty(colorNode["background"])
                ? colorNode["background"] : _defaultBackgroundColor;
        RegularUserColor = !string.IsNullOrEmpty(colorNode["regularUser"])
                ? colorNode["regularUser"] : _defaultRegularUserColor;
        MemberColor = !string.IsNullOrEmpty(colorNode["memberUser"])
                ? colorNode["memberUser"] : _defaultMemberUserColor;
        ModeratorColor = !string.IsNullOrEmpty(colorNode["moderator"])
                ? colorNode["moderator"] : _defaultModeratorColor;
        OwnerColor = !string.IsNullOrEmpty(colorNode["owner"])
                ? colorNode["owner"] : _defaultOwnerColor;
        HighlightUserColor = !string.IsNullOrEmpty(colorNode["highlightUser"])
                ? colorNode["highlightUser"] : _defaultHighlightUserColor;
        RegularMessageColor = !string.IsNullOrEmpty(colorNode["regularMessage"])
                ? colorNode["regularMessage"] : _defaultRegularMessageColor;
        HighlightMessageColor = !string.IsNullOrEmpty(colorNode["highlightMessage"])
                ? colorNode["highlightMessage"] : _defaultHighlightMessageColor;

        for (int i = 0; i < UserSuperColors.Count; i++)
        {
            UserSuperColors[i] = !string.IsNullOrEmpty(colorNode[$"tier{i + 1}SuperUser"])
                ? colorNode[$"tier{i + 1}SuperUser"] : _defaultUserSuperColors[i];

            MessageSuperColors[i] = !string.IsNullOrEmpty(colorNode[$"tier{i + 1}SuperMessage"])
                ? colorNode[$"tier{i + 1}SuperMessage"] : _defaultMessageSuperColors[i];
        }

        OnColorsUpdated?.Invoke();
    }

    public void DefaultColors()
    {
        MainColor = _defaultMainColor;
        SecondaryColor = _defaultSecondaryColor;
        ObjectBackgroundColor = _defaultObjectBackgroundColor;
        BackgroundColor = _defaultBackgroundColor;
        RegularUserColor = _defaultRegularUserColor;
        MemberColor = _defaultMemberUserColor;
        ModeratorColor = _defaultModeratorColor;
        OwnerColor = _defaultOwnerColor;
        HighlightUserColor = _defaultHighlightUserColor;
        RegularMessageColor = _defaultRegularMessageColor;
        HighlightMessageColor = _defaultHighlightMessageColor;

        for (int i = 0; i < UserSuperColors.Count; i++)
        {
            UserSuperColors[i] = _defaultUserSuperColors[i];
            MessageSuperColors[i] = _defaultMessageSuperColors[i];
        }

        OnColorsUpdated?.Invoke();
    }

    public void SetColors(List<ColorField> colorFields)
    {
        MainColor               = colorFields[0].GetColorString();
        SecondaryColor          = colorFields[1].GetColorString();
        ObjectBackgroundColor   = colorFields[2].GetColorString();
        BackgroundColor         = colorFields[3].GetColorString();
        RegularUserColor        = colorFields[4].GetColorString();
        RegularMessageColor     = colorFields[5].GetColorString();
        MemberColor             = colorFields[6].GetColorString();
        ModeratorColor          = colorFields[7].GetColorString();
        OwnerColor              = colorFields[8].GetColorString();
        HighlightUserColor      = colorFields[9].GetColorString();
        HighlightMessageColor   = colorFields[10].GetColorString();

        int num = 11;

        for (int i = 0; i < UserSuperColors.Count; i++)
        {
            UserSuperColors[i] = colorFields[num].GetColorString();
            MessageSuperColors[i] = colorFields[num + 1].GetColorString();
            i += 2;
        }

        OnColorsUpdated?.Invoke();
    }

    public void SaveColors()
    {
        JSONNode data = YoutubeData.GetData();
        data["personalisation"]["colors"]["main"] = MainColor;
        data["personalisation"]["colors"]["secondary"] = SecondaryColor;
        data["personalisation"]["colors"]["objectBackground"] = ObjectBackgroundColor;
        data["personalisation"]["colors"]["background"] = BackgroundColor;
        data["personalisation"]["colors"]["regularUser"] = RegularUserColor;
        data["personalisation"]["colors"]["memberUser"] = MemberColor;
        data["personalisation"]["colors"]["moderator"] = ModeratorColor;
        data["personalisation"]["colors"]["owner"] = OwnerColor;
        data["personalisation"]["colors"]["highlightUser"] = HighlightUserColor;
        data["personalisation"]["colors"]["regularMessage"] = RegularMessageColor;
        data["personalisation"]["colors"]["highlightMessage"] = HighlightMessageColor;

        for (int i = 0; i < UserSuperColors.Count; i++)
        {
            data["personalisation"]["colors"][$"tier{i + 1}SuperUser"] = UserSuperColors[i];
            data["personalisation"]["colors"][$"tier{i + 1}SuperMessage"] = MessageSuperColors[i];
        }

    }
}
