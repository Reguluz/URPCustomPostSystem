using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomUberPostPass : ScriptableRenderPass
{
    public Material blitMaterial = null;
    private RenderTargetIdentifier source { get; set; }
    private RenderTextureDescriptor descriptor { get; set; }

    private CustomUberPostFeature.CustomUberPostSettings _uberSettings = new CustomUberPostFeature.CustomUberPostSettings();
    FilteringSettings m_FilteringSettings;
    const string m_ProfilerTag = "Custom Uber Pass";
    private RenderTargetIdentifier InputUse;
    private RenderTargetIdentifier OutputUse;
    private int onUseSerial;
    
    private BlurSampleLevel[] m_Pyramid;
    const int k_MaxPyramidSize = 16;
    Vector2Int widthHeight = Vector2Int.zero;
    private int lastUp;

    private static readonly int STATIC_Next1 = Shader.PropertyToID("_Next1");
    private static readonly int STATIC_Next2 = Shader.PropertyToID("_Next2");
    private static readonly int STATIC_ScreenAspect = Shader.PropertyToID("_ScreenAspect");
    private static readonly int STATIC_IdMask = Shader.PropertyToID("_IDMask");
    
    /***** Overlay *****/
    private static readonly int STATIC_OverlayPass = 9;
    private static readonly int STATIC_PolarUV = Shader.PropertyToID("_PolarUV");
    private static readonly int STATIC_OverlayTex = Shader.PropertyToID("_SideTex");
    private static readonly int STATIC_OverlayCoordiate = Shader.PropertyToID("_SideTex_ST");
    private static readonly int STATIC_NormalTex = Shader.PropertyToID("_NormalTex");
    private static readonly int STATIC_NormalCoordiate = Shader.PropertyToID("_NormalTex_ST");
    private static readonly int STATIC_NormalStr = Shader.PropertyToID("_NormalStr");
    private static readonly int STATIC_GradingOffset = Shader.PropertyToID("_GradingOffset");
    
    /***** Radial Blur *****/
    private static readonly int STATIC_RadialBlurPass = 7;
    private static readonly int STATIC_RadialBlurCenter = Shader.PropertyToID("_RadialBlurCenter");
    private static readonly int STATIC_RadialBlurMaskStr = Shader.PropertyToID("_RadialBlurMaskStr");
    private static readonly int STATIC_RadialBlurSampleTime = Shader.PropertyToID("_RadialBlurSampleTime");
    private static readonly int STATIC_RadialBlurOffsetStr = Shader.PropertyToID("_RadialBlurOffsetStr");
    
    /***** Dual Kawase DOF *****/
    private static readonly int STATIC_DownSamplePass = 0;
    private static readonly int STATIC_UpSamplePass = 1;
    private static readonly int STATIC_DofPass = 2;
    private static readonly int STATIC_BlurOffset = Shader.PropertyToID("_Offset");
    private static readonly int STATIC_FocusLength = Shader.PropertyToID("_FocusLength");
    private static readonly int STATIC_DofRadius = Shader.PropertyToID("_DOFRadius");
    private static readonly int STATIC_DofSource = Shader.PropertyToID("_DOFSource");
    
    /***** RainDrop *****/
    private static readonly int STATIC_RainDropPass = 3;
    private static readonly int STATIC_RainForce = Shader.PropertyToID("_RainForce");
    private static readonly int STATIC_RainDropSize = Shader.PropertyToID("_RainDropSize");
    private static readonly int STATIC_RainDropAngle = Shader.PropertyToID("_RainDropAngle");
    private static readonly int STATIC_DropAspect = Shader.PropertyToID("_DropAspect");
    private static readonly int STATIC_DropSpeed = Shader.PropertyToID("_DropSpeed");
    private static readonly int STATIC_DropMotionTurb = Shader.PropertyToID("_DropMotionTurb");
    private static readonly int STATIC_DISTORTION = Shader.PropertyToID("_Distortion");
    private static readonly int STATIC_BLUR = Shader.PropertyToID("_Blur");
    
    /***** Eye Mask *****/
    private static readonly int STATIC_EyeMaskPass = 5;
    private static readonly int STATIC_Handle = Shader.PropertyToID("_Handle");
    private static readonly int STATIC_Gradient = Shader.PropertyToID("_Gradient");
    
    /***** Vignette*****/
    private static readonly int STATIC_CustomVignettePass = 4;
    private static readonly int STATIC_VignetteColor = Shader.PropertyToID("_VignetteColor");
    private static readonly int STATIC_VignetteCenter = Shader.PropertyToID("_VignetteCenter");
    private static readonly int STATIC_VignetteIntensity = Shader.PropertyToID("_VignetteIntensity");
    private static readonly int STATIC_VignetteSmoothness = Shader.PropertyToID("_VignetteSmoothness");
    
    /***** Glitch *****/
    private static readonly int STATIC_GlitchPass = 6;
    private static readonly int STATIC_ShiftSpeed = Shader.PropertyToID("_ShiftSpeed");
    private static readonly int STATIC_ScanLineJitter = Shader.PropertyToID("_ScanLineJitter");
    private static readonly int STATIC_VerticalJump = Shader.PropertyToID("_VerticalJump");
    private static readonly int STATIC_HorizontalShake = Shader.PropertyToID("_HorizontalShake");
    private static readonly int STATIC_RGBShiftAmp = Shader.PropertyToID("_RGBShiftAmp");
    private static readonly int STATIC_ManualShift = Shader.PropertyToID("_ManualShift");
    private static readonly int STATIC_Lighten = Shader.PropertyToID("_Lighten");
    private static readonly int STATIC_Color1 = Shader.PropertyToID("_Color1");
    private static readonly int STATIC_Color2 = Shader.PropertyToID("_Color2");
    private static readonly int STATIC_Color3 = Shader.PropertyToID("_Color3");
    private static readonly int STATIC_Balance = Shader.PropertyToID("_Balance");
    private static readonly int STATIC_X = Shader.PropertyToID("_X");
    private static readonly int STATIC_Y = Shader.PropertyToID("_Y");
    
    /***** Saturate *****/
    private static readonly int STATIC_SaturatePass = 8;
    private static readonly int STATIC_Saturate = Shader.PropertyToID("_PostCustomSaturate");
    
    struct BlurSampleLevel
    {
        internal int down;
        internal int up;
    }

    public void RefreshSettings(CustomUberPostFeature.CustomUberPostSettings settings)
    {
        _uberSettings.Copy(settings);
        blitMaterial.SetFloat(STATIC_ScreenAspect, _uberSettings.ScreenAspect);
        blitMaterial.SetFloat(STATIC_IdMask, _uberSettings.IDMask ? 1 : 0);
        if (_uberSettings.IDMask)
        {
            blitMaterial.EnableKeyword("USE_ID_TEXTURE");
        }
        else
        {
            blitMaterial.DisableKeyword("USE_ID_TEXTURE");
        }

        if (settings.OverlayEnable)
        {
            if (_uberSettings.PolarUV)
            {
                blitMaterial.EnableKeyword("USE_POLARUV");
            }
            else
            {
                blitMaterial.DisableKeyword("USE_POLARUV");
            }
            blitMaterial.SetVector(STATIC_OverlayCoordiate, _uberSettings.OverlayCoordiate);
            blitMaterial.SetVector(STATIC_NormalCoordiate, _uberSettings.NormalCoordiate);
            blitMaterial.SetFloat(STATIC_NormalStr, _uberSettings.NormalStr);
            blitMaterial.SetFloat(STATIC_GradingOffset, _uberSettings.GradientOffset);
        }
        
        if (settings.EyeMaskEnable)
        {
            blitMaterial.SetFloat(STATIC_Handle, _uberSettings.Handle);
            blitMaterial.SetFloat(STATIC_Gradient, _uberSettings.Gradient);
        }

        if (settings.RadialBlurEnable)
        {
            blitMaterial.SetVector(STATIC_RadialBlurCenter, _uberSettings.RadialBlurCenter);
            blitMaterial.SetFloat(STATIC_RadialBlurMaskStr, _uberSettings.RadialBlurMaskStr);
            blitMaterial.SetInt(STATIC_RadialBlurSampleTime, _uberSettings.RadialBlurSampleTime);
            blitMaterial.SetFloat(STATIC_RadialBlurOffsetStr, _uberSettings.RadialBlurOffsetStr);
        }
        
        if (settings.DualKawaseDOFEnable)
        {
            blitMaterial.SetFloat(STATIC_BlurOffset, _uberSettings.BlurRadius);
            blitMaterial.SetFloat(STATIC_FocusLength, _uberSettings.FocusLength);
            blitMaterial.SetFloat(STATIC_DofRadius, _uberSettings.DOFRadius);
        }

        if (settings.RainDropEnable)
        {
            blitMaterial.SetFloat(STATIC_RainForce, _uberSettings.RainForce);
            blitMaterial.SetFloat(STATIC_RainDropSize, _uberSettings.RainDropSize);
            blitMaterial.SetFloat(STATIC_RainDropAngle, _uberSettings.RainDropAngle);
            blitMaterial.SetFloat(STATIC_DropAspect, _uberSettings.DropAspect);
            blitMaterial.SetFloat(STATIC_DropSpeed, _uberSettings.DropSpeed);
            blitMaterial.SetFloat(STATIC_DropMotionTurb, _uberSettings.DropMotionTurb);
            blitMaterial.SetFloat(STATIC_DISTORTION, _uberSettings.Distortion);
            blitMaterial.SetFloat(STATIC_BLUR, _uberSettings.Blur);
        }

        if (settings.VignetteEnable)
        {
            blitMaterial.SetColor(STATIC_VignetteColor, _uberSettings.VignetteColor);
            blitMaterial.SetVector(STATIC_VignetteCenter, _uberSettings.VigenetteCenter);
            blitMaterial.SetFloat(STATIC_VignetteIntensity, _uberSettings.VignetteIntensity);
            blitMaterial.SetFloat(STATIC_VignetteSmoothness, _uberSettings.VignetteSmoothness);
        }

        if (settings.GlitchEnable)
        {
            blitMaterial.SetFloat(STATIC_ScanLineJitter, _uberSettings.ScanLineJitter);
            blitMaterial.SetFloat(STATIC_VerticalJump, _uberSettings.VerticleJump);
            blitMaterial.SetFloat(STATIC_HorizontalShake, _uberSettings.HorizontalShake);
            blitMaterial.SetFloat(STATIC_RGBShiftAmp, _uberSettings.RGBShiftAmp);
            blitMaterial.SetFloat(STATIC_ShiftSpeed, _uberSettings.ShiftSpeed);
            blitMaterial.SetVector(STATIC_ManualShift, _uberSettings.ManualShift);
            blitMaterial.SetFloat(STATIC_Lighten, _uberSettings.Lighten);
            blitMaterial.SetColor(STATIC_Color1, _uberSettings.Color1);
            blitMaterial.SetColor(STATIC_Color2, _uberSettings.Color2);
            blitMaterial.SetColor(STATIC_Color3, _uberSettings.Color3);
            blitMaterial.SetFloat(STATIC_Balance, _uberSettings.Balance);
            blitMaterial.SetFloat(STATIC_X, _uberSettings.X ? 1 : 0);
            blitMaterial.SetFloat(STATIC_Y, _uberSettings.Y ? 1 : 0);
        }
        
        blitMaterial.SetFloat(STATIC_Saturate, _uberSettings.SaturateStr);
    }

    public void RefreshState(CustomUberPostFeature.CustomUberPostSettings settings)
    {
        _uberSettings.EyeMaskEnable = settings.EyeMaskEnable;
        _uberSettings.DualKawaseDOFEnable = settings.DualKawaseDOFEnable;
        _uberSettings.RainDropEnable = settings.RainDropEnable;
        _uberSettings.VignetteEnable = settings.VignetteEnable;
        _uberSettings.GlitchEnable = settings.GlitchEnable;
    }
    public CustomUberPostPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, Material blitMaterial, CustomUberPostFeature.CustomUberPostSettings settings)
    {
        this.renderPassEvent = evt;
        this.blitMaterial = blitMaterial;
        m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
        RefreshSettings(settings);
        
        /******** DualKawase ********/
        m_Pyramid = new BlurSampleLevel[k_MaxPyramidSize];
        for (int i = 0; i < k_MaxPyramidSize; i++)
        {
            m_Pyramid[i] = new BlurSampleLevel()
            {
                down = Shader.PropertyToID("_BlurMipDown" + i),
                up = Shader.PropertyToID("_BlurMipUp" + i)
            };
        }
    }
        
    public void Setup(
        RenderTargetIdentifier source,
        RenderTextureDescriptor descriptor)
    {
        this.source = source;
           
        this.descriptor = descriptor;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        // tmpId1 = Shader.PropertyToID("_CustomVignette");
        // cmd.GetTemporaryRT(tmpId1, cameraTextureDescriptor.width,cameraTextureDescriptor.height, 0, FilterMode.Bilinear, RenderTextureFormat.Default,RenderTextureReadWrite.Default);
        // ConfigureTarget(tmpId1);
        widthHeight = new Vector2Int(cameraTextureDescriptor.width, cameraTextureDescriptor.height);
        ConfigureTarget(source);
        //ConfigureClear(ClearFlag.All, Color.black);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        
        CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        //new version
        // cmd.GetTemporaryRT(STATIC_Next1, widthHeight.x,widthHeight.y,0,FilterMode.Bilinear,GraphicsFormat.R16G16B16_SNorm,2,true,RenderTextureMemoryless.MSAA);
        //old version
        cmd.GetTemporaryRT(STATIC_Next1, widthHeight.x,widthHeight.y,0,FilterMode.Bilinear,RenderTextureFormat.Default,RenderTextureReadWrite.Default);
        cmd.GetTemporaryRT(STATIC_Next2, widthHeight.x,widthHeight.y,0,FilterMode.Bilinear,RenderTextureFormat.Default,RenderTextureReadWrite.Default);
        InputUse = STATIC_Next1;
        OutputUse = STATIC_Next2;
        onUseSerial = 0;

        if (_uberSettings.OverlayEnable)
        {
            ExecuteOverlay(cmd, onUseSerial == 0 ? source : InputUse, OutputUse);
            ExchangeTarget();
            onUseSerial++;
        }
        
        if (_uberSettings.RadialBlurEnable)
        {
            ExecuteRadialBlur(cmd, onUseSerial == 0 ? source : InputUse, OutputUse);
            ExchangeTarget();
            onUseSerial++;
        }
        
        if (_uberSettings.DualKawaseDOFEnable)
        {
            ExecuteGaussianDof(cmd, onUseSerial == 0 ? source : InputUse, OutputUse);
            ExchangeTarget();
            onUseSerial++;
        }

        if (_uberSettings.RainDropEnable)
        {
            ExecuteRainDrop(cmd, onUseSerial == 0 ? source : InputUse, OutputUse);
            ExchangeTarget();
            onUseSerial++;
        }

        if (_uberSettings.EyeMaskEnable)
        {
            ExecuteEyeMask(cmd, onUseSerial == 0 ? source : InputUse, OutputUse);
            ExchangeTarget();
            onUseSerial++;
        }

        if (_uberSettings.VignetteEnable)
        {
            ExecuteCustomVignette(cmd, onUseSerial == 0 ? source : InputUse, OutputUse);
            ExchangeTarget();
            onUseSerial++;
        }

        if (_uberSettings.GlitchEnable)
        {
            ExecuteGlitch(cmd, onUseSerial == 0 ? source : InputUse, OutputUse);
            ExchangeTarget();
            onUseSerial++;
        }
        
        if (_uberSettings.SaturateEnable)
        {
            ExecuteSaturate(cmd, onUseSerial == 0 ? source : InputUse, OutputUse);
            ExchangeTarget();
            onUseSerial++;
        }

        if (onUseSerial != 0)
        {
            cmd.Blit(InputUse, source);
        }
        
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
        
    }

    private void ExchangeTarget()
    {
        RenderTargetIdentifier temp = InputUse;
        InputUse = OutputUse;
        OutputUse = temp;
    }
    private void ExecuteGaussianDof(CommandBuffer cmd, RenderTargetIdentifier inputUse, RenderTargetIdentifier outputUse)
    {
        int tw = (int)(widthHeight.x / Mathf.Pow(2,_uberSettings.RTDownScaling));
        int th = (int)(widthHeight.y / Mathf.Pow(2,_uberSettings.RTDownScaling));
       
        

        cmd.GetTemporaryRT(STATIC_DofSource,tw,th,0,FilterMode.Bilinear,RenderTextureFormat.Default,RenderTextureReadWrite.Default);
        // cmd.GetTemporaryRT(STATIC_FinalBlur,tw,th,0,FilterMode.Bilinear,RenderTextureFormat.Default,RenderTextureReadWrite.Default);
        cmd.Blit(inputUse, STATIC_DofSource);
        RenderTargetIdentifier lastDown = inputUse;
        for (int i = 0; i < _uberSettings.Iteration; i++)
        {
            int mipDown = m_Pyramid[i].down;
            int mipUp = m_Pyramid[i].up;
            cmd.GetTemporaryRT(mipDown,tw,th,0,FilterMode.Bilinear,RenderTextureFormat.Default,RenderTextureReadWrite.Default);
            cmd.GetTemporaryRT(mipUp,tw,th,0,FilterMode.Bilinear,RenderTextureFormat.Default,RenderTextureReadWrite.Default);
            cmd.Blit(lastDown, mipDown, blitMaterial, STATIC_DownSamplePass);
        
            lastDown = mipDown;
            tw = Mathf.Max(tw / 2, 1);
            th = Mathf.Max(th / 2, 1);
        }
        // cmd.Blit(lastDown,source);
        //
        lastUp = m_Pyramid[_uberSettings.Iteration - 1].down;
        for (int i = _uberSettings.Iteration - 2; i >= 0; i--)
        {
            int mipUp = m_Pyramid[i].up;
                
            cmd.Blit(lastUp, mipUp, blitMaterial, STATIC_UpSamplePass);
            lastUp = mipUp;
        }
        cmd.Blit(lastDown, outputUse, blitMaterial, STATIC_DofPass);
    }
    private void ExecuteOverlay(CommandBuffer cmd,
        RenderTargetIdentifier inputUse, RenderTargetIdentifier outputUse)
    {
        cmd.Blit(inputUse, outputUse, blitMaterial, STATIC_OverlayPass);
    }
    private void ExecuteRainDrop(CommandBuffer cmd,
        RenderTargetIdentifier inputUse, RenderTargetIdentifier outputUse)
    {
        cmd.Blit(inputUse, outputUse, blitMaterial, STATIC_RainDropPass);
    }
    private void ExecuteCustomVignette(CommandBuffer cmd,
        RenderTargetIdentifier inputUse, RenderTargetIdentifier outputUse)
    {
        cmd.Blit(inputUse, outputUse, blitMaterial, STATIC_CustomVignettePass);
    }
    
    private void ExecuteEyeMask(CommandBuffer cmd,
        RenderTargetIdentifier inputUse, RenderTargetIdentifier outputUse)
    {
        cmd.Blit(inputUse, outputUse, blitMaterial, STATIC_EyeMaskPass);
    }

    private void ExecuteGlitch(CommandBuffer cmd,
        RenderTargetIdentifier inputUse, RenderTargetIdentifier outputUse)
    {
        cmd.Blit(inputUse, outputUse, blitMaterial, STATIC_GlitchPass);
    }
    
    private void ExecuteRadialBlur(CommandBuffer cmd,
        RenderTargetIdentifier inputUse, RenderTargetIdentifier outputUse)
    {
        cmd.Blit(inputUse, outputUse, blitMaterial, STATIC_RadialBlurPass);
    }
    
    private void ExecuteSaturate(CommandBuffer cmd,
        RenderTargetIdentifier inputUse, RenderTargetIdentifier outputUse)
    {
        cmd.Blit(inputUse, outputUse, blitMaterial, STATIC_SaturatePass);
    }
    
    public override void FrameCleanup(CommandBuffer cmd)
    {
        if (_uberSettings.DualKawaseDOFEnable)
        {
            // Cleanup
            for (int i = 0; i < _uberSettings.Iteration; i++)
            {
                if (m_Pyramid[i].down != lastUp)
                    cmd.ReleaseTemporaryRT(m_Pyramid[i].down);
                if (m_Pyramid[i].up != lastUp)
                    cmd.ReleaseTemporaryRT(m_Pyramid[i].up);
            }
            cmd.ReleaseTemporaryRT(STATIC_DofSource);
        }
        cmd.ReleaseTemporaryRT(STATIC_Next1);
        cmd.ReleaseTemporaryRT(STATIC_Next2);
    }
}
