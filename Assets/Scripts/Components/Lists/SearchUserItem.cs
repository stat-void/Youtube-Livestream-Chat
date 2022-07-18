using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SearchUserItem : ARecyclerItem, IPointerClickHandler
{

    [SerializeField] protected Button Button;
    [SerializeField] protected Image Background;
    [SerializeField] protected TMP_Text UsernameField;

    public UserContainer User;

    private const string _highlightColor = "FFFFFF";


    protected override void OnDataBind(object data)
    {
        var node = data as UserContainer;
        User = node;

        UsernameField.text = User.Username;
    }

    protected override void OnDataBind(object data, (int, int) indexLengthTextHighlight)
    {
        var node = data as UserContainer;
        User = node;

        if (indexLengthTextHighlight != (0, 0))
            UsernameField.text = string.Format("{0}<color=#{1}>{2}</color>{3}",
                User.Username.Substring(0, indexLengthTextHighlight.Item1),
                _highlightColor,
                User.Username.Substring(indexLengthTextHighlight.Item1, indexLengthTextHighlight.Item2),
                User.Username.Substring(indexLengthTextHighlight.Item1 + indexLengthTextHighlight.Item2));
        else
            UsernameField.text = User.Username;
    }

    protected override void OnDataUnbind()
    {
        User = null;
    }

    public void OnPointerClick(PointerEventData eventData) =>
        ListPresenter.SetSelection(this);
    

    
}
