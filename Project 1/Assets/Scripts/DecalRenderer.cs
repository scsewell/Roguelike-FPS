using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class DecalSystem
{
    private static DecalSystem m_instance;
    public static DecalSystem Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new DecalSystem();
            }
            return m_instance;
        }
    }

    internal HashSet<Decal> m_decals = new HashSet<Decal>();

    public void AddDecal(Decal decal)
    {
        RemoveDecal(decal);
        m_decals.Add(decal);
    }

    public void RemoveDecal(Decal decal)
    {
        m_decals.Remove(decal);
    }
}

[ExecuteInEditMode]
public class DecalRenderer : MonoBehaviour
{
    public Mesh cubeMesh;

    private Dictionary<Camera, CommandBuffer> m_cameraCommands = new Dictionary<Camera, CommandBuffer>();

    public void OnDisable()
    {
        foreach (KeyValuePair<Camera, CommandBuffer> cam in m_cameraCommands)
        {
            if (cam.Key)
            {
                cam.Key.RemoveCommandBuffer(CameraEvent.BeforeLighting, cam.Value);
            }
        }
    }

    public void LateUpdate()
    {
        Camera cam = Camera.current;
        if (!cam)
        {
            return;
        }

        CommandBuffer buffer = null;
        if (m_cameraCommands.ContainsKey(cam))
        {
            buffer = m_cameraCommands[cam];
            buffer.Clear();
        }
        else
        {
            buffer = new CommandBuffer();
            buffer.name = "Decals";
            m_cameraCommands[cam] = buffer;

            // set this command buffer to be executed just before deferred lighting pass
            cam.AddCommandBuffer(CameraEvent.BeforeLighting, buffer);
        }
        
        DecalSystem system = DecalSystem.Instance;

        // if there are no decals, no need to continue
        if (system.m_decals.Count == 0)
        {
            return;
        }

        // copy g-buffer normals into a temporary render texture
        int normalsID = Shader.PropertyToID("_NormalsCopy");
        buffer.GetTemporaryRT(normalsID, -1, -1);
        buffer.Blit(BuiltinRenderTextureType.GBuffer2, normalsID);

        // render decals into diffuse channel
        RenderTargetIdentifier[] renderTargets = { BuiltinRenderTextureType.GBuffer0 };
        buffer.SetRenderTarget(renderTargets, BuiltinRenderTextureType.CameraTarget);

        // render visible decals
        Plane[] clippingPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
        foreach (Decal decal in system.m_decals)
        {
            if (GeometryUtility.TestPlanesAABB(clippingPlanes, decal.GetBounds()))
            {
                buffer.DrawMesh(cubeMesh, decal.transform.localToWorldMatrix, decal.m_material);
            }
        }

        // release temporary normals render texture
        buffer.ReleaseTemporaryRT(normalsID);
    }
}
