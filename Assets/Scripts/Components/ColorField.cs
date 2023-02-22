using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorField : MonoBehaviour
{
    [SerializeField] protected TMP_Text NameField;
    [SerializeField] protected TMP_InputField RedField;
    [SerializeField] protected TMP_InputField GreenField;
    [SerializeField] protected TMP_InputField BlueField;
    [SerializeField] protected Image ColorShower;

    public event Action<Color> OnColorChange;


    private void Awake()
    {
        RedField.onValueChanged.AddListener(RedValueChanged);
        GreenField.onValueChanged.AddListener(GreenValueChanged);
        BlueField.onValueChanged.AddListener(BlueValueChanged);
    }

    private void OnDestroy()
    {
        RedField.onValueChanged.RemoveListener(RedValueChanged);
        GreenField.onValueChanged.RemoveListener(GreenValueChanged);
        BlueField.onValueChanged.RemoveListener(BlueValueChanged);
    }

    /// <summary> Get a hex converted representation of the color </summary>
    /// <returns> Hexadecimal string form of RGB values.</returns>
    public string GetColorString()
    {
        return ColorUtility.ToHtmlStringRGB(ColorShower.color);
    }

    public Color GetColor() 
    {
        return ColorShower.color;
    }

    public void SetColor(string colorString)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#"+colorString, out color);
        ColorShower.color = color;

        Color32 col32 = color;
        RedField.text = col32.r.ToString();
        GreenField.text = col32.g.ToString();
        BlueField.text = col32.b.ToString();
    }

    private void RedValueChanged(string value)
    {
        float red = ParseColorValue(value);
        ColorShower.color = new Color(red, ColorShower.color.g, ColorShower.color.b, ColorShower.color.a);
        OnColorChange?.Invoke(ColorShower.color);
    }

    private void GreenValueChanged(string value)
    {
        float green = ParseColorValue(value);
        ColorShower.color = new Color(ColorShower.color.r, green, ColorShower.color.b, ColorShower.color.a);
        OnColorChange?.Invoke(ColorShower.color);
    }

    private void BlueValueChanged(string value)
    {
        float blue = ParseColorValue(value);
        ColorShower.color = new Color(ColorShower.color.r, ColorShower.color.g, blue, ColorShower.color.a);
        OnColorChange?.Invoke(ColorShower.color);
    }


    /// <summary> Parse a string into a color value. Assumes the InputField where the color is taken only allows integers. </summary>
    /// <param name="value"> Integer value as a string. </param>
    /// <returns> Float representation of a color between the range 0 to 255.</returns>
    private float ParseColorValue(string value)
    {
        if (value.Length == 0)
            return 0;
        else
            return Mathf.Clamp(int.Parse(value), 0, 255) / 255f;
    }
}
