using System;
using System.Threading.Tasks;
using UnityEngine;

public class ScreenResizeListener : MonoBehaviour
{
    /// <summary> Invoke the current minimum and maximum world positions of this transform. </summary>
    public static event Action<Vector2, Vector2> OnResize;

    private RectTransform _rect;
    private bool _dimensionsChanged = false;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private async void OnRectTransformDimensionsChange()
    {
        if (_rect == null)
            return;

        if (_dimensionsChanged)
            return;

        _dimensionsChanged = true;

        //TODO: This is not right, in the future for RecyclerViews, you want to grab this RectTransforms corners relative to screen size.
        var anchorWorldMin = _rect.anchorMin * Screen.width;
        var anchorWorldMax = _rect.anchorMax * Screen.height;

        OnResize?.Invoke(anchorWorldMin, anchorWorldMax);
        await Task.Yield();
        _dimensionsChanged = false;
    }
}
