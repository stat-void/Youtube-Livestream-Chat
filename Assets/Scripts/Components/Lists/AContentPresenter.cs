using System;
using UnityEngine;

/// <summary> Abstract class used to present different views that interact with Youtube livestream chat information. </summary>
public abstract class AModePresenter : MonoBehaviour, IComparable<AModePresenter>
{
    /// <summary> Get the name of this content presenter. </summary>
    public abstract string GetName();

    /// <summary> Get the information on what this view does. </summary>
    public abstract string GetDescription();

    /// <summary> Perform all the necessary steps to make this view visible to the user. </summary>
    public abstract void Open();

    /// <summary> Perform all the necessary steps to hide this view from the user. </summary>
    public abstract void Close();

    /// <summary> Notify that this particular content presenter has loaded. </summary>
    public static event Action<AModePresenter> OnLoaded;


    /// <summary> Let classes listening to NotifyLoaded find out that this class is ready to use. </summary>
    protected void NotifyClassReady(AModePresenter presenter) =>
        OnLoaded?.Invoke(presenter);

    public int CompareTo(AModePresenter compMode)
    {
        return string.Compare(GetName(), compMode.GetName());
    }
}
