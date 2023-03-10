using System.Collections.Generic;

/// <summary> Custom class for messages to store what parts of strings need to be highlighted. </summary>
public class MultiRange
{
    private SortedDictionary<int, int> _indexLengthPairs = new(new ReverseComparer<int>());
    private Dictionary<int, int> _numberBasePairs = new();

    /// <summary>
    /// Get the sorted dictionary of string highlights in REVERSE key order.
    /// Reversed because rich text highlighting will change string sizes.
    /// </summary>
    public SortedDictionary<int, int> GetIndexLengthPairs()
        => _indexLengthPairs;

    public int Count
        => _indexLengthPairs.Count;

    public void Add(int index, int length)
    {
        if (length == 0)
            return;

        // Case 0 - The given index is the newest starting index, and nothing else takes control.
        int baseNewNumber = index;

        // Case 1 - The given index already has a number
        if (_numberBasePairs.ContainsKey(index))
            baseNewNumber = _numberBasePairs[index];

        // Case 1a - Otherwise, there is a chance that the numberBasePair of this is empty
        else
            _numberBasePairs[index] = index;

        // Case 2 - The preceeding number has a base value, which could be smaller than the current one
        if (index > 0 && _numberBasePairs.ContainsKey(index - 1))
        {
            baseNewNumber = _numberBasePairs[index - 1];
            _numberBasePairs[index] = baseNewNumber;
        }
            
        // Go through the main match with its index and length
        for (int i = index+1; i < index + length; i++)
        {
            CheckAndAdd(i, baseNewNumber);
        }

        // Keep going if there exist values
        int lastPass = index + length;

        while (_numberBasePairs.ContainsKey(lastPass))
        {
            CheckAndAdd(lastPass, baseNewNumber);
            lastPass++;
        }

        // Now take the true end length
        int trueLength = lastPass - baseNewNumber;

        if (trueLength > 0)
            _indexLengthPairs[baseNewNumber] = trueLength;
    }

    private void CheckAndAdd(int index, int baseVal)
    {
        // If an _indexLengthPair is hit, remove it, as it's going to become a larger part.
        if (_indexLengthPairs.ContainsKey(index))
            _indexLengthPairs.Remove(index);

        // Repoint _numberBasePairs value
        _numberBasePairs[index] = baseVal;
    }
}
