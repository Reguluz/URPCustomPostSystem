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
        
        _targetScript.Overlay =
            EditorGUILayout.Toggle("Overlay Enable", _targetScript.Overlay);
        _targetScript.EyeMask =
            EditorGUILayout.Toggle("Eye Mask Enable", _targetScript.EyeMask);
        _targetScript.RadialBlur =
            EditorGUILayout.Toggle("Radial Blur Enable", _targetScript.RadialBlur);
        _targetScript.DualKawaseDOF =
            EditorGUILayout.Toggle("DOF Enable", _targetScript.DualKawaseDOF);
        _targetScript.RainDrop =
            EditorGUILayout.Toggle("Rain Drop Enable", _targetScript.RainDrop);
        _targetScript.Vignette =
            EditorGUILayout.Toggle("Vignette Enable", _targetScript.Vignette);
        _targetScript.Glitch =
            EditorGUILayout.Toggle("Glitch Enable", _targetScript.Glitch);
        _targetScript.Saturate =
            EditorGUILayout.Toggle("Saturate Enable", _targetScript.Saturate);

        if (_targetScript.Overlay)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Overlay", title);
            _targetScript.OverlayEnable = EditorGUILayout.Toggle("Overlay Enable", _targetScript.OverlayEnable);
            _targetScript.OverlayTexReplace = EditorGUILayout.ObjectField("OverlayTexReplace", _targetScript.OverlayTexReplace, typeof(Texture), false) as Texture2D;
            _targetScript.NormalTexReplace = EditorGUILayout.ObjectField("NormalTexReplace", _targetScript.NormalTexReplace, typeof(Texture), false) as Texture2D;
            _targetScript.PolarUV = EditorGUILayout.Toggle("PolarUV", _targetScript.PolarUV);
            _targetScript.NormalStr =
                EditorGUILayout.Slider("NormalStr", _targetScript.NormalStr,0,3);
            _targetScript.GradientOffset =
                EditorGUILayout.FloatField("GradientOffset", _targetScript.GradientOffset);
            _targetScript.OverlayCoordiate =
                EditorGUILayout.Vector4Field("OverlayCoordiate", _targetScript.OverlayCoordiate);
            _targetScript.NormalCoordiate =
                EditorGUILayout.Vector4Field("NormalCoordiate", _targetScript.NormalCoordiate);
        }

        if (_targetScript.EyeMask)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("EyeMask", title);
            _targetScript.EyeMaskEnable =
                EditorGUILayout.Toggle("EyeMask Enable", _targetScript.EyeMaskEnable);
            _targetScript.Handle = EditorGUILayout.Slider("Handle", _targetScript.Handle, 0, 1);
            _targetScript.Gradient =
                EditorGUILayout.Slider("Gradient", _targetScript.Gradient, .01f, .99f);
        }

        if (_targetScript.RadialBlur)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("RadialBlur", title);
            _targetScript.RadialBlurEnable =
                EditorGUILayout.Toggle("RadialBlur Enable", _targetScript.RadialBlurEnable);
            _targetScript.RadialBlurCenter =
                EditorGUILayout.Vector2Field("RadialBlur Center", _targetScript.RadialBlurCenter);
            _targetScript.RadialBlurMaskStr = EditorGUILayout.Slider("RadialBlur Mask Str",
                _targetScript.RadialBlurMaskStr, 0, 2);
            _targetScript.RadialBlurSampleTime = EditorGUILayout.IntSlider("RadialBlur Sample Time",
                _targetScript.RadialBlurSampleTime, 1, 10);
            _targetScript.RadialBlurOffsetStr = EditorGUILayout.Slider("RadialBlur Offset Str",
                _targetScript.RadialBlurOffsetStr, 0, 0.5f);
        }

        if (_targetScript.DualKawaseDOF)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("DOF", title);
            _targetScript.DualKawaseDOFEnable =
                EditorGUILayout.Toggle("DOF Enable", _targetScript.DualKawaseDOFEnable);
            _targetScript.BlurRadius = EditorGUILayout.Slider("DOF Blur Radius",
                _targetScript.BlurRadius, 0.01f, 10);
            _targetScript.Iteration = EditorGUILayout.IntSlider("DOF Iteration",
                _targetScript.Iteration, 1, 10);
            _targetScript.RTDownScaling = EditorGUILayout.IntSlider("DOF DownScaling",
                _targetScript.RTDownScaling, 0, 4);
            _targetScript.FocusLength = Mathf.Max(0, EditorGUILayout.FloatField("Focus Length",
                _targetScript.FocusLength));
            _targetScript.DOFRadius =  Mathf.Max(0, EditorGUILayout.FloatField("DOF Radius",
                _targetScript.DOFRadius));
        }

        if (_targetScript.RainDrop)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("RainDrop", title);
            _targetScript.RainDropEnable =
                EditorGUILayout.Toggle("RainDrop Enable", _targetScript.RainDropEnable);
            _targetScript.RainForce =
                EditorGUILayout.Slider("RainForce", _targetScript.RainForce, 0, 1);
            _targetScript.RainDropSize =
                EditorGUILayout.Slider("Rain Drop Size", _targetScript.RainDropSize, 0, 10);
            _targetScript.RainDropAngle =
                EditorGUILayout.Slider("Rain Drop Angle", _targetScript.RainDropAngle, -90, 90);
            _targetScript.DropAspect =
                Mathf.Max(0,EditorGUILayout.FloatField("Drop Aspect", _targetScript.DropAspect));
            _targetScript.DropSpeed =
                EditorGUILayout.Slider("Rain Drop Size", _targetScript.DropSpeed, 0, 5);
            _targetScript.DropMotionTurb =
                EditorGUILayout.Slider("Rain Drop Size", _targetScript.DropMotionTurb, 0, 5);
            _targetScript.Distortion =
                EditorGUILayout.Slider("Rain Drop Size", _targetScript.Distortion, 0, 10);
            _targetScript.Blur =
                EditorGUILayout.Slider("Rain Drop Size", _targetScript.Blur, 0, 1);
        }

        if (_targetScript.Vignette)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Vignette", title);
            _targetScript.VignetteEnable =
                EditorGUILayout.Toggle("Vignette Enable", _targetScript.VignetteEnable);
            _targetScript.VignetteColor =
                EditorGUILayout.ColorField("Vignette Color", _targetScript.VignetteColor);
            _targetScript.VigenetteCenter =
                EditorGUILayout.Vector2Field("Vignette Center", _targetScript.VigenetteCenter);
            _targetScript.VignetteIntensity =
                EditorGUILayout.Slider("Vignette Intensity", _targetScript.VignetteIntensity, 0, 1);
            _targetScript.VignetteSmoothness = EditorGUILayout.Slider("Vignette Smoothness",
                _targetScript.VignetteSmoothness, 0.001f, 1);
        }

        if (_targetScript.Glitch)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Glitch", title);
            _targetScript.GlitchEnable =
                EditorGUILayout.Toggle("Glitch Enable", _targetScript.GlitchEnable);
            _targetScript.ScanLineJitter =
                EditorGUILayout.FloatField("Scanline Jitter", _targetScript.ScanLineJitter);
            _targetScript.VerticleJump =
                EditorGUILayout.FloatField("Verticle Jump", _targetScript.VerticleJump);
            _targetScript.HorizontalShake =
                EditorGUILayout.FloatField("Horizontal Shake", _targetScript.HorizontalShake);
            _targetScript.RGBShiftAmp =
                EditorGUILayout.FloatField("RGB Shift Amp", _targetScript.RGBShiftAmp);
            _targetScript.ManualShift =
                EditorGUILayout.Vector4Field("Manual Shift", _targetScript.ManualShift);
            _targetScript.ShiftSpeed =
                EditorGUILayout.Slider("Shift Speed", _targetScript.ShiftSpeed, .01f, 10);
            _targetScript.Lighten = EditorGUILayout.FloatField("Lighten", _targetScript.Lighten);
            _targetScript.Color1 = EditorGUILayout.ColorField(new GUIContent("Color1"),
                _targetScript.Color1, true, false, true);
            _targetScript.Color2 = EditorGUILayout.ColorField(new GUIContent("Color2"),
                _targetScript.Color2, true, false, true);
            _targetScript.Color3 = EditorGUILayout.ColorField(new GUIContent("Color3"),
                _targetScript.Color3, true, false, true);
            _targetScript.Balance =
                EditorGUILayout.Slider("Balance", _targetScript.Balance, 0, 1);
            _targetScript.X = EditorGUILayout.Toggle("X", _targetScript.X);
            _targetScript.Y = EditorGUILayout.Toggle("Y", _targetScript.Y);
        }

        if (_targetScript.Saturate)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Saturate", title);
            _targetScript.SaturateEnable =
                EditorGUILayout.Toggle("Saturate Enable", _targetScript.SaturateEnable);
            _targetScript.SaturateStr =
                EditorGUILayout.Slider("Saturate Str", _targetScript.SaturateStr, 0, 1);
        }
        
    }
}
