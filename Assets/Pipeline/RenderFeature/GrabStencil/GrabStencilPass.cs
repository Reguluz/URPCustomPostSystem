using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GrabStencilPass : ScriptableRenderPass
{
    public Material blitMaterial = null;
    private RenderTargetIdentifier source { get; set; }
    private RenderTextureDescriptor descriptor { get; set; }
    FilteringSettings m_FilteringSettings;
    const string m_ProfilerTag = "Grab Stencil";
    private Vector2Int widthHeight;
    private static int STATIC_IDMASK = Shader.PropertyToID("_IDMaskGrab");
    public GrabStencilPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask)
    {
        this.renderPassEvent = evt;
        m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
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
        cmd.GetTemporaryRT(STATIC_IDMASK, widthHeight.x,widthHeight.y,0,FilterMode.Bilinear,RenderTextureFormat.Default,RenderTextureReadWrite.Default);
        cmd.Blit(source,STATIC_IDMASK);
        cmd.SetGlobalTexture("_IDMask", STATIC_IDMASK);
        cmd.Blit(STATIC_IDMASK, source);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
    }
}
