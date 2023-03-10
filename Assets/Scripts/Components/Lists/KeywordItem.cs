using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class KeywordItem : ARecyclerItem, IPointerClickHandler
{
    [SerializeField] protected Button Button;
    [SerializeField] protected Image Background;
    [SerializeField] protected TMP_Text KeywordField;

    public KeywordContainer Keyword;

    private Color _bgColor;

    private void Awake()
    {
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out _bgColor);
    }

    protected override void OnDataBind(object data)
    {
        var node = data as KeywordContainer;
        Keyword = node;
        Background.color = _bgColor;

        KeywordField.text = Keyword.Keyword;
    }

    protected override void OnDataBind(object data, (int, int) indexLengthTextHighlight)
    {
        // Keywords don't use searching, so highlighting is not needed.
        OnDataBind(data);
    }

    protected override void OnDataUnbind()
    {
        Keyword = null;
    }

    public void OnPointerClick(PointerEventData eventData) =>
        ListPresenter.SetSelection(this);
}
