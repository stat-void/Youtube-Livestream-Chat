using UnityEngine;

/// <summary>
/// Base abstract class that listens to whenever colors are updated.
/// Attach its child classes to GameObjects that want to update a particular component color;
/// </summary>
public abstract class ColorListener : MonoBehaviour
{
    private void Awake()
        => ColorSettings.OnColorsUpdated += ColorUpdate;
    
    private void OnDestroy()
        => ColorSettings.OnColorsUpdated -= ColorUpdate;

    private void OnEnable()
        => ColorUpdate();
    
    protected abstract void ColorUpdate();
}
