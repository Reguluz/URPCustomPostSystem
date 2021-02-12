using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class CustomUberPostFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class CustomUberPostFeatureSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public RenderQueueRange renderQueueRange = RenderQueueRange.opaque;
        public LayerMask layerMask;
        public Material blitMaterial = null;
    }
    
    [System.Serializable]
    public class CustomUberPostSettings
    {
        [Range(0.1f,10)]public float ScreenAspect = 16 / 9;
        public bool IDMask;

        [Header("Overlay")] public bool OverlayEnable;
        public bool PolarUV;
        [Range(0, 3)] public float NormalStr = 0;
        public float GradientOffset = 0;
        public Vector4 OverlayCoordiate = new Vector4(1,1,0,0);
        public Vector4 NormalCoordiate = new Vector4(1,1,0,0);
        
        [Header("EyeMask")] public bool EyeMaskEnable;
        [Range(0, 1)] public float Handle = 0.5f;
        [Range(0.01f, 0.99f)]public float Gradient = 0.5f;

        [Header("RadialBlur")] public bool RadialBlurEnable;
        public Vector2 RadialBlurCenter = new Vector2(0.5f, 0.5f);
        [Range(0,2)]public float RadialBlurMaskStr = 1;
        [Range(1,10)]public int RadialBlurSampleTime = 3;
        [Range(0, 0.5f)] public float RadialBlurOffsetStr = 0.1f;
        
        [Header("Custom DualKawaseDOF")] public bool DualKawaseDOFEnable;
        [Range(0.01f,10)]public float BlurRadius = 0.1f;
        [Range(1,10)]public int Iteration = 1;
        [Range(0,4)]public int RTDownScaling = 0;
        [Min(0)] public float FocusLength = 50;
        [Min(0)] public float DOFRadius = 25;

        [Header("RainDrop")] public bool RainDropEnable;
        [Range(0, 1)] public float RainForce = 1;
        [Range(0, 10)] public float RainDropSize = 1;
        [Range(-90, 90)] public float RainDropAngle = 0;
        [Min(0.1f)]public float DropAspect = 2;
        [Range(0, 5)] public float DropSpeed = 0.2f;
        [Range(0, 5)] public float DropMotionTurb = 1;
        [Range(0, 10)] public float Distortion = 1f;
        [Range(0, 1)] public float Blur = 0.5f;

        [Header("Custom Vignette")] public bool VignetteEnable;
        [ColorUsage(true, true)] public Color VignetteColor;
        public Vector2 VigenetteCenter = new Vector2(0.5f, 0.5f);
        [Range(0, 1)] public float VignetteIntensity = 0;
        [Range(0.001f, 1)] public float VignetteSmoothness = 0.5f;

        [Header("Glitch")] public bool GlitchEnable;
        public float ScanLineJitter = 0.25f;
        public float VerticleJump = 1;
        public float HorizontalShake = 0.1f;
        public float RGBShiftAmp = 0;
        public Vector4 ManualShift = new Vector4(0.2f, -0.2f, 0.5f, 0.01f);
        [Range(0.01f, 10)]public float ShiftSpeed = 1f;
        public float Lighten = 0.01f;
        [ColorUsage(false, true)]public Color Color1 = Color.red;
        [ColorUsage(false, true)]public Color Color2 = Color.green;
        [ColorUsage(false, true)]public Color Color3 = Color.blue;
        [Range(0, 1)] public float Balance = 0.5f;
        public bool X = true;
        public bool Y = false;

        [Header("Saturate")] public bool SaturateEnable;
        [Range(0, 1)] public float SaturateStr;

        public void CopyOverlay(CustomUberPostSettings postSettings)
        {
            OverlayEnable = postSettings.OverlayEnable;
            if (!OverlayEnable)return;
            Debug.Log("Changed");
            PolarUV = postSettings.PolarUV;
            OverlayCoordiate = postSettings.OverlayCoordiate;
            NormalCoordiate = postSettings.NormalCoordiate;
            NormalStr = postSettings.NormalStr;
            GradientOffset = postSettings.GradientOffset;
        }

        public void CopyEyeMask(CustomUberPostSettings postSettings)
        {
            EyeMaskEnable = postSettings.EyeMaskEnable;
            if (!EyeMaskEnable) return;
            Handle = postSettings.Handle;
            Gradient = postSettings.Gradient;
            
        }

        public void CopyRadialBlur(CustomUberPostSettings postSettings)
        {
            RadialBlurEnable = postSettings.RadialBlurEnable;
            if (!RadialBlurEnable) return;

            RadialBlurCenter = postSettings.RadialBlurCenter;
            RadialBlurMaskStr = postSettings.RadialBlurMaskStr;
            RadialBlurSampleTime = postSettings.RadialBlurSampleTime;
            RadialBlurOffsetStr = postSettings.RadialBlurOffsetStr;
        }

        public void CopyDOF(CustomUberPostSettings postSettings)
        {
            DualKawaseDOFEnable = postSettings.DualKawaseDOFEnable;
            if (!DualKawaseDOFEnable) return;
            BlurRadius = postSettings.BlurRadius;
            Iteration = postSettings.Iteration;
            RTDownScaling = postSettings.RTDownScaling;
            FocusLength = postSettings.FocusLength;
            DOFRadius = postSettings.DOFRadius;
        }

        public void CopyRainDrop(CustomUberPostSettings postSettings)
        {
            RainDropEnable = postSettings.RainDropEnable;
            if (!RainDropEnable)return;
            RainForce = postSettings.RainForce;
            RainDropSize = postSettings.RainDropSize;
            RainDropAngle = postSettings.RainDropAngle;
            DropAspect = postSettings.DropAspect;
            DropSpeed = postSettings.DropSpeed;
            DropMotionTurb = postSettings.DropMotionTurb;
            Distortion = postSettings.Distortion;
            Blur = postSettings.Blur;
        }

        public void CopyVignette(CustomUberPostSettings postSettings)
        {
            VignetteEnable = postSettings.VignetteEnable;
            if (!VignetteEnable) return;
            VignetteColor = postSettings.VignetteColor;
            VigenetteCenter = postSettings.VigenetteCenter;
            VignetteIntensity = postSettings.VignetteIntensity;
            VignetteSmoothness = postSettings.VignetteSmoothness;
        }

        public void CopyGlitch(CustomUberPostSettings postSettings)
        {
            GlitchEnable = postSettings.GlitchEnable;
            if (!GlitchEnable) return;
            ScanLineJitter = postSettings.ScanLineJitter;
            VerticleJump = postSettings.VerticleJump;
            HorizontalShake = postSettings.HorizontalShake;
            RGBShiftAmp = postSettings.RGBShiftAmp;
            ShiftSpeed = postSettings.ShiftSpeed;
            ManualShift = postSettings.ManualShift;
            Lighten = postSettings.Lighten;
            Color1 = postSettings.Color1;
            Color2 = postSettings.Color2;
            Color3 = postSettings.Color3;
            Balance = postSettings.Balance;
            X = postSettings.X;
            Y = postSettings.Y;
        }

        public void CopySaturate(CustomUberPostSettings postSettings)
        {
            SaturateEnable = postSettings.SaturateEnable;
            SaturateStr = SaturateEnable ? postSettings.SaturateStr : 0;
        }
        public void Copy(CustomUberPostSettings postSettings)
        {
            ScreenAspect = postSettings.ScreenAspect;
            IDMask = postSettings.IDMask;

            CopyOverlay(postSettings);
            CopyEyeMask(postSettings);
            CopyRadialBlur(postSettings);
            CopyDOF(postSettings);
            CopyRainDrop(postSettings);
            CopyVignette(postSettings);
            CopyGlitch(postSettings);
            CopySaturate(postSettings);
        }
        
    }
    

    public CustomUberPostFeatureSettings featureSettings = new CustomUberPostFeatureSettings();
    public CustomUberPostSettings settings = new CustomUberPostSettings();
    CustomUberPostPass pass;
    private static CustomUberPostFeature _instance;

    public static CustomUberPostFeature Instance
    {
        get => _instance;
        set => _instance = value;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        var descriptor = renderingData.cameraData.cameraTargetDescriptor;
        //RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;1
        pass.Setup(src, descriptor);
        renderer.EnqueuePass(pass);
    }

    public override void Create()
    {
        _instance = this;
        pass = new CustomUberPostPass(featureSettings.renderPassEvent, featureSettings.renderQueueRange, featureSettings.layerMask, featureSettings.blitMaterial, settings);
    }
    
    public void RefreshSettings()
    {
        pass.RefreshSettings(settings);
    }
}
