using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class PersonalisationPresenter : AModePresenter
{
    [SerializeField] protected Canvas BaseCanvas;

    [Header("Bloom")]
    [SerializeField] protected Toggle Bloom;
    [SerializeField] protected SliderField BIntensity;
    [SerializeField] protected SliderField BScatter;
    [SerializeField] protected SliderField BTintRed;
    [SerializeField] protected SliderField BTintGreen;
    [SerializeField] protected SliderField BTintBlue;

    [Header("Vignette")]
    [SerializeField] protected Toggle Vignette;
    [SerializeField] protected SliderField VIntensity;
    [SerializeField] protected SliderField VSmoothness;
    [SerializeField] protected Toggle VRounded;
    [SerializeField] protected SliderField VColorRed;
    [SerializeField] protected SliderField VColorGreen;
    [SerializeField] protected SliderField VColorBlue;
    [SerializeField] protected SliderField VCenterX;
    [SerializeField] protected SliderField VCenterY;

    [Header("Film Grain")]
    [SerializeField] protected Toggle FilmGrain;
    [SerializeField] protected SliderField FGType;
    [SerializeField] protected SliderField FGIntensity;
    [SerializeField] protected SliderField FGResponse;

    [Header("White Balance")]
    [SerializeField] protected Toggle WhiteBalance;
    [SerializeField] protected SliderField WBTemperature;
    [SerializeField] protected SliderField WBTint;

    [Header("Lens Distortion")]
    [SerializeField] protected Toggle LensDistortion;
    [SerializeField] protected SliderField LDIntensity;

    [Header("Panini Projection")]
    [SerializeField] protected Toggle PaniniProjection;
    [SerializeField] protected SliderField PPDistance;

    [Header("Chromatic Aberration")]
    [SerializeField] protected Toggle ChromaticAberration;
    [SerializeField] protected SliderField CAIntensity;

    [Header("Buttons")]
    [SerializeField] protected Button SaveButton;
    [SerializeField] protected Button DefaultButton;

    private PersonalisationManager _manager;
    

    private void Awake()
    {
        BaseCanvas.worldCamera = Camera.main;
        BaseCanvas.gameObject.SetActive(false);
    }
    
    private void Start()
    {
        _manager = FindObjectOfType<PersonalisationManager>();
        Initialize();

        NotifyClassReady(this);
    }

    private void OnDestroy()
    {
        if (BaseCanvas.gameObject.activeSelf)
            ChangeListeners(false);
    }

    public override string GetName()
    {
        return "Personalize";
    }

    public override string GetDescription()
    {
        return "Customize visual presentation of this program.";
    }

    public override void Open()
    {
        BaseCanvas.gameObject.SetActive(true);
        ChangeListeners(true);
    }

    public override void Close()
    {
        BaseCanvas.gameObject.SetActive(false);
        ChangeListeners(false);
    }

    private void Initialize()
    {
        BloomToggle(_manager.Bloom.active);
        Bloom.SetIsOnWithoutNotify(_manager.Bloom.active);
        BIntensity.Initialize("Intensity", _manager.Bloom.intensity.value, _manager.Bloom.intensity.min, 20);
        BScatter.Initialize("Scatter", _manager.Bloom.scatter.value, _manager.Bloom.scatter.min, _manager.Bloom.scatter.max);
        BTintRed.Initialize("Tint - Red", _manager.Bloom.tint.value.r, 0, 1);
        BTintGreen.Initialize("Tint - Green", _manager.Bloom.tint.value.g, 0, 1);
        BTintBlue.Initialize("Tint - Blue", _manager.Bloom.tint.value.b, 0, 1);

        VignetteToggle(_manager.Vignette.active);
        Vignette.SetIsOnWithoutNotify(_manager.Vignette.active);
        VIntensity.Initialize("Intensity", _manager.Vignette.intensity.value, _manager.Vignette.intensity.min, _manager.Vignette.intensity.max);
        VSmoothness.Initialize("Smoothness", _manager.Vignette.smoothness.value, _manager.Vignette.smoothness.min, _manager.Vignette.smoothness.max);
        VRounded.SetIsOnWithoutNotify(_manager.Vignette.rounded.value);
        VColorRed.Initialize("Color - Red", _manager.Vignette.color.value.r, 0.05f, 1);
        VColorGreen.Initialize("Color - Green", _manager.Vignette.color.value.g, 0.05f, 1);
        VColorBlue.Initialize("Color - Blue", _manager.Vignette.color.value.b, 0.05f, 1);
        VCenterX.Initialize("Center - X", _manager.Vignette.center.value.x, -0.25f, 1.25f);
        VCenterY.Initialize("Center - Y", _manager.Vignette.center.value.y, -0.25f, 1.25f);

        FilmGrainToggle(_manager.FilmGrain.active);
        FilmGrain.SetIsOnWithoutNotify(_manager.FilmGrain.active);
        FGType.Initialize("Type", _manager.FilmGrainType, 0, 2);
        FGIntensity.Initialize("Intensity", _manager.FilmGrain.intensity.value, _manager.FilmGrain.intensity.min, _manager.FilmGrain.intensity.max);
        FGResponse.Initialize("Response", _manager.FilmGrain.response.value, _manager.FilmGrain.response.min, _manager.FilmGrain.response.max);

        WhiteBalanceToggle(_manager.WhiteBalance.active);
        WhiteBalance.SetIsOnWithoutNotify(_manager.WhiteBalance.active);
        WBTemperature.Initialize("Temperature", _manager.WhiteBalance.temperature.value, _manager.WhiteBalance.temperature.min, _manager.WhiteBalance.temperature.max);
        WBTint.Initialize("Tint", _manager.WhiteBalance.tint.value, _manager.WhiteBalance.tint.min, _manager.WhiteBalance.tint.max);

        LensDistortionToggle(_manager.LensDistortion.active);
        LensDistortion.SetIsOnWithoutNotify(_manager.LensDistortion.active);
        LDIntensity.Initialize("Intensity", _manager.LensDistortion.intensity.value, -0.1f, 0.1f);

        PaniniProjectionToggle(_manager.PaniniProjection.active);
        PaniniProjection.SetIsOnWithoutNotify(_manager.PaniniProjection.active);
        PPDistance.Initialize("Distance", _manager.PaniniProjection.distance.value, 0, 0.1f);

        ChromaticAberrationToggle(_manager.ChromaticAberration.active);
        ChromaticAberration.SetIsOnWithoutNotify(_manager.ChromaticAberration.active);
        CAIntensity.Initialize("Intensity", _manager.ChromaticAberration.intensity.value, _manager.ChromaticAberration.intensity.min, 0.2f);

        //TODO: There is still more
    }

    private void ChangeListeners(bool state)
    {
        if (state)
        {
            Bloom.onValueChanged.AddListener(BloomToggle);
            BIntensity.OnSliderChange   += BloomIntensity;
            BScatter.OnSliderChange     += BloomScatter;
            BTintRed.OnSliderChange     += BloomTintRed;
            BTintGreen.OnSliderChange   += BloomTintGreen;
            BTintBlue.OnSliderChange    += BloomTintBlue;

            Vignette.onValueChanged.AddListener(VignetteToggle);
            VIntensity.OnSliderChange   += VignetteIntensity;
            VSmoothness.OnSliderChange  += VignetteSmoothness;
            VRounded.onValueChanged.AddListener(VignetteRounded);
            VColorRed.OnSliderChange    += VignetteColorRed;
            VColorGreen.OnSliderChange  += VignetteColorGreen;
            VColorBlue.OnSliderChange   += VignetteColorBlue;
            VCenterX.OnSliderChange     += VignetteCenterX;
            VCenterY.OnSliderChange     += VignetteCenterY;

            FilmGrain.onValueChanged.AddListener(FilmGrainToggle);
            FGType.OnSliderChange       += FilmGrainType;
            FGIntensity.OnSliderChange  += FilmGrainIntensity;
            FGResponse.OnSliderChange   += FilmGrainResponse;

            WhiteBalance.onValueChanged.AddListener(WhiteBalanceToggle);
            WBTemperature.OnSliderChange    += WhiteBalanceTemperature;
            WBTint.OnSliderChange           += WhiteBalanceTint;

            LensDistortion.onValueChanged.AddListener(LensDistortionToggle);
            LDIntensity.OnSliderChange += LensDistortionIntensity;

            PaniniProjection.onValueChanged.AddListener(PaniniProjectionToggle);
            PPDistance.OnSliderChange += PaniniProjectionDistance;

            ChromaticAberration.onValueChanged.AddListener(ChromaticAberrationToggle);
            CAIntensity.OnSliderChange += ChromaticAberrationIntensity;

            SaveButton.onClick.AddListener(SaveButtonClicked);
            DefaultButton.onClick.AddListener(DefaultButtonClicked);
        }

        else
        {
            Bloom.onValueChanged.RemoveListener(BloomToggle);
            BIntensity.OnSliderChange   -= BloomIntensity;
            BScatter.OnSliderChange     -= BloomScatter;
            BTintRed.OnSliderChange     -= BloomTintRed;
            BTintGreen.OnSliderChange   -= BloomTintGreen;
            BTintBlue.OnSliderChange    -= BloomTintBlue;

            Vignette.onValueChanged.RemoveListener(VignetteToggle);
            VIntensity.OnSliderChange   -= VignetteIntensity;
            VSmoothness.OnSliderChange  -= VignetteSmoothness;
            VRounded.onValueChanged.RemoveListener(VignetteRounded);
            VColorRed.OnSliderChange    -= VignetteColorRed;
            VColorGreen.OnSliderChange  -= VignetteColorGreen;
            VColorBlue.OnSliderChange   -= VignetteColorBlue;
            VCenterX.OnSliderChange     -= VignetteCenterX;
            VCenterY.OnSliderChange     -= VignetteCenterY;

            FilmGrain.onValueChanged.RemoveListener(FilmGrainToggle);
            FGType.OnSliderChange       -= FilmGrainType;
            FGIntensity.OnSliderChange  -= FilmGrainIntensity;
            FGResponse.OnSliderChange   -= FilmGrainResponse;

            WhiteBalance.onValueChanged.RemoveListener(WhiteBalanceToggle);
            WBTemperature.OnSliderChange    -= WhiteBalanceTemperature;
            WBTint.OnSliderChange           -= WhiteBalanceTint;

            LensDistortion.onValueChanged.RemoveListener(LensDistortionToggle);
            LDIntensity.OnSliderChange -= LensDistortionIntensity;

            PaniniProjection.onValueChanged.RemoveListener(PaniniProjectionToggle);
            PPDistance.OnSliderChange -= PaniniProjectionDistance;

            ChromaticAberration.onValueChanged.RemoveListener(ChromaticAberrationToggle);
            CAIntensity.OnSliderChange -= ChromaticAberrationIntensity;

            SaveButton.onClick.RemoveListener(SaveButtonClicked);
            DefaultButton.onClick.RemoveListener(DefaultButtonClicked);
        }
    }

    private void BloomToggle(bool value)
    {
        _manager.Bloom.active = value;
        BIntensity.gameObject.SetActive(value);
        BScatter.gameObject.SetActive(value);
        BTintRed.gameObject.SetActive(value);
        BTintGreen.gameObject.SetActive(value);
        BTintBlue.gameObject.SetActive(value);
    }
    private void BloomIntensity(float value)
        => _manager.Bloom.intensity.value = value;
    private void BloomScatter(float value)
        => _manager.Bloom.scatter.value = value;
    private void BloomTintRed(float value)
        => _manager.Bloom.tint.value = new Color(value, _manager.Bloom.tint.value.g, _manager.Bloom.tint.value.b);
    private void BloomTintGreen(float value)
        => _manager.Bloom.tint.value = new Color(_manager.Bloom.tint.value.r, value, _manager.Bloom.tint.value.b);
    private void BloomTintBlue(float value)
        => _manager.Bloom.tint.value = new Color(_manager.Bloom.tint.value.r, _manager.Bloom.tint.value.g, value);

    private void VignetteToggle(bool value)
    {
        _manager.Vignette.active = value;
        VIntensity.gameObject.SetActive(value);
        VSmoothness.gameObject.SetActive(value);
        VRounded.gameObject.SetActive(value);
        VColorRed.gameObject.SetActive(value);
        VColorGreen.gameObject.SetActive(value);
        VColorBlue.gameObject.SetActive(value);
        VCenterX.gameObject.SetActive(value);
        VCenterY.gameObject.SetActive(value);
    }
    private void VignetteIntensity(float value)
        => _manager.Vignette.intensity.value = value;
    private void VignetteSmoothness(float value)
        => _manager.Vignette.smoothness.value = value;
    private void VignetteRounded(bool value)
        => _manager.Vignette.rounded.value = value;
    private void VignetteColorRed(float value)
        => _manager.Vignette.color.value = new Color(value, _manager.Vignette.color.value.g, _manager.Vignette.color.value.b);
    private void VignetteColorGreen(float value)
        => _manager.Vignette.color.value = new Color(_manager.Vignette.color.value.r, value, _manager.Vignette.color.value.b);
    private void VignetteColorBlue(float value)
        => _manager.Vignette.color.value = new Color(_manager.Vignette.color.value.r, _manager.Vignette.color.value.g, value);
    private void VignetteCenterX(float value)
        => _manager.Vignette.center.value = new Vector2(value, _manager.Vignette.center.value.y);
    private void VignetteCenterY(float value)
        => _manager.Vignette.center.value = new Vector2(_manager.Vignette.center.value.x, value);

    private void FilmGrainToggle(bool value)
    {
        _manager.FilmGrain.active = value;
        FGType.gameObject.SetActive(value);
        FGIntensity.gameObject.SetActive(value);
        FGResponse.gameObject.SetActive(value);
    }
    private void FilmGrainType(float value)
    {
        _manager.FilmGrainType = Mathf.RoundToInt(value);
        _manager.FilmGrain.type.value = _manager.FilmGrainType switch
        {
            0 => FilmGrainLookup.Thin1,
            1 => FilmGrainLookup.Medium1,
            _ => FilmGrainLookup.Large01
        };
    }
        
    private void FilmGrainIntensity(float value)
        => _manager.FilmGrain.intensity.value = value;
    private void FilmGrainResponse(float value)
        => _manager.FilmGrain.response.value = value;

    private void WhiteBalanceToggle(bool value)
    {
        _manager.WhiteBalance.active = value;
        WBTemperature.gameObject.SetActive(value);
        WBTint.gameObject.SetActive(value);
    }
    private void WhiteBalanceTemperature(float value)
        => _manager.WhiteBalance.temperature.value = value;
    private void WhiteBalanceTint(float value)
        => _manager.WhiteBalance.tint.value = value;

    private void LensDistortionToggle(bool value)
    {
        _manager.LensDistortion.active = value;
        LDIntensity.gameObject.SetActive(value);
    }  
    private void LensDistortionIntensity(float value)
        => _manager.LensDistortion.intensity.value = value;

    private void PaniniProjectionToggle(bool value)
    {
        _manager.PaniniProjection.active = value;
        PPDistance.gameObject.SetActive(value);
    }
    private void PaniniProjectionDistance(float value)
        => _manager.PaniniProjection.distance.value = value;

    private void ChromaticAberrationToggle(bool value)
    {
        _manager.ChromaticAberration.active = value;
        CAIntensity.gameObject.SetActive(value);
    }
    private void ChromaticAberrationIntensity(float value)
        => _manager.ChromaticAberration.intensity.value = value;

    private void SaveButtonClicked()
        => _manager.SavePersonalisationData();

    private void DefaultButtonClicked()
    {
        BloomToggle(_manager.BloomDefaultActive);
        BloomIntensity(_manager.BloomDefaultIntensity);
        BloomScatter(_manager.BloomDefaultScatter);
        BloomTintRed(_manager.BloomDefaultTint.r);
        BloomTintGreen(_manager.BloomDefaultTint.g);
        BloomTintBlue(_manager.BloomDefaultTint.b);

        VignetteToggle(_manager.VignetteDefaultActive);
        VignetteIntensity(_manager.VignetteDefaultIntensity);
        VignetteSmoothness(_manager.VignetteDefaultSmoothness);
        VignetteRounded(_manager.VignetteDefaultRounded);
        VignetteColorRed(_manager.VignetteDefaultColor.r);
        VignetteColorGreen(_manager.VignetteDefaultColor.g);
        VignetteColorBlue(_manager.VignetteDefaultColor.b);
        VignetteCenterX(_manager.VignetteDefaultCenter.x);
        VignetteCenterY(_manager.VignetteDefaultCenter.y);

        FilmGrainToggle(_manager.GrainDefaultActive);
        FilmGrainType(_manager.GrainDefaultType);
        FilmGrainIntensity(_manager.GrainDefaultIntensity);
        FilmGrainResponse(_manager.GrainDefaultResponse);

        WhiteBalanceToggle(_manager.WhiteDefaultActive);
        WhiteBalanceTemperature(_manager.WhiteDefaultTemperature);
        WhiteBalanceTint(_manager.WhiteDefaultTint);

        LensDistortionToggle(_manager.LensDefaultActive);
        LensDistortionIntensity(_manager.LensDefaultIntensity);

        PaniniProjectionToggle(_manager.PaniniDefaultActive);
        PaniniProjectionDistance(_manager.PaniniDefaultDistance);

        ChromaticAberrationToggle(_manager.AberrationDefaultActive);
        ChromaticAberrationIntensity(_manager.AberrationDefaultIntensity);

        Initialize();
    }
}
