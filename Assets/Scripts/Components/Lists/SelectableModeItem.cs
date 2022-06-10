using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableModeItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected RectTransform RectTransform;

    [SerializeField] protected Button Button;
    [SerializeField] protected TMP_Text ModeName;

    private AModePresenter _mode;
    private string _description;


    public static event Action<AModePresenter> OnClick;
    public static event Action<string> OnMouseHover;

    public void Bind(AModePresenter mode, Transform active)
    {
        transform.SetParent(active);
        _mode = mode;

        ModeName.text = _mode.GetName();
        _description = _mode.GetDescription();

        Button.onClick.AddListener(OnButtonClick);

    }

    public void UpdateFit()
    {
        if (RectTransform)
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, ModeName.preferredHeight + 10);
    }


    private void OnButtonClick() =>
        OnClick?.Invoke(_mode);

    public void OnPointerEnter(PointerEventData eventData) =>
        OnMouseHover?.Invoke(_description);
    
    public void OnPointerExit(PointerEventData eventData) =>
        OnMouseHover?.Invoke("");
    



}
