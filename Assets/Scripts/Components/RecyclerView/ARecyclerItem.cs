using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement), typeof(RectTransform))]
public abstract class ARecyclerItem : MonoBehaviour, IRecyclerItem
{
    protected IRecyclerListPresenter ListPresenter;

    private LayoutElement _layoutElement;
    private RectTransform _rectTransform;

    public RectTransform Transform => _rectTransform;

    public float TopEdge =>
        _rectTransform.anchoredPosition.y + 0.5f * _layoutElement.minHeight;

    public float BottomEdge =>
        _rectTransform.anchoredPosition.y - 0.5f * _layoutElement.minHeight;


    public void Initialize(IRecyclerListPresenter listPresenter)
    {
        ListPresenter  = listPresenter;
        _layoutElement = GetComponent<LayoutElement>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Bind(object data)
    {
        OnDataBind(data);
        _rectTransform.SetParent(ListPresenter.ContentParent);
        gameObject.SetActive(true);
    }

    public void Bind(object data, (int, int) indexLengthTextHighlight)
    {
        if (indexLengthTextHighlight.Item2 > 0)
            OnDataBind(data, indexLengthTextHighlight);
        else
            OnDataBind(data);
        _rectTransform.SetParent(ListPresenter.ContentParent);
        gameObject.SetActive(true);
    }

    public void Unbind()
    {
        OnDataUnbind();

        _rectTransform.SetParent(ListPresenter.PoolParent);
        gameObject.SetActive(false);
    }

    protected abstract void OnDataBind(object data);
    protected abstract void OnDataBind(object data, (int, int) indexLengthTextHighlight);
    protected abstract void OnDataUnbind();

}
