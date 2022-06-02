using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedViewPresenter : MonoBehaviour
{
    public static Transform Transform;

    private void Awake()
    {
        if (Transform != null)
        {
            Debug.LogWarning($"Transform of {gameObject.name} can't be made static, as {Transform.gameObject} has already occupied it.");
            return;
        }
        Transform = transform;
    }
}
