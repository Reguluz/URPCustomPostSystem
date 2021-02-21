using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// [Serializable]
// public struct PropertyToggle
// {
//     public bool OverlayEnable;
//     public bool EyeMaskEnable;
//     public bool RadialBlurEnable;
//     public bool DualKawaseDOFEnable;
//     public bool RainDropEnable;
//     public bool VignetteEnable;
//     public bool GlitchEnable;
//     public bool SaturateEnable;
// }
[ExecuteInEditMode]
[Serializable]
public class CustomUberPostSetter : MonoBehaviour
{
    // public PropertyToggle Toggle;
    public bool Overlay;
    public bool EyeMask;
    public bool RadialBlur;
    public bool DualKawaseDOF;
    public bool RainDrop;
    public bool Vignette;
    public bool Glitch;
    public bool Saturate;
    [Space(10)]
    public Texture2D OverlayTexReplace;
    public Texture2D NormalTexReplace;
    // [SerializeField]public CustomUberPostFeature.CustomUberPostSettings settings;
    
    private CustomUberPostFeature _uberPost;
    private static readonly int StaticSideTex = Shader.PropertyToID("_SideTex");
    private static readonly int StaticNormalTex = Shader.PropertyToID("_NormalTex");
    private Texture2D oldOverlayTex;
    private Texture2D oldNormalTex;
    
    [SerializeField][Range(0.1f,10)]public float ScreenAspect = 16 / 9;
    public bool IDMask;

    [Header("Overlay")] public bool OverlayEnable;
    [SerializeField]public bool PolarUV;
    [SerializeField][Range(0, 3)] public float NormalStr = 0;
    [SerializeField]public float GradientOffset = 0;
    [SerializeField]public Vector4 OverlayCoordiate = new Vector4(1,1,0,0);
    [SerializeField]public Vector4 NormalCoordiate = new Vector4(1,1,0,0);
    
    [Header("EyeMask")] public bool EyeMaskEnable;
    [SerializeField][Range(0, 1)] public float Handle = 0.5f;
    [SerializeField][Range(0.01f, 0.99f)]public float Gradient = 0.5f;

    [Header("RadialBlur")] public bool RadialBlurEnable;
    [SerializeField]public Vector2 RadialBlurCenter = new Vector2(0.5f, 0.5f);
    [SerializeField][Range(0,2)]public float RadialBlurMaskStr = 1;
    [SerializeField][Range(1,10)]public int RadialBlurSampleTime = 3;
    [SerializeField][Range(0, 0.5f)] public float RadialBlurOffsetStr = 0.1f;
    
    [Header("Custom DualKawaseDOF")] public bool DualKawaseDOFEnable;
    [SerializeField][Range(0.01f,10)]public float BlurRadius = 0.1f;
    [SerializeField][Range(1,10)]public int Iteration = 1;
    [SerializeField][Range(0,4)]public int RTDownScaling = 0;
    [SerializeField][Min(0)] public float FocusLength = 50;
    [SerializeField][Min(0)] public float DOFRadius = 25;

    [Header("RainDrop")] public bool RainDropEnable;
    [SerializeField][Range(0, 1)] public float RainForce = 1;
    [SerializeField][Range(0, 10)] public float RainDropSize = 1;
    [SerializeField][Range(-90, 90)] public float RainDropAngle = 0;
    [SerializeField][Min(0.1f)]public float DropAspect = 2;
    [SerializeField][Range(0, 5)] public float DropSpeed = 0.2f;
    [SerializeField][Range(0, 5)] public float DropMotionTurb = 1;
    [SerializeField][Range(0, 10)] public float Distortion = 1f;
    [SerializeField][Range(0, 1)] public float Blur = 0.5f;

    [Header("Custom Vignette")] public bool VignetteEnable;
    [SerializeField][ColorUsage(true, true)] public Color VignetteColor;
    [SerializeField]public Vector2 VigenetteCenter = new Vector2(0.5f, 0.5f);
    [SerializeField][Range(0, 1)] public float VignetteIntensity = 0;
    [SerializeField][Range(0.001f, 1)] public float VignetteSmoothness = 0.5f;

    [Header("Glitch")] public bool GlitchEnable;
    [SerializeField]public float ScanLineJitter = 0.25f;
    [SerializeField]public float VerticleJump = 1;
    [SerializeField]public float HorizontalShake = 0.1f;
    [SerializeField]public float RGBShiftAmp = 0;
    [SerializeField]public Vector4 ManualShift = new Vector4(0.2f, -0.2f, 0.5f, 0.01f);
    [SerializeField][Range(0.01f, 10)]public float ShiftSpeed = 1f;
    [SerializeField]public float Lighten = 0.01f;
    [SerializeField][ColorUsage(false, true)]public Color Color1 = Color.red;
    [SerializeField][ColorUsage(false, true)]public Color Color2 = Color.green;
    [SerializeField][ColorUsage(false, true)]public Color Color3 = Color.blue;
    [SerializeField][Range(0, 1)] public float Balance = 0.5f;
    [SerializeField]public bool X = true;
    [SerializeField]public bool Y = false;

    [Header("Saturate")] public bool SaturateEnable;
    [SerializeField][Range(0, 1)] public float SaturateStr;

    // Start is called before the first frame update
    void OnEnable()
    {
        _uberPost = CustomUberPostFeature.Instance;
        if (Overlay)
        {
            if (OverlayTexReplace != null)
            {
                oldOverlayTex = _uberPost.featureSettings.blitMaterial.GetTexture(StaticSideTex) as Texture2D;
                _uberPost.featureSettings.blitMaterial.SetTexture(StaticSideTex, OverlayTexReplace);
            }

            if (NormalTexReplace != null)
            {
                oldNormalTex = _uberPost.featureSettings.blitMaterial.GetTexture(StaticNormalTex) as Texture2D;
                _uberPost.featureSettings.blitMaterial.SetTexture(StaticNormalTex, NormalTexReplace);
            }
        }
        
    }

    private void OnDisable()
    {
        if (Overlay)
        {
            if (oldOverlayTex != null)
            {
                _uberPost.featureSettings.blitMaterial.SetTexture(StaticSideTex, oldOverlayTex);
            }

            if (oldNormalTex != null)
            {
                _uberPost.featureSettings.blitMaterial.SetTexture(StaticNormalTex, oldNormalTex);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Updated");
        // if(_uberPost == null)_uberPost = CustomUberPostFeature.Instance;
        bool changed = false;
        if (Overlay)
        {
            // _uberPost.settings.CopyOverlay(settings);
            _uberPost.settings.OverlayEnable = OverlayEnable;
            if (!OverlayEnable)return;
            // Debug.Log("Changed");
            _uberPost.settings.PolarUV = PolarUV;
            _uberPost.settings.OverlayCoordiate = OverlayCoordiate;
            _uberPost.settings.NormalCoordiate = NormalCoordiate;
            _uberPost.settings.NormalStr = NormalStr;
            _uberPost.settings.GradientOffset = GradientOffset;
            changed = true;
        }
        if (EyeMask)
        {
            // _uberPost.settings.CopyEyeMask(settings);
            _uberPost.settings.EyeMaskEnable = EyeMaskEnable;
            if (!EyeMaskEnable) return;
            // Debug.Log("Run");
            _uberPost.settings.Handle = Handle;
            _uberPost.settings.Gradient = Gradient;
            changed = true;
        }
        if (RadialBlur)
        {
            // _uberPost.settings.CopyRadialBlur(settings);
            _uberPost.settings.RadialBlurEnable = RadialBlurEnable;
            if (!RadialBlurEnable) return;
            _uberPost.settings.RadialBlurCenter = RadialBlurCenter;
            _uberPost.settings.RadialBlurMaskStr = RadialBlurMaskStr;
            _uberPost.settings.RadialBlurSampleTime = RadialBlurSampleTime;
            _uberPost.settings.RadialBlurOffsetStr = RadialBlurOffsetStr;
            changed = true;
        }
        if (DualKawaseDOF)
        {
            // _uberPost.settings.CopyDOF(settings);
            _uberPost.settings.DualKawaseDOFEnable = DualKawaseDOFEnable;
            if (!DualKawaseDOFEnable) return;
            _uberPost.settings.BlurRadius = BlurRadius;
            _uberPost.settings.Iteration = Iteration;
            _uberPost.settings.RTDownScaling = RTDownScaling;
            _uberPost.settings.FocusLength = FocusLength;
            _uberPost.settings.DOFRadius = DOFRadius;
            changed = true;
        }
        if (RainDrop)
        {
            // _uberPost.settings.CopyRainDrop(settings);
            _uberPost.settings.RainDropEnable = RainDropEnable;
            if (!RainDropEnable)return;
            _uberPost.settings.RainForce = RainForce;
            _uberPost.settings.RainDropSize = RainDropSize;
            _uberPost.settings.RainDropAngle = RainDropAngle;
            _uberPost.settings.DropAspect = DropAspect;
            _uberPost.settings.DropSpeed = DropSpeed;
            _uberPost.settings.DropMotionTurb = DropMotionTurb;
            _uberPost.settings.Distortion = Distortion;
            _uberPost.settings.Blur = Blur;
            changed = true;
        }
        if (Vignette)
        {
            // _uberPost.settings.CopyVignette(settings);
            _uberPost.settings.VignetteEnable = VignetteEnable;
            if (!VignetteEnable) return;
            _uberPost.settings.VignetteColor = VignetteColor;
            _uberPost.settings.VigenetteCenter = VigenetteCenter;
            _uberPost.settings.VignetteIntensity = VignetteIntensity;
            _uberPost.settings.VignetteSmoothness = VignetteSmoothness;
            changed = true;
        }
        if (Glitch)
        {
            // _uberPost.settings.CopyGlitch(settings);
            _uberPost.settings.GlitchEnable = GlitchEnable;
            if (!GlitchEnable) return;
            _uberPost.settings.ScanLineJitter = ScanLineJitter;
            _uberPost.settings.VerticleJump = VerticleJump;
            _uberPost.settings.HorizontalShake = HorizontalShake;
            _uberPost.settings.RGBShiftAmp = RGBShiftAmp;
            _uberPost.settings.ShiftSpeed = ShiftSpeed;
            _uberPost.settings.ManualShift = ManualShift;
            _uberPost.settings.Lighten = Lighten;
            _uberPost.settings.Color1 = Color1;
            _uberPost.settings.Color2 = Color2;
            _uberPost.settings.Color3 = Color3;
            _uberPost.settings.Balance = Balance;
            _uberPost.settings.X = X;
            _uberPost.settings.Y = Y;
            changed = true;
        }
        if (Saturate)
        {
            // _uberPost.settings.CopySaturate(settings);
            _uberPost.settings.SaturateEnable = SaturateEnable;
            _uberPost.settings.SaturateStr = SaturateEnable ? SaturateStr : 0;
            changed = true;
        }

        if (changed)
        {
            _uberPost.RefreshSettings();
        }
    }
}
