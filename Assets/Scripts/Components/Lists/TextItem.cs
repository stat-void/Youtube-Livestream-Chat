using UnityEngine;
using TMPro;

/// <summary> Simple class used for resize related events, to automatically adjust height. </summary>
public class TextItem : MonoBehaviour
{
    [SerializeField] protected RectTransform Transform;
    [SerializeField] protected TMP_Text Text;
    [SerializeField] protected Type ItemType;

    public void UpdateFit()
    {
        if (Transform)
        {
            int size = 0;
            switch (ItemType)
            {
                case Type.Text:
                    break;
                case Type.Button:
                    size = 34;
                    break;
            }
            Transform.sizeDelta = new Vector2(Transform.sizeDelta.x, Text.preferredHeight + size);
        }
            
    }

    public enum Type
    {
        Text,
        Button
    }
}
