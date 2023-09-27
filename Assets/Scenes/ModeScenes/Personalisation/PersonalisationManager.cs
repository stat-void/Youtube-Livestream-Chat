using SimpleJSON;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Void.YoutubeAPI;

/// <summary>
/// This script should recover all personalisation settings from the JSON file, if they exist, and let all other components know about these settings.
/// </summary>
public class PersonalisationManager : MonoBehaviour
{
    [SerializeField] protected Volume Volume;
    [SerializeField] protected ColorSettings ColorSettings;

    private Bloom _bloom;
    private Vignette _vignette;
    private FilmGrain _filmGrain;
    private WhiteBalance _whiteBalance;
    private LensDistortion _lensDistortion;
    private PaniniProjection _paniniProjection;
    private ChromaticAberration _chromaticAberration;

    public readonly bool    BloomDefaultActive = true;
    public readonly float   BloomDefaultIntensity = 1.5f;
    public readonly float   BloomDefaultScatter = 0.5f;
    public readonly Color   BloomDefaultTint = Color.white;

    public readonly bool    VignetteDefaultActive = false;
    public readonly Color   VignetteDefaultColor = Color.gray;
    public readonly Vector2 VignetteDefaultCenter = new(0.5f, 0.5f);
    public readonly float   VignetteDefaultIntensity = 0;
    public readonly float   VignetteDefaultSmoothness = 1;
    public readonly bool    VignetteDefaultRounded = true;

    public readonly bool  GrainDefaultActive = false;
    public readonly int   GrainDefaultType = 0;
    public readonly float GrainDefaultIntensity = 1;
    public readonly float GrainDefaultResponse = 0.8f;

    public readonly bool WhiteDefaultActive = false;
    public readonly float WhiteDefaultTemperature = 0;
    public readonly float WhiteDefaultTint = 0;

    public readonly bool LensDefaultActive = false;
    public readonly float LensDefaultIntensity = 0;

    public readonly bool PaniniDefaultActive = false;
    public readonly float PaniniDefaultDistance = 0; // 0 to 0.1f

    public readonly bool  AberrationDefaultActive = false;
    public readonly float AberrationDefaultIntensity = 0;

    public int FilmGrainType = -1;

    public Bloom Bloom { get => _bloom; }
    public Vignette Vignette { get => _vignette; }
    public FilmGrain FilmGrain { get => _filmGrain; }
    public WhiteBalance WhiteBalance { get => _whiteBalance; }
    public LensDistortion LensDistortion { get => _lensDistortion; }
    public PaniniProjection PaniniProjection { get => _paniniProjection; }
    public ChromaticAberration ChromaticAberration { get => _chromaticAberration; }
    public ColorSettings Colors { get => ColorSettings; }


    private void Awake()
    {
        Initialize();
    }


    /// <summary>
    /// Go through all the open settings, check if their data exists and autoset them.
    /// </summary>
    private void Initialize()
    {
        JSONNode data = YoutubeSaveData.GetData();

        // Bloom
        if (Volume.profile.TryGet(out _bloom)) 
        {
            JSONNode bloNode = data["personalisation"]["bloom"];

            _bloom.active = !string.IsNullOrEmpty(bloNode["active"]) 
                ? bloNode["active"].AsBool : BloomDefaultActive;

            _bloom.intensity.value = !string.IsNullOrEmpty(bloNode["intensity"])
                ? Mathf.Clamp(bloNode["intensity"].AsFloat, 0, 20) : BloomDefaultIntensity;

            _bloom.scatter.value = !string.IsNullOrEmpty(bloNode["scatter"])
                ? Mathf.Clamp01(bloNode["scatter"].AsFloat) : BloomDefaultScatter;

            _bloom.tint.value = new Color (
                !string.IsNullOrEmpty(bloNode["tintR"]) ?
                    Mathf.Clamp01(bloNode["tintR"].AsFloat) : BloomDefaultTint.r,
                !string.IsNullOrEmpty(bloNode["tintG"]) ?
                    Mathf.Clamp01(bloNode["tintG"].AsFloat) : BloomDefaultTint.g,
                !string.IsNullOrEmpty(bloNode["tintB"]) ?
                    Mathf.Clamp01(bloNode["tintB"].AsFloat) : BloomDefaultTint.b );
        }

        // Vignette
        if (Volume.profile.TryGet(out _vignette)) 
        {
            JSONNode vigNode = data["personalisation"]["vignette"];

            _vignette.active = !string.IsNullOrEmpty(vigNode["active"])
                ? vigNode["active"].AsBool : VignetteDefaultActive;

            _vignette.intensity.value = !string.IsNullOrEmpty(vigNode["intensity"])
                ? Mathf.Clamp01(vigNode["intensity"].AsFloat) : VignetteDefaultIntensity;

            _vignette.smoothness.value = !string.IsNullOrEmpty(vigNode["smoothness"])
                ? Mathf.Clamp(vigNode["smoothness"].AsFloat, 0.01f, 1) : VignetteDefaultSmoothness;

            _vignette.rounded.value = !string.IsNullOrEmpty(vigNode["rounded"])
                ? vigNode["rounded"].AsBool : VignetteDefaultRounded;

            _vignette.color.value = new Color (
                !string.IsNullOrEmpty(vigNode["colorR"]) ?
                    Mathf.Clamp(vigNode["colorR"].AsFloat, 0.05f, 1) : VignetteDefaultColor.r,
                !string.IsNullOrEmpty(vigNode["colorG"]) ?
                    Mathf.Clamp(vigNode["colorG"].AsFloat, 0.05f, 1) : VignetteDefaultColor.g,
                !string.IsNullOrEmpty(vigNode["colorB"]) ?
                    Mathf.Clamp(vigNode["colorB"].AsFloat, 0.05f, 1) : VignetteDefaultColor.b);

            _vignette.center.value = new Vector2(
                !string.IsNullOrEmpty(vigNode["centerX"]) ?
                    Mathf.Clamp01(vigNode["centerX"].AsFloat) : VignetteDefaultCenter.x,
                !string.IsNullOrEmpty(vigNode["centerY"]) ?
                    Mathf.Clamp01(vigNode["centerY"].AsFloat) : VignetteDefaultCenter.y);
        }

        // Film Grain
        if (Volume.profile.TryGet(out _filmGrain))
        {
            JSONNode figNode = data["personalisation"]["filmGrain"];

            _filmGrain.active = !string.IsNullOrEmpty(figNode["active"])
                ? figNode["active"].AsBool : GrainDefaultActive;

            FilmGrainType = !string.IsNullOrEmpty(figNode["type"])
                ? figNode["type"].AsInt : GrainDefaultType;

            _filmGrain.type.value = FilmGrainType switch
            {
                0 => FilmGrainLookup.Thin1,
                1 => FilmGrainLookup.Medium1,
                _ => FilmGrainLookup.Large01
            };

            _filmGrain.intensity.value = !string.IsNullOrEmpty(figNode["intensity"])
                ? Mathf.Clamp01(figNode["intensity"].AsFloat) : GrainDefaultIntensity;

            _filmGrain.response.value = !string.IsNullOrEmpty(figNode["response"])
                ? Mathf.Clamp01(figNode["response"].AsFloat) : GrainDefaultResponse;
        }

        // White Balance
        if (Volume.profile.TryGet(out _whiteBalance)) 
        {
            JSONNode whiNode = data["personalisation"]["whiteBalance"];

            _whiteBalance.active = !string.IsNullOrEmpty(whiNode["active"])
                ? whiNode["active"].AsBool : WhiteDefaultActive;

            _whiteBalance.temperature.value = !string.IsNullOrEmpty(whiNode["temperature"])
                ?  Mathf.Clamp(whiNode["temperature"].AsFloat, _whiteBalance.temperature.min, _whiteBalance.temperature.max) : WhiteDefaultTemperature;

            _whiteBalance.tint.value = !string.IsNullOrEmpty(whiNode["tint"])
                ? Mathf.Clamp(whiNode["tint"].AsFloat, _whiteBalance.temperature.min, _whiteBalance.temperature.max) : WhiteDefaultTint;
        }

        // Lens Distortion
        if (Volume.profile.TryGet(out _lensDistortion))
        {
            JSONNode lenNode = data["personalisation"]["lensDistortion"];

            _lensDistortion.active = !string.IsNullOrEmpty(lenNode["active"])
                ? lenNode["active"].AsBool : LensDefaultActive;

            _lensDistortion.intensity.value = !string.IsNullOrEmpty(lenNode["intensity"])
                ? Mathf.Clamp(lenNode["intensity"].AsFloat, 0, 0.1f) : LensDefaultIntensity;
        }

        // Panini Projection
        if (Volume.profile.TryGet(out _paniniProjection))
        {
            JSONNode panNode = data["personalisation"]["paniniProjection"];

            _paniniProjection.active = !string.IsNullOrEmpty(panNode["active"])
                ? panNode["active"].AsBool : PaniniDefaultActive;

            _paniniProjection.distance.value = !string.IsNullOrEmpty(panNode["distance"])
                ? Mathf.Clamp(panNode["distance"].AsFloat, 0, 0.1f) : PaniniDefaultDistance;
        }

        // Chromatic Aberration
        if (Volume.profile.TryGet(out _chromaticAberration))
        {
            JSONNode chrNode = data["personalisation"]["chromaticAberration"];

            _chromaticAberration.active = !string.IsNullOrEmpty(chrNode["active"])
                ? chrNode["active"].AsBool : AberrationDefaultActive;

            _chromaticAberration.intensity.value = !string.IsNullOrEmpty(chrNode["intensity"])
                ? Mathf.Clamp(chrNode["intensity"].AsFloat, 0, 0.2f) : AberrationDefaultIntensity;
        }

        // Theme coloring
        ColorSettings.RefreshColors();
    }

    /// <summary> Overwrite all personalisation settings to a JSONNode. </summary>
    public void SavePersonalisationData()
    {
        JSONNode data = YoutubeSaveData.GetData();
        data["personalisation"]["bloom"]["active"]     = _bloom.active ? "true" : "false";
        data["personalisation"]["bloom"]["intensity"]  = _bloom.intensity.value.ToString();
        data["personalisation"]["bloom"]["scatter"]    = _bloom.scatter.value.ToString();
        data["personalisation"]["bloom"]["tintR"]      = _bloom.tint.value.r.ToString();
        data["personalisation"]["bloom"]["tintG"]      = _bloom.tint.value.g.ToString();
        data["personalisation"]["bloom"]["tintB"]      = _bloom.tint.value.b.ToString();

        data["personalisation"]["vignette"]["active"]      = _vignette.active ? "true" : "false";
        data["personalisation"]["vignette"]["intensity"]   = _vignette.intensity.value.ToString();
        data["personalisation"]["vignette"]["smoothness"]  = _vignette.smoothness.value.ToString();
        data["personalisation"]["vignette"]["rounded"]     = _vignette.rounded.value ? "true" : "false";
        data["personalisation"]["vignette"]["colorR"]      = _vignette.color.value.r.ToString();
        data["personalisation"]["vignette"]["colorG"]      = _vignette.color.value.g.ToString();
        data["personalisation"]["vignette"]["colorB"]      = _vignette.color.value.b.ToString();
        data["personalisation"]["vignette"]["centerX"]     = _vignette.center.value.x.ToString();
        data["personalisation"]["vignette"]["centerY"]     = _vignette.center.value.y.ToString();

        data["personalisation"]["filmGrain"]["active"]     = _filmGrain.active ? "true" : "false";
        data["personalisation"]["filmGrain"]["type"]       = FilmGrainType.ToString();
        data["personalisation"]["filmGrain"]["intensity"]  = _filmGrain.intensity.value.ToString();
        data["personalisation"]["filmGrain"]["response"]   = _filmGrain.response.value.ToString();

        data["personalisation"]["whiteBalance"]["active"]      = _whiteBalance.active ? "true" : "false";
        data["personalisation"]["whiteBalance"]["temperature"] = _whiteBalance.temperature.value.ToString();
        data["personalisation"]["whiteBalance"]["tint"]        = _whiteBalance.tint.value.ToString();

        data["personalisation"]["lensDistortion"]["active"]    = _lensDistortion.active ? "true" : "false";
        data["personalisation"]["lensDistortion"]["intensity"] = _lensDistortion.intensity.value.ToString();

        data["personalisation"]["paniniProjection"]["active"]   = _paniniProjection.active ? "true" : "false";
        data["personalisation"]["paniniProjection"]["distance"] = _paniniProjection.distance.value.ToString();

        data["personalisation"]["chromaticAberration"]["active"]    = _chromaticAberration.active ? "true" : "false";
        data["personalisation"]["chromaticAberration"]["intensity"] = _chromaticAberration.intensity.value.ToString();

        ColorSettings.SaveColors();
    }

}
