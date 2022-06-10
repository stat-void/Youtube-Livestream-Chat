using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeManagerPresenter : MonoBehaviour
{
    [SerializeField] protected GameObject ComponentPanel;

    [Header("View specific functions")]
    [SerializeField] public Button AddModeButton;

    private Dictionary<string, AModePresenter> _modes = new();
    private AModePresenter _currentMode = null;

    private void Awake()
    {
        AModePresenter.OnLoaded += OnNewModeAdded;
        SelectableModeItem.OnClick += OpenDirect;
    }

    private void OnNewModeAdded(AModePresenter presenter)
    {
        _modes.Add(presenter.GetName(), presenter);
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
        if (_currentMode != null)
            _currentMode.Close();

        _currentMode = mode;
        mode.Open();

        //TODO: Check if this mode is already added as a tab and if not, do so.
    }

    public List<AModePresenter> GetAllModes()
    {
        List<AModePresenter> baseList = new(_modes.Values);
        baseList.Sort();

        return baseList;
    }

}
