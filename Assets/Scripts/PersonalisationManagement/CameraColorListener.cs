using UnityEngine;

/// <summary>
/// Color updater for the camera background.
/// </summary>
public class CameraColorListener : ColorListener
{
    [SerializeField] protected Camera Camera;

    protected override void ColorUpdate()
    {
        Color color;
        ColorUtility.TryParseHtmlString("#" + ColorSettings.BackgroundColor, out color);
        Camera.backgroundColor = color;
    }
}
