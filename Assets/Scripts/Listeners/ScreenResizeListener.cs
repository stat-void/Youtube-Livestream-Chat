using System;
using System.Threading.Tasks;
using UnityEngine;

public class ScreenResizeListener : MonoBehaviour
{
    /// <summary> Invoke the current minimum and maximum world positions of this transform. </summary>
    public static event Action OnResize;

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

        OnResize?.Invoke();
        await Task.Yield();
        _dimensionsChanged = false;
    }
}
