using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectionPresenter : MonoBehaviour
{
    [SerializeField] protected Canvas BaseCanvas;

    [Header("Mode selection functions")]
    [SerializeField] protected ScrollRect ScrollRect;
    [SerializeField] protected Transform ScrollerContent;

    [SerializeField] protected TMP_Text DescriptorText;

    [SerializeField] protected SelectableModeItem ButtonPrefab;
    //TODO: Some form of way to show animation demos of these?

    private ModeManagerPresenter _modeManager;
    private List<SelectableModeItem> _activeModes = new();

    private bool _setupDone = false;

    private void Start()
    {
        _modeManager = FindObjectOfType<ModeManagerPresenter>();
        _modeManager.AddModeButton.onClick.AddListener(Open);
        _modeManager.CloseModeButton.onClick.AddListener(Close);

        BaseCanvas.gameObject.SetActive(false);

        SelectableModeItem.OnClick += OnChoiceClicked;
        SelectableModeItem.OnMouseHover += OnChoiceHovered;
        //ScreenResizeListener.OnResize += OnScreenResize;
    }

    private void Open()
    {
        // First time Scrollbar initialization
        if (!_setupDone)
        {
            foreach (var mode in _modeManager.GetAllModes())
            {
                var item = Instantiate(ButtonPrefab, ScrollerContent);
                item.Bind(mode, ScrollerContent);

                _activeModes.Add(item);
            }
            _setupDone = true;
        }

        BaseCanvas.gameObject.SetActive(true);
    }


    private void Close()
    {
        BaseCanvas.gameObject.SetActive(false);
    }

    private void OnChoiceHovered(string desc) =>
        DescriptorText.text = desc;

    private void OnChoiceClicked(AModePresenter obj)
    {
        DescriptorText.text = "";
        _modeManager.CloseButtonPressed();
        Close();
    }
        

    /*private void OnScreenResize(Vector2 anchorWorldMin, Vector2 anchorWorldMax)
    {
        foreach (var item in _activeModes)
            item.UpdateFit();
    }*/

}
