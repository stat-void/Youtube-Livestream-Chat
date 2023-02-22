using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using TMPro;

// Yes, I know, there are way too many serialized fields.
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
    [SerializeField] protected Button ApplyColorsButton;
    [SerializeField] protected Button SaveButton;
    [SerializeField] protected Button DefaultButton;

    [Header("Color Fields")]
    [SerializeField] protected ColorField PrimaryField;
    [SerializeField] protected ColorField SecondaryField;
    [SerializeField] protected ColorField ObjectBGField;
    [SerializeField] protected ColorField BackgroundField;
    [SerializeField] protected ColorField RegularUserField;
    [SerializeField] protected ColorField RegularMessageField;
    [SerializeField] protected ColorField MemberField;
    [SerializeField] protected ColorField ModeratorField;
    [SerializeField] protected ColorField OwnerField;
    [SerializeField] protected ColorField HighlightUserField;
    [SerializeField] protected ColorField HighlightMessageField;
    [SerializeField] protected ColorField Super1UserField;
    [SerializeField] protected ColorField Super1MessageField;
    [SerializeField] protected ColorField Super2UserField;
    [SerializeField] protected ColorField Super2MessageField;
    [SerializeField] protected ColorField Super3UserField;
    [SerializeField] protected ColorField Super3MessageField;
    [SerializeField] protected ColorField Super4UserField;
    [SerializeField] protected ColorField Super4MessageField;
    [SerializeField] protected ColorField Super5UserField;
    [SerializeField] protected ColorField Super5MessageField;
    [SerializeField] protected ColorField Super6UserField;
    [SerializeField] protected ColorField Super6MessageField;
    [SerializeField] protected ColorField Super7UserField;
    [SerializeField] protected ColorField Super7MessageField;

    [Header("Color Editing Fields")]
    [SerializeField] protected Image DemoBox;
    [SerializeField] protected Image DemoBackground;
    [SerializeField] protected TMP_Text DemoText;
    [SerializeField] protected Image DemoIFBG;
    [SerializeField] protected TMP_InputField DemoIFMain;
    [SerializeField] protected Image DemoButtonBox;
    [SerializeField] protected Image DemoButtonBG;
    [SerializeField] protected TMP_Text DemoButtonText;
    [SerializeField] protected TMP_Text DemoUserText;
    [SerializeField] protected TMP_Text DemoMSGText;
    [SerializeField] protected TMP_Text DemoMemberUSRText;
    [SerializeField] protected TMP_Text DemoMemberMSGText;
    [SerializeField] protected TMP_Text DemoModUSRText;
    [SerializeField] protected TMP_Text DemoModMSGText;
    [SerializeField] protected TMP_Text DemoOwnerUSRText;
    [SerializeField] protected TMP_Text DemoOwnerMSGText;
    [SerializeField] protected TMP_Text DemoHighlightUSRText;
    [SerializeField] protected TMP_Text DemoHighlightMSGText;
    [SerializeField] protected TMP_Text DemoSC1USRText;
    [SerializeField] protected TMP_Text DemoSC1MSGText;
    [SerializeField] protected TMP_Text DemoSC2USRText;
    [SerializeField] protected TMP_Text DemoSC2MSGText;
    [SerializeField] protected TMP_Text DemoSC3USRText;
    [SerializeField] protected TMP_Text DemoSC3MSGText;
    [SerializeField] protected TMP_Text DemoSC4USRText;
    [SerializeField] protected TMP_Text DemoSC4MSGText;
    [SerializeField] protected TMP_Text DemoSC5USRText;
    [SerializeField] protected TMP_Text DemoSC5MSGText;
    [SerializeField] protected TMP_Text DemoSC6USRText;
    [SerializeField] protected TMP_Text DemoSC6MSGText;
    [SerializeField] protected TMP_Text DemoSC7USRText;
    [SerializeField] protected TMP_Text DemoSC7MSGText;

    private PersonalisationManager _manager;

    public List<ColorField> ColorFields
    {
        get
        {
            return new()
            {
                PrimaryField, SecondaryField,
                ObjectBGField, BackgroundField,
                RegularUserField, RegularMessageField,
                MemberField,
                ModeratorField,
                OwnerField,
                HighlightUserField, HighlightMessageField,
                Super1UserField, Super1MessageField,
                Super2UserField, Super2MessageField,
                Super3UserField, Super3MessageField,
                Super4UserField, Super4MessageField,
                Super5UserField, Super5MessageField,
                Super6UserField, Super6MessageField,
                Super7UserField, Super7MessageField
            };
        }
    }

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

        PrimaryField            .SetColor(ColorSettings.MainColor);
        SecondaryField          .SetColor(ColorSettings.SecondaryColor);
        ObjectBGField           .SetColor(ColorSettings.ObjectBackgroundColor);
        BackgroundField         .SetColor(ColorSettings.BackgroundColor);
        RegularUserField        .SetColor(ColorSettings.RegularUserColor);
        RegularMessageField     .SetColor(ColorSettings.RegularMessageColor);
        MemberField             .SetColor(ColorSettings.MemberColor);
        ModeratorField          .SetColor(ColorSettings.ModeratorColor);
        OwnerField              .SetColor(ColorSettings.OwnerColor);
        HighlightUserField      .SetColor(ColorSettings.HighlightUserColor);
        HighlightMessageField   .SetColor(ColorSettings.HighlightMessageColor);
        Super1UserField         .SetColor(ColorSettings.UserSuperColors[0]);
        Super1MessageField      .SetColor(ColorSettings.MessageSuperColors[0]);
        Super2UserField         .SetColor(ColorSettings.UserSuperColors[1]);
        Super2MessageField      .SetColor(ColorSettings.MessageSuperColors[1]);
        Super3UserField         .SetColor(ColorSettings.UserSuperColors[2]);
        Super3MessageField      .SetColor(ColorSettings.MessageSuperColors[2]);
        Super4UserField         .SetColor(ColorSettings.UserSuperColors[3]);
        Super4MessageField      .SetColor(ColorSettings.MessageSuperColors[3]);
        Super5UserField         .SetColor(ColorSettings.UserSuperColors[4]);
        Super5MessageField      .SetColor(ColorSettings.MessageSuperColors[4]);
        Super6UserField         .SetColor(ColorSettings.UserSuperColors[5]);
        Super6MessageField      .SetColor(ColorSettings.MessageSuperColors[5]);
        Super7UserField         .SetColor(ColorSettings.UserSuperColors[6]);
        Super7MessageField      .SetColor(ColorSettings.MessageSuperColors[6]);

        PrimaryFieldChanged(PrimaryField.GetColor());
        SecondaryFieldChanged(SecondaryField.GetColor());
        ObjectBGFieldChanged(ObjectBGField.GetColor());
        BackgroundFieldChanged(BackgroundField.GetColor());
        RegularUserFieldChanged(RegularUserField.GetColor());
        RegularMessageFieldChanged(RegularMessageField.GetColor());
        MemberFieldChanged(MemberField.GetColor());
        ModeratorFieldChanged(ModeratorField.GetColor());
        OwnerFieldChanged(OwnerField.GetColor());
        HighlightUserFieldChanged(HighlightUserField.GetColor());
        HighlightMessageFieldChanged(HighlightMessageField.GetColor());
        Super1USRFieldChanged(Super1UserField.GetColor());
        Super1MSGFieldChanged(Super1MessageField.GetColor());
        Super2USRFieldChanged(Super2UserField.GetColor());
        Super2MSGFieldChanged(Super2MessageField.GetColor());
        Super3USRFieldChanged(Super3UserField.GetColor());
        Super3MSGFieldChanged(Super3MessageField.GetColor());
        Super4USRFieldChanged(Super4UserField.GetColor());
        Super4MSGFieldChanged(Super4MessageField.GetColor());
        Super5USRFieldChanged(Super5UserField.GetColor());
        Super5MSGFieldChanged(Super5MessageField.GetColor());
        Super6USRFieldChanged(Super6UserField.GetColor());
        Super6MSGFieldChanged(Super6MessageField.GetColor());
        Super7USRFieldChanged(Super7UserField.GetColor());
        Super7MSGFieldChanged(Super7MessageField.GetColor());
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

            PrimaryField.OnColorChange          += PrimaryFieldChanged;
            SecondaryField.OnColorChange        += SecondaryFieldChanged;
            ObjectBGField.OnColorChange         += ObjectBGFieldChanged;
            BackgroundField.OnColorChange       += BackgroundFieldChanged;
            RegularUserField.OnColorChange      += RegularUserFieldChanged;
            RegularMessageField.OnColorChange   += RegularMessageFieldChanged;
            MemberField.OnColorChange           += MemberFieldChanged;
            ModeratorField.OnColorChange        += ModeratorFieldChanged;
            OwnerField.OnColorChange            += OwnerFieldChanged;
            HighlightUserField.OnColorChange    += HighlightUserFieldChanged;
            HighlightMessageField.OnColorChange += HighlightMessageFieldChanged;
            Super1UserField.OnColorChange       += Super1USRFieldChanged;
            Super1MessageField.OnColorChange    += Super1MSGFieldChanged;
            Super2UserField.OnColorChange       += Super2USRFieldChanged;
            Super2MessageField.OnColorChange    += Super2MSGFieldChanged;
            Super3UserField.OnColorChange       += Super3USRFieldChanged;
            Super3MessageField.OnColorChange    += Super3MSGFieldChanged;
            Super4UserField.OnColorChange       += Super4USRFieldChanged;
            Super4MessageField.OnColorChange    += Super4MSGFieldChanged;
            Super5UserField.OnColorChange       += Super5USRFieldChanged;
            Super5MessageField.OnColorChange    += Super5MSGFieldChanged;
            Super6UserField.OnColorChange       += Super6USRFieldChanged;
            Super6MessageField.OnColorChange    += Super6MSGFieldChanged;
            Super7UserField.OnColorChange       += Super7USRFieldChanged;
            Super7MessageField.OnColorChange    += Super7MSGFieldChanged;

            ApplyColorsButton.onClick.AddListener(ApplyButtonClicked);
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

            PrimaryField.OnColorChange          -= PrimaryFieldChanged;
            SecondaryField.OnColorChange        -= SecondaryFieldChanged;
            ObjectBGField.OnColorChange         -= ObjectBGFieldChanged;
            BackgroundField.OnColorChange       -= BackgroundFieldChanged;
            RegularUserField.OnColorChange      -= RegularUserFieldChanged;
            RegularMessageField.OnColorChange   -= RegularMessageFieldChanged;
            MemberField.OnColorChange           -= MemberFieldChanged;
            ModeratorField.OnColorChange        -= ModeratorFieldChanged;
            OwnerField.OnColorChange            -= OwnerFieldChanged;
            HighlightUserField.OnColorChange    -= HighlightUserFieldChanged;
            HighlightMessageField.OnColorChange -= HighlightMessageFieldChanged;
            Super1UserField.OnColorChange       -= Super1USRFieldChanged;
            Super1MessageField.OnColorChange    -= Super1MSGFieldChanged;
            Super2UserField.OnColorChange       -= Super2USRFieldChanged;
            Super2MessageField.OnColorChange    -= Super2MSGFieldChanged;
            Super3UserField.OnColorChange       -= Super3USRFieldChanged;
            Super3MessageField.OnColorChange    -= Super3MSGFieldChanged;
            Super4UserField.OnColorChange       -= Super4USRFieldChanged;
            Super4MessageField.OnColorChange    -= Super4MSGFieldChanged;
            Super5UserField.OnColorChange       -= Super5USRFieldChanged;
            Super5MessageField.OnColorChange    -= Super5MSGFieldChanged;
            Super6UserField.OnColorChange       -= Super6USRFieldChanged;
            Super6MessageField.OnColorChange    -= Super6MSGFieldChanged;
            Super7UserField.OnColorChange       -= Super7USRFieldChanged;
            Super7MessageField.OnColorChange    -= Super7MSGFieldChanged;

            ApplyColorsButton.onClick.RemoveListener(ApplyButtonClicked);
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

    private void PrimaryFieldChanged(Color color)
    {
        DemoBox.color = color;
        DemoText.color = color;
        DemoButtonBox.color = color;
        DemoButtonText.color = color;
        //TODO: DemoIFMain.colors.
    }
    private void SecondaryFieldChanged(Color color)
        => DemoIFMain.placeholder.color = color;
    private void ObjectBGFieldChanged(Color color)
    {
        DemoIFBG.color = color;
        DemoButtonBG.color = color;
    }
    private void BackgroundFieldChanged(Color color)
        => DemoBackground.color = color;
    private void RegularUserFieldChanged(Color color)
        => DemoUserText.color = color;
    private void RegularMessageFieldChanged(Color color)
    {
        DemoMSGText.color = color;
        DemoMemberMSGText.color = color;
        DemoModMSGText.color = color;
        DemoOwnerMSGText.color = color;
    }
    private void MemberFieldChanged(Color color)
        => DemoMemberUSRText.color = color;
    private void ModeratorFieldChanged(Color color)
        => DemoModUSRText.color = color;
    private void OwnerFieldChanged(Color color)
        => DemoOwnerUSRText.color = color;
    private void HighlightUserFieldChanged(Color color)
        => DemoHighlightUSRText.color = color;
    private void HighlightMessageFieldChanged(Color color)
        => DemoHighlightMSGText.color = color;
    private void Super1USRFieldChanged(Color color)
        => DemoSC1USRText.color = color;
    private void Super1MSGFieldChanged(Color color)
        => DemoSC1MSGText.color = color;
    private void Super2USRFieldChanged(Color color)
        => DemoSC2USRText.color = color;
    private void Super2MSGFieldChanged(Color color)
        => DemoSC2MSGText.color = color;
    private void Super3USRFieldChanged(Color color)
        => DemoSC3USRText.color = color;
    private void Super3MSGFieldChanged(Color color)
        => DemoSC3MSGText.color = color;
    private void Super4USRFieldChanged(Color color)
        => DemoSC4USRText.color = color;
    private void Super4MSGFieldChanged(Color color)
        => DemoSC4MSGText.color = color;
    private void Super5USRFieldChanged(Color color)
        => DemoSC5USRText.color = color;
    private void Super5MSGFieldChanged(Color color)
        => DemoSC5MSGText.color = color;
    private void Super6USRFieldChanged(Color color)
        => DemoSC6USRText.color = color;
    private void Super6MSGFieldChanged(Color color)
        => DemoSC6MSGText.color = color;
    private void Super7USRFieldChanged(Color color)
        => DemoSC7USRText.color = color;
    private void Super7MSGFieldChanged(Color color)
        => DemoSC7MSGText.color = color;

    private void ApplyButtonClicked()
        => _manager.Colors.SetColors(ColorFields);

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

        _manager.Colors.DefaultColors();

        Initialize();
    }
}
