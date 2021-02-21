using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FrustumDeliver : MonoBehaviour
{
    // public Material mat;
    private Camera cam;

    public Matrix4x4 frustumDir;

    public GameObject RainSource;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
        // float halfHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView/2);
        // float halfWidth = halfHeight * cam.aspect;
        // Vector3 toTop = transform.up * halfHeight;
        // Vector3 toRight = transform.right * halfWidth;
        //
        // Vector3 toTopLeft = transform.forward + toTop - toRight;
        // Vector3 toBottomLeft = transform.forward - toTop - toRight;
        // Vector3 toTopRight = transform.forward + toTop + toRight;
        // Vector3 toBottomRight = transform.forward - toTop + toRight;
        //
        // toTopLeft /= cam.nearClipPlane;
        // toBottomLeft /= cam.nearClipPlane;
        // toTopRight /= cam.nearClipPlane;
        // toBottomRight /= cam.nearClipPlane;
        // frustumDir = Matrix4x4.identity;
        // frustumDir.SetRow(0, toBottomLeft);
        // frustumDir.SetRow(1, toBottomRight);
        // frustumDir.SetRow(2, toTopLeft);
        // frustumDir.SetRow(3, toTopRight);
        
        var aspect = cam.aspect;
        var far = cam.farClipPlane;
        var right = transform.right;
        var up = transform.up;
        var forward = transform.forward;
        var halfFovTan = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
 
        //计算相机在远裁剪面处的xyz三方向向量
        var rightVec = right * far * halfFovTan * aspect;
        var upVec = up * far * halfFovTan;
        var forwardVec = forward * far;
 
        //构建四个角的方向向量
        var topLeft = (forwardVec - rightVec + upVec);
        var topRight = (forwardVec + rightVec + upVec);
        var bottomLeft = (forwardVec - rightVec - upVec);
        var bottomRight = (forwardVec + rightVec - upVec);
 
        var viewPortRay = Matrix4x4.identity;
        viewPortRay.SetRow(0, topLeft);
        viewPortRay.SetRow(1, topRight);
        viewPortRay.SetRow(2, bottomLeft);
        viewPortRay.SetRow(3, bottomRight);
        
        Shader.SetGlobalMatrix("_FrustumDir", viewPortRay);
        Shader.SetGlobalVector("_CameraForward", transform.forward);

        Vector3 t;
        if(!RainSource)t = Vector3.one;
        else
        {
            t = -RainSource.transform.forward;
        }
        float cosTheta = Vector3.Dot(transform.up, t);
        Vector3 p = t - t * cosTheta;
        Vector3 e = transform.right * Vector3.Dot(p, transform.right);
        float tanAlpha = Vector3.Magnitude(e) / Vector3.Magnitude(t * cosTheta);
        if (Vector3.Dot(e, transform.right) < 0)
        {
            tanAlpha = -tanAlpha;
        }
        Shader.SetGlobalFloat("_RainDropAngle", Mathf.Atan(tanAlpha) / Mathf.PI * 180);
    }
}
