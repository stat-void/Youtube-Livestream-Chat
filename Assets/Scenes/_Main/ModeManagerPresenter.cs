using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeManagerPresenter : MonoBehaviour
{
    [SerializeField] protected GameObject ComponentPanel;
    [SerializeField] protected Transform ActiveTabs;
    [SerializeField] protected Transform TabPool;
    [SerializeField] protected TabItem TabPrefab;

    [Header("View specific functions")]
    public Button AddModeButton;
    public Button CloseModeButton;

    // For holding all types of usable modes in memory
    private Dictionary<string, AModePresenter> _modes = new();

    // For presenting selected modes
    private TabItem _currentTab = null;
    private List<TabItem> _tabs = new();
    private Stack<TabItem> _objectPool = new();


    private void Awake()
    {
        AModePresenter.OnLoaded += OnNewModeAdded;
        SelectableModeItem.OnClick += OpenDirect;
        TabItem.OnClick += OnTabItemSelected;
        TabItem.OnDelete += OnTabCloseRequested;

        AddModeButton.onClick.AddListener(AddButtonPressed);
        CloseModeButton.onClick.AddListener(CloseButtonPressed);
    }

    private void OnNewModeAdded(AModePresenter presenter)
    {
        _modes.Add(presenter.GetName(), presenter);
    }

    public void ShowAddModeButton()
    {
        AddModeButton.gameObject.SetActive(true);
    }

    /// <summary> Open a mode by the direct name they were given. </summary>
    /// <param name="name"> The name of the mode to open.</param>
    public void OpenByName(string name)
    {
        if (_modes.TryGetValue(name, out var mode))
            OpenDirect(mode);

        else
            Debug.LogWarning($"Did not find a mode with the name \"{name}\"");
    }

    private void OpenDirect(AModePresenter mode)
    {

        if (_currentTab != null)
        {
            // Don't do anything if attempting to open the currently active tab
            if (mode.GetName() == _currentTab.ModePresenter.GetName())
                return;

            _currentTab.Deselect();
            _currentTab.ModePresenter.Close();
        }

        // Find or create the tab for the given mode
        bool tabExists = false;
        foreach(TabItem tab in _tabs)
        {
            if (tab.ModePresenter.GetName() == mode.GetName())
            {
                tabExists = true;
                _currentTab = tab;
                break;
            }
        }

        if (!tabExists)
        {
            _currentTab = GetPoolItem();
            _currentTab.Bind(mode);
            _tabs.Add(_currentTab);
        }

        _currentTab.Select();
        mode.Open();
    }

    private void OnTabItemSelected(TabItem tab)
    {
        if (_currentTab &&
            tab.ModePresenter.GetName() == _currentTab.ModePresenter.GetName())
            return;

        OpenDirect(tab.ModePresenter);
    }

    private void OnTabCloseRequested()
    {
        int index = -1;

        // If there is more than one tab, select and open a new tab
        if (_tabs.Count > 1)
        {
            index = 0;

            foreach (TabItem tab in _tabs)
            {
                if (tab.ModePresenter.GetName() == _currentTab.ModePresenter.GetName())
                    break;

                index++;
            }

            if (index == 0)
            {
                _tabs[index + 1].Select();
                _tabs[index + 1].ModePresenter.Open();
            }
            else
            {
                _tabs[index - 1].Select();
                _tabs[index - 1].ModePresenter.Open();
            }
        }

        _tabs.Remove(_currentTab);
        _currentTab.Unbind(TabPool);

        if (index == -1)
            _currentTab = null;
        else if (index == 0)
            _currentTab = _tabs[index];
        else
            _currentTab = _tabs[index - 1];
    }

    private void AddButtonPressed()
    {
        AddModeButton.gameObject.SetActive(false);
        CloseModeButton.gameObject.SetActive(true);
    }

    public void CloseButtonPressed()
    {
        AddModeButton.gameObject.SetActive(true);
        CloseModeButton.gameObject.SetActive(false);
    }

    public List<AModePresenter> GetAllModes()
    {
        List<AModePresenter> baseList = new(_modes.Values);
        baseList.Sort();

        return baseList;
    }


    private TabItem GetPoolItem()
    {
        if (_objectPool.Count > 0)
            return _objectPool.Pop();

        // Create new pooling item
        var item = Instantiate(TabPrefab, ActiveTabs);
        return item;
    }

}
