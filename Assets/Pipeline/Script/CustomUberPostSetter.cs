using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct PropertyToggle
{
    public bool OverlayEnable;
    public bool EyeMaskEnable;
    public bool RadialBlurEnable;
    public bool DualKawaseDOFEnable;
    public bool RainDropEnable;
    public bool VignetteEnable;
    public bool GlitchEnable;
    public bool SaturateEnable;
}
[ExecuteInEditMode]
public class CustomUberPostSetter : MonoBehaviour
{
    // public PropertyToggle Toggle;
    public bool OverlayEnable;
    public bool EyeMaskEnable;
    public bool RadialBlurEnable;
    public bool DualKawaseDOFEnable;
    public bool RainDropEnable;
    public bool VignetteEnable;
    public bool GlitchEnable;
    public bool SaturateEnable;
    [Space(10)]
    public Texture2D OverlayTexReplace;
    public Texture2D NormalTexReplace;
    public CustomUberPostFeature.CustomUberPostSettings settings;
    private CustomUberPostFeature _uberPost;
    private static readonly int StaticSideTex = Shader.PropertyToID("_SideTex");
    private static readonly int StaticNormalTex = Shader.PropertyToID("_NormalTex");
    private Texture2D oldOverlayTex;
    private Texture2D oldNormalTex;

    // Start is called before the first frame update
    void OnEnable()
    {
        _uberPost = CustomUberPostFeature.Instance;
        if (OverlayEnable)
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
        if (OverlayEnable)
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
        bool changed = false;
        if (OverlayEnable)
        {
            _uberPost.settings.CopyOverlay(settings);
            changed = true;
        }
        if (EyeMaskEnable)
        {
            _uberPost.settings.CopyEyeMask(settings);
            changed = true;
        }
        if (RadialBlurEnable)
        {
            _uberPost.settings.CopyRadialBlur(settings);
            changed = true;
        }
        if (DualKawaseDOFEnable)
        {
            _uberPost.settings.CopyDOF(settings);
            changed = true;
        }
        if (RainDropEnable)
        {
            _uberPost.settings.CopyRainDrop(settings);
            changed = true;
        }
        if (VignetteEnable)
        {
            _uberPost.settings.CopyVignette(settings);
            changed = true;
        }
        if (GlitchEnable)
        {
            _uberPost.settings.CopyGlitch(settings);
            changed = true;
        }
        if (SaturateEnable)
        {
            _uberPost.settings.CopySaturate(settings);
            changed = true;
        }

        if (changed)
        {
            _uberPost.RefreshSettings();
        }
    }
}
