using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderField : MonoBehaviour
{
    [SerializeField] protected TMP_Text Name;
    [SerializeField] protected Slider Slider;
    [SerializeField] protected TMP_InputField Input;

    public event Action<float> OnSliderChange;

    private void Awake()
    {
        Slider.onValueChanged.AddListener(SliderChanged);
        Input.onValueChanged.AddListener(InputChanged);
    }

    private void OnDestroy()
    {
        Slider.onValueChanged.RemoveListener(SliderChanged);
        Input.onValueChanged.RemoveListener(InputChanged);

    }

    /// <summary>
    /// Set all the variables for the given Slider.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The current value of the slider.</param>
    /// <param name="min">Minimum value for the slider.</param>
    /// <param name="max">Maximum value for the slider.</param>
    public void Initialize(string name, float value, float min, float max)
    {
        Name.text = name;

        Slider.minValue = min;
        Slider.maxValue = max;
        Slider.value = Mathf.Clamp(value, min, max);

        Input.text = Slider.value.ToString();
    }

    private void SliderChanged(float value)
    {
        Input.SetTextWithoutNotify(value.ToString("0.00"));

        OnSliderChange?.Invoke(value);
    }

    private void InputChanged(string value)
    {
        try
        {
            float trueValue = Mathf.Clamp(float.Parse(value), Slider.minValue, Slider.maxValue);
            //Input.SetTextWithoutNotify(trueValue.ToString());
            Slider.SetValueWithoutNotify(trueValue);

            OnSliderChange?.Invoke(trueValue);
        }
        catch (FormatException e)
        {
            
        }
        
    }

}
