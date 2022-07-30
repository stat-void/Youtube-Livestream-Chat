using UnityEngine;

public interface IRecyclerList
{
    /// <summary> Informs the presenter that an item was selected by the user. </summary>
    /// <param name="selected"> The selected line from the list. </param>
    void SetSelection(object selected);

    /// <summary> Parent tranform of active virtual lines with data bindings. </summary>
    RectTransform ContentParent { get; }

    /// <summary> Parent tranfsorm of inactive virtual line. </summary>
    RectTransform PoolParent { get; }
}
