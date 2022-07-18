using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AssetListItem : ARecyclerItem, IPointerClickHandler
{
    [SerializeField] protected GameObject contentMask; // The whole object content, to be disableable without deactivating script
    [SerializeField] protected TMP_Text mainText;

    //private Asset asset;
    //internal Asset Asset => asset;

    private void Awake()
    {

    }

    private void OnDestroy()
    {
        //if (asset != null)
        //    asset.OnAvailablityChanged -= Asset_OnAvailablityChanged;
    }

    private void Present() // Asset target
    {
        //if (asset != null)
        //    asset.OnAvailablityChanged -= Asset_OnAvailablityChanged;

        //asset = target;
        //asset.OnAvailablityChanged += Asset_OnAvailablityChanged;

        UpdateDisplay();
    }

    private void Present((int, int) indexLengthTextHighlight) //Also had Asset target
    {
        //if (asset != null)
        //    asset.OnAvailablityChanged -= Asset_OnAvailablityChanged;

        //asset = target;
        //asset.OnAvailablityChanged += Asset_OnAvailablityChanged;

        UpdateDisplay(indexLengthTextHighlight);
    }


    private void UpdateDisplay()
    {
        mainText.text = "I need text"; //asset.FriendlyName;
    }

    private void UpdateDisplay((int, int) indexLengthTextHighlight)
    {
        string color = "FFFFFF";

        if (indexLengthTextHighlight != (0, 0))
            mainText.text = "I need text";/*string.Format("{0}<color=#{1}>{2}</color>{3}",
                asset.FriendlyName.Substring(0, indexLengthTextHighlight.Item1),
                color,
                asset.FriendlyName.Substring(indexLengthTextHighlight.Item1, indexLengthTextHighlight.Item2),
                asset.FriendlyName.Substring(indexLengthTextHighlight.Item1 + indexLengthTextHighlight.Item2));*/
        else
            mainText.text = "I need text"; //asset.FriendlyName;

    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        //listPresenter.SetSelection(this);
    }

    protected override void OnDataBind(object data)
    {
        //asset = data as Asset;
        //Present(asset);
    }

    protected override void OnDataBind(object data, (int, int) indexLengthTextHighlight)
    {
        //asset = data as Asset;
        //Present(asset, indexLengthTextHighlight);
    }

    protected override void OnDataUnbind()
    {
        //if (asset == null)
        //    return;

        //asset.OnAvailablityChanged -= Asset_OnAvailablityChanged;
        //asset = null;
    }

}
