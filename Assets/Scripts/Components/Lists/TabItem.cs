using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabItem : MonoBehaviour
{
    public static event Action<TabItem> OnClick;
    public static event Action OnDelete;

    [SerializeField] protected Button MainButton;
    [SerializeField] protected TMP_Text MainButtonText;
    [SerializeField] protected Button DeleteButton;

     public AModePresenter ModePresenter { get; private set; }
    

    private void Awake()
    {
        MainButton.onClick.AddListener(OnMainButtonClicked);
        DeleteButton.onClick.AddListener(OnDeleteButtonClicked);
    }

    public void Bind(AModePresenter presenter)
    {
        gameObject.SetActive(true);
        ModePresenter = presenter;
        MainButtonText.text = ModePresenter.GetName();
    }

    public void Unbind(Transform pool)
    {
        ModePresenter.Close();
        Deselect();

        ModePresenter = null;
        MainButtonText.text = "";
        transform.SetParent(pool);
        gameObject.SetActive(false);
    }

    private void OnMainButtonClicked() =>
        OnClick?.Invoke(this);

    private void OnDeleteButtonClicked() =>
        OnDelete?.Invoke();


    public void Select() =>
        DeleteButton.gameObject.SetActive(true);

    public void Deselect() =>
        DeleteButton.gameObject.SetActive(false);

}
