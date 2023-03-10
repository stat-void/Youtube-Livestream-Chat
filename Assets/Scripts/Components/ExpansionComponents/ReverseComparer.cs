using System.Collections.Generic;

/// <summary>
/// Variant IComparer that sorts items in reverse.
/// </summary>
/// <typeparam name="T">The given class used for comparison.</typeparam>
public sealed class ReverseComparer<T> : IComparer<T>
{
    private readonly IComparer<T> inner;
    public ReverseComparer() : this(null) { }
    public ReverseComparer(IComparer<T> inner)
    {
        this.inner = inner ?? Comparer<T>.Default;
    }
    int IComparer<T>.Compare(T x, T y) { return inner.Compare(y, x); }
}