using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SearchUserItem : ARecyclerItem, IPointerClickHandler
{
    [SerializeField] protected Button Button;
    [SerializeField] protected Image Background;
    [SerializeField] protected TMP_Text UsernameField;

    public UserContainer User;

    private Color _bgColor;
    private Color _bgHighlighted;

    private void Awake()
    {
        ColorUtility.TryParseHtmlString("#" + ColorSettings.ObjectBackgroundColor, out _bgColor);
        ColorUtility.TryParseHtmlString("#" + ColorSettings.SecondaryColor, out _bgHighlighted);
    }

    protected override void OnDataBind(object data)
    {
        var node = data as UserContainer;
        User = node;
        Background.color = User.Highlighted ? _bgHighlighted : _bgColor;

        UsernameField.text = User.Username;
    }

    protected override void OnDataBind(object data, (int, int) indexLengthTextHighlight)
    {
        var node = data as UserContainer;
        User = node;
        Background.color = User.Highlighted ? _bgHighlighted : _bgColor;

        if (indexLengthTextHighlight != (0, 0))
            UsernameField.text = string.Format("{0}<color=#{1}>{2}</color>{3}",
                User.Username.Substring(0, indexLengthTextHighlight.Item1),
                ColorSettings.HighlightUserColor,
                User.Username.Substring(indexLengthTextHighlight.Item1, indexLengthTextHighlight.Item2),
                User.Username.Substring(indexLengthTextHighlight.Item1 + indexLengthTextHighlight.Item2));
        else
            UsernameField.text = User.Username;
    }

    protected override void OnDataUnbind()
    {
        User = null;
    }

    public void UpdateHighlightState(bool state)
    {
        User.Highlighted = state;
        Background.color = User.Highlighted ? _bgHighlighted : _bgColor;
    }

    public void RefreshHighlight()
    {
        Background.color = User.Highlighted ? _bgHighlighted : _bgColor;
    }

    public void OnPointerClick(PointerEventData eventData) =>
        ListPresenter.SetSelection(this);
    

    
}
