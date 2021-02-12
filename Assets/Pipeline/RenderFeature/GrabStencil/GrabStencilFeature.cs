using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GrabStencilFeature : ScriptableRendererFeature
{
    GrabStencilPass pass;
    public GrabStencilFeatureSettings featureSettings = new GrabStencilFeatureSettings();
    [System.Serializable]
    public class GrabStencilFeatureSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        public RenderQueueRange renderQueueRange = RenderQueueRange.opaque;
        public LayerMask layerMask;
        public Material blitMaterial = null;
    }
    public override void Create()
    {
        pass = new GrabStencilPass(featureSettings.renderPassEvent, featureSettings.renderQueueRange, featureSettings.layerMask);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        var descriptor = renderingData.cameraData.cameraTargetDescriptor;
        pass.Setup(src, descriptor);
        renderer.EnqueuePass(pass);
    }
}