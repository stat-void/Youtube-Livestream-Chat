using UnityEngine;

public interface IRecyclerItem
{
    /// <summary> Initializes this virtual item to work with the given list presenter. Use after creating an instance. </summary>
    void Initialize(IRecyclerList listPresenter);

    /// <summary> Set data for this object. </summary>
    void Bind(object data);

    /// <summary> Set data for this object, with values on what text to highlight. </summary>
    void Bind(object data, (int, int) indexLengthTextHighlight);

    /// <summary> Clears the data from this object. </summary>
    void Unbind();

    /// <summary> Top edge of the object to calculate pooling element status. </summary>
    float TopEdge { get; }

    /// <summary> Bottom edge of the object to calculate pooling element status. </summary>
    float BottomEdge { get; }

    /// <summary> General transform to use for setting Unity side sibling order. </summary>
    RectTransform Transform { get; }
}
