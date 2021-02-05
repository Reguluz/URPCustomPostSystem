using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CanEditMultipleObjects]
[CustomEditor(typeof(CustomUberPostSetter))]
public class CustomUberPostSetterIns : Editor
{
    private CustomUberPostSetter _targetScript;
    private CustomUberPostSetterIns ins;
    private GUIStyle title;
    private void Awake()
    {
        _targetScript = target as CustomUberPostSetter;
        
    }

    private void OnEnable()
    {
        title = new GUIStyle() {
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState(){textColor = Color.white}
        };
    }

    public override void OnInspectorGUI()
    {
        
        _targetScript.OverlayEnable =
            EditorGUILayout.Toggle("Overlay Enable", _targetScript.OverlayEnable);
        _targetScript.EyeMaskEnable =
            EditorGUILayout.Toggle("Eye Mask Enable", _targetScript.EyeMaskEnable);
        _targetScript.RadialBlurEnable =
            EditorGUILayout.Toggle("Radial Blur Enable", _targetScript.RadialBlurEnable);
        _targetScript.DualKawaseDOFEnable =
            EditorGUILayout.Toggle("DOF Enable", _targetScript.DualKawaseDOFEnable);
        _targetScript.RainDropEnable =
            EditorGUILayout.Toggle("Rain Drop Enable", _targetScript.RainDropEnable);
        _targetScript.VignetteEnable =
            EditorGUILayout.Toggle("Vignette Enable", _targetScript.VignetteEnable);
        _targetScript.GlitchEnable =
            EditorGUILayout.Toggle("Glitch Enable", _targetScript.GlitchEnable);
        _targetScript.SaturateEnable =
            EditorGUILayout.Toggle("Saturate Enable", _targetScript.SaturateEnable);

        if (_targetScript.OverlayEnable)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Overlay", title);
            _targetScript.settings.OverlayEnable = EditorGUILayout.Toggle("Overlay Enable", _targetScript.settings.OverlayEnable);
            _targetScript.OverlayTexReplace = EditorGUILayout.ObjectField("OverlayTexReplace", _targetScript.OverlayTexReplace, typeof(Texture), false) as Texture2D;
            _targetScript.NormalTexReplace = EditorGUILayout.ObjectField("NormalTexReplace", _targetScript.NormalTexReplace, typeof(Texture), false) as Texture2D;
            _targetScript.settings.PolarUV = EditorGUILayout.Toggle("PolarUV", _targetScript.settings.PolarUV);
            _targetScript.settings.NormalStr =
                EditorGUILayout.Slider("NormalStr", _targetScript.settings.NormalStr,0,3);
            _targetScript.settings.GradientOffset =
                EditorGUILayout.FloatField("GradientOffset", _targetScript.settings.GradientOffset);
            _targetScript.settings.OverlayCoordiate =
                EditorGUILayout.Vector4Field("OverlayCoordiate", _targetScript.settings.OverlayCoordiate);
            _targetScript.settings.NormalCoordiate =
                EditorGUILayout.Vector4Field("NormalCoordiate", _targetScript.settings.NormalCoordiate);
        }

        if (_targetScript.EyeMaskEnable)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("EyeMask", title);
            _targetScript.settings.EyeMaskEnable =
                EditorGUILayout.Toggle("EyeMask Enable", _targetScript.settings.EyeMaskEnable);
            _targetScript.settings.Handle = EditorGUILayout.Slider("Handle", _targetScript.settings.Handle, 0, 1);
            _targetScript.settings.Gradient =
                EditorGUILayout.Slider("Gradient", _targetScript.settings.Gradient, .01f, .99f);
        }

        if (_targetScript.RadialBlurEnable)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("RadialBlur", title);
            _targetScript.settings.RadialBlurEnable =
                EditorGUILayout.Toggle("RadialBlur Enable", _targetScript.settings.RadialBlurEnable);
            _targetScript.settings.RadialBlurCenter =
                EditorGUILayout.Vector2Field("RadialBlur Center", _targetScript.settings.RadialBlurCenter);
            _targetScript.settings.RadialBlurMaskStr = EditorGUILayout.Slider("RadialBlur Mask Str",
                _targetScript.settings.RadialBlurMaskStr, 0, 2);
            _targetScript.settings.RadialBlurSampleTime = EditorGUILayout.IntSlider("RadialBlur Sample Time",
                _targetScript.settings.RadialBlurSampleTime, 1, 10);
            _targetScript.settings.RadialBlurOffsetStr = EditorGUILayout.Slider("RadialBlur Offset Str",
                _targetScript.settings.RadialBlurOffsetStr, 0, 0.5f);
        }

        if (_targetScript.DualKawaseDOFEnable)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("DOF", title);
            _targetScript.settings.DualKawaseDOFEnable =
                EditorGUILayout.Toggle("DOF Enable", _targetScript.settings.DualKawaseDOFEnable);
            _targetScript.settings.BlurRadius = EditorGUILayout.Slider("DOF Blur Radius",
                _targetScript.settings.BlurRadius, 0.01f, 10);
            _targetScript.settings.Iteration = EditorGUILayout.IntSlider("DOF Iteration",
                _targetScript.settings.Iteration, 1, 10);
            _targetScript.settings.RTDownScaling = EditorGUILayout.IntSlider("DOF DownScaling",
                _targetScript.settings.RTDownScaling, 0, 4);
            _targetScript.settings.FocusLength = Mathf.Max(0, EditorGUILayout.FloatField("Focus Length",
                _targetScript.settings.FocusLength));
            _targetScript.settings.DOFRadius =  Mathf.Max(0, EditorGUILayout.FloatField("DOF Radius",
                _targetScript.settings.DOFRadius));
        }

        if (_targetScript.RainDropEnable)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("RainDrop", title);
            _targetScript.settings.RainDropEnable =
                EditorGUILayout.Toggle("RainDrop Enable", _targetScript.settings.RainDropEnable);
            _targetScript.settings.RainForce =
                EditorGUILayout.Slider("RainForce", _targetScript.settings.RainForce, 0, 1);
            _targetScript.settings.RainDropSize =
                EditorGUILayout.Slider("Rain Drop Size", _targetScript.settings.RainDropSize, 0, 10);
            _targetScript.settings.RainDropAngle =
                EditorGUILayout.Slider("Rain Drop Angle", _targetScript.settings.RainDropAngle, -90, 90);
            _targetScript.settings.DropAspect =
                Mathf.Max(0,EditorGUILayout.FloatField("Drop Aspect", _targetScript.settings.DropAspect));
            _targetScript.settings.DropSpeed =
                EditorGUILayout.Slider("Rain Drop Size", _targetScript.settings.DropSpeed, 0, 5);
            _targetScript.settings.DropMotionTurb =
                EditorGUILayout.Slider("Rain Drop Size", _targetScript.settings.DropMotionTurb, 0, 5);
            _targetScript.settings.Distortion =
                EditorGUILayout.Slider("Rain Drop Size", _targetScript.settings.Distortion, 0, 10);
            _targetScript.settings.Blur =
                EditorGUILayout.Slider("Rain Drop Size", _targetScript.settings.Blur, 0, 1);
        }

        if (_targetScript.VignetteEnable)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Vignette", title);
            _targetScript.settings.VignetteEnable =
                EditorGUILayout.Toggle("Vignette Enable", _targetScript.settings.VignetteEnable);
            _targetScript.settings.VignetteColor =
                EditorGUILayout.ColorField("Vignette Color", _targetScript.settings.VignetteColor);
            _targetScript.settings.VigenetteCenter =
                EditorGUILayout.Vector2Field("Vignette Center", _targetScript.settings.VigenetteCenter);
            _targetScript.settings.VignetteIntensity =
                EditorGUILayout.Slider("Vignette Intensity", _targetScript.settings.VignetteIntensity, 0, 1);
            _targetScript.settings.VignetteSmoothness = EditorGUILayout.Slider("Vignette Smoothness",
                _targetScript.settings.VignetteSmoothness, 0.001f, 1);
        }

        if (_targetScript.GlitchEnable)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Glitch", title);
            _targetScript.settings.GlitchEnable =
                EditorGUILayout.Toggle("Glitch Enable", _targetScript.settings.GlitchEnable);
            _targetScript.settings.ScanLineJitter =
                EditorGUILayout.FloatField("Scanline Jitter", _targetScript.settings.ScanLineJitter);
            _targetScript.settings.VerticleJump =
                EditorGUILayout.FloatField("Verticle Jump", _targetScript.settings.VerticleJump);
            _targetScript.settings.HorizontalShake =
                EditorGUILayout.FloatField("Horizontal Shake", _targetScript.settings.HorizontalShake);
            _targetScript.settings.RGBShiftAmp =
                EditorGUILayout.FloatField("RGB Shift Amp", _targetScript.settings.RGBShiftAmp);
            _targetScript.settings.ManualShift =
                EditorGUILayout.Vector4Field("Manual Shift", _targetScript.settings.ManualShift);
            _targetScript.settings.ShiftSpeed =
                EditorGUILayout.Slider("Shift Speed", _targetScript.settings.ShiftSpeed, .01f, 10);
            _targetScript.settings.Lighten = EditorGUILayout.FloatField("Lighten", _targetScript.settings.Lighten);
            _targetScript.settings.Color1 = EditorGUILayout.ColorField(new GUIContent("Color1"),
                _targetScript.settings.Color1, true, false, true);
            _targetScript.settings.Color2 = EditorGUILayout.ColorField(new GUIContent("Color2"),
                _targetScript.settings.Color2, true, false, true);
            _targetScript.settings.Color3 = EditorGUILayout.ColorField(new GUIContent("Color3"),
                _targetScript.settings.Color3, true, false, true);
            _targetScript.settings.Balance =
                EditorGUILayout.Slider("Balance", _targetScript.settings.Balance, 0, 1);
            _targetScript.settings.X = EditorGUILayout.Toggle("X", _targetScript.settings.X);
            _targetScript.settings.Y = EditorGUILayout.Toggle("Y", _targetScript.settings.Y);
        }

        if (_targetScript.SaturateEnable)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Saturate", title);
            _targetScript.settings.SaturateEnable =
                EditorGUILayout.Toggle("Saturate Enable", _targetScript.settings.SaturateEnable);
            _targetScript.settings.SaturateStr =
                EditorGUILayout.Slider("Saturate Str", _targetScript.settings.SaturateStr, 0, 1);
        }
        
    }
}
