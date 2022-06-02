using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AContentPresenter : MonoBehaviour
{
    /// <summary> Perform all the necessary steps to make this view visible to the user. </summary>
    public abstract void Open();

    /// <summary> Perform all the necessary steps to hide this view from the user. </summary>
    public abstract void Close();
}
