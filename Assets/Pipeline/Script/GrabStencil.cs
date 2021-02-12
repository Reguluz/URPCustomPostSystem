using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class GrabStencil : MonoBehaviour
{
    private RenderTexture buffer;
    private static int STATIC_IDMASK = Shader.PropertyToID("_IDMask");

    private void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        buffer = RenderTexture.GetTemporary(src.width, src.height, 24);
        Graphics.SetRenderTarget(buffer.colorBuffer, src.depthBuffer);
        Graphics.Blit(buffer,dest);
        Shader.SetGlobalTexture(STATIC_IDMASK, dest);
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        RenderTexture.ReleaseTemporary(buffer);
    }

}
