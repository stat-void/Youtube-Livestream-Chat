using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    [SerializeField] protected Button Button;

    [SerializeField] protected List<string> Choices;
    [SerializeField] protected TMP_Text DisplayText;

    public event Action<string> OnSwitchUpdate;

    private int _currentChoice = 0;

    private void Awake()
    {
        if (Choices.Count > 0)
            DisplayText.text = Choices[0];
    }

    private void OnEnable()
    {
        Button.onClick.AddListener(UpdateChoice);

    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(UpdateChoice);
    }

    public string GetCurrentChoice()
        => DisplayText.text;

    private void UpdateChoice()
    {
        _currentChoice = (_currentChoice + 1) % Choices.Count;
        DisplayText.text = Choices[_currentChoice];

        OnSwitchUpdate?.Invoke(Choices[_currentChoice]);
    }
}
