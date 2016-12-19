/*
//  Copyright (c) 2015 JosÃ© Guerreiro. All rights reserved.
//
//  MIT license, see http://www.opensource.org/licenses/mit-license.php
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
*/

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class OutlineEffect : MonoBehaviour
{
    [SerializeField] private Shader m_outlineShader;
    [SerializeField] private Shader m_outlineBufferShader;

    [Range(0, 4)]
    public float lineThickness = 4f;
    [Range(0, 10)]
    public float lineIntensity = 0.5f;
    [Range(0, 1)]
    public float fillAmount = 0.2f;

    public Color lineColor0 = Color.red;
    public Color lineColor1 = Color.green;
    public Color lineColor2 = Color.blue;

    [Range(0, 1)]
    public float alphaCutoff = 0.5f;
    public bool additiveRendering = true;
    [Header("These settings can affect performance!")]
    public bool cornerOutlines = false;
    public bool addLinesBetweenColors = false;

    private List<Outline> m_outlines = new List<Outline>();

    private Camera m_mainCam;
    private Camera m_outlineCam;

    private Material m_outline1Material;
    private Material m_outline2Material;
    private Material m_outline3Material;
    private Material m_outlineEraseMaterial;
    private Material m_outlineShaderMaterial;
    private RenderTexture m_renderTexture;
    private RenderTexture m_extraRenderTexture;


    private void Start()
    {
        CreateMaterialsIfNeeded();
        UpdateMaterialsPublicProperties();
        
        m_mainCam = GetComponent<Camera>();

        GameObject cameraGameObject = new GameObject("OutlineCamera");
        cameraGameObject.transform.SetParent(m_mainCam.transform, false);
        m_outlineCam = cameraGameObject.AddComponent<Camera>();

		m_renderTexture = new RenderTexture(m_mainCam.pixelWidth, m_mainCam.pixelHeight, 16, RenderTextureFormat.Default);
        m_extraRenderTexture = new RenderTexture(m_mainCam.pixelWidth, m_mainCam.pixelHeight, 16, RenderTextureFormat.Default);
        UpdateOutlineCameraFromSource();
    }

    private void OnDestroy()
    {
        m_renderTexture.Release();
        m_extraRenderTexture.Release();
        DestroyMaterials();
    }

    private void OnPreCull()
    {
		if (m_renderTexture.width != m_mainCam.pixelWidth || m_renderTexture.height != m_mainCam.pixelHeight)
		{
			m_renderTexture = new RenderTexture(m_mainCam.pixelWidth, m_mainCam.pixelHeight, 16, RenderTextureFormat.Default);
            m_extraRenderTexture = new RenderTexture(m_mainCam.pixelWidth, m_mainCam.pixelHeight, 16, RenderTextureFormat.Default);
            m_outlineCam.targetTexture = m_renderTexture;
		}
		UpdateMaterialsPublicProperties();
		UpdateOutlineCameraFromSource();

		if (m_outlines != null)
        {
			for (int i = 0; i < m_outlines.Count; i++)
            {
                if (m_outlines[i] != null)
                {
                    m_outlines[i].originalMaterial = m_outlines[i].GetComponent<Renderer>().sharedMaterial;
                    m_outlines[i].originalLayer = m_outlines[i].gameObject.layer;

                    if (m_outlines[i].blockOutlines)
                    {
                        m_outlines[i].GetComponent<Renderer>().sharedMaterial = m_outlineEraseMaterial;
                    }
                    else
                    {
                        m_outlines[i].GetComponent<Renderer>().sharedMaterial = GetMaterialFromID(m_outlines[i].color);
                    }

                    if (m_outlines[i].GetComponent<Renderer>() is MeshRenderer)
                    {
                        m_outlines[i].GetComponent<Renderer>().sharedMaterial.mainTexture = m_outlines[i].originalMaterial.mainTexture;
                    }

                    m_outlines[i].gameObject.layer = LayerMask.NameToLayer("Outline");
                }
            }
        }

        m_outlineCam.Render();

        if (m_outlines != null)
        {
            for (int i = 0; i < m_outlines.Count; i++)
            {
                if (m_outlines[i] != null)
                {
                    if (m_outlines[i].GetComponent<Renderer>() is MeshRenderer)
                    {
						m_outlines[i].GetComponent<Renderer>().sharedMaterial.mainTexture = null;
                    }

					m_outlines[i].GetComponent<Renderer>().sharedMaterial = m_outlines[i].originalMaterial;
					m_outlines[i].gameObject.layer = m_outlines[i].originalLayer;
                }
            }
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        m_outlineShaderMaterial.SetTexture("_OutlineSource", m_renderTexture);
        if (addLinesBetweenColors)
        {
            Graphics.Blit(source, m_extraRenderTexture, m_outlineShaderMaterial, 0);
            m_outlineShaderMaterial.SetTexture("_OutlineSource", m_extraRenderTexture);
        }
        Graphics.Blit(source, destination, m_outlineShaderMaterial, 1);
    }

    private void CreateMaterialsIfNeeded()
    {
        if (m_outlineShaderMaterial == null)
        {
            m_outlineShaderMaterial = new Material(m_outlineShader);
            m_outlineShaderMaterial.hideFlags = HideFlags.HideAndDontSave;
            UpdateMaterialsPublicProperties();
        }
        if (m_outlineEraseMaterial == null)
        {
            m_outlineEraseMaterial = CreateMaterial(new Color(0, 0, 0, 1));
        }
        if (m_outline1Material == null)
        {
            m_outline1Material = CreateMaterial(new Color(1, 0, 0, 1));
        }
        if (m_outline2Material == null)
        {
            m_outline2Material = CreateMaterial(new Color(0, 1, 0, 1));
        }
        if (m_outline3Material == null)
        {
            m_outline3Material = CreateMaterial(new Color(0, 0, 1, 1));
        }
    }

    private void DestroyMaterials()
    {
        DestroyImmediate(m_outlineShaderMaterial);
        DestroyImmediate(m_outlineEraseMaterial);
        DestroyImmediate(m_outline1Material);
        DestroyImmediate(m_outline2Material);
        DestroyImmediate(m_outline3Material);
        m_outlineShader = null;
        m_outlineBufferShader = null;
        m_outlineShaderMaterial = null;
        m_outlineEraseMaterial = null;
        m_outline1Material = null;
        m_outline2Material = null;
        m_outline3Material = null;
    }

    private void UpdateMaterialsPublicProperties()
    {
        if (m_outlineShaderMaterial)
        {
            m_outlineShaderMaterial.SetFloat("_LineThicknessX", lineThickness / 1000);
            m_outlineShaderMaterial.SetFloat("_LineThicknessY", lineThickness / 1000);
            m_outlineShaderMaterial.SetFloat("_LineIntensity", lineIntensity);
            m_outlineShaderMaterial.SetFloat("_FillAmount", fillAmount);
            m_outlineShaderMaterial.SetColor("_LineColor1", lineColor0);
            m_outlineShaderMaterial.SetColor("_LineColor2", lineColor1);
            m_outlineShaderMaterial.SetColor("_LineColor3", lineColor2);
            m_outlineShaderMaterial.SetInt("_CornerOutlines", cornerOutlines ? 1 : 0);

            Shader.SetGlobalFloat("_OutlineAlphaCutoff", alphaCutoff);
        }
    }

    private void UpdateOutlineCameraFromSource()
    {
        m_outlineCam.CopyFrom(m_mainCam);
        m_outlineCam.renderingPath = RenderingPath.Forward;
        m_outlineCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
        m_outlineCam.clearFlags = CameraClearFlags.SolidColor;
        m_outlineCam.cullingMask = LayerMask.GetMask("Outline");
        m_outlineCam.rect = new Rect(0, 0, 1, 1);
		m_outlineCam.enabled = true;
		m_outlineCam.targetTexture = m_renderTexture;
    }

    private Material GetMaterialFromID(int id)
    {
        if (id == 0)
        {
            return m_outline1Material;
        }
        else if (id == 1)
        {
            return m_outline2Material;
        }
        else
        {
            return m_outline3Material;
        }
    }

    private Material CreateMaterial(Color emissionColor)
    {
        Material m = new Material(m_outlineBufferShader);
        m.SetColor("_Color", emissionColor);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = 3000;
        return m;
    }

    public void AddOutline(Outline outline)
    {
        if (!m_outlines.Contains(outline))
        {
			m_outlines.Add(outline);
        }
    }

    public void RemoveOutline(Outline outline)
	{
		m_outlines.Remove(outline);
    }
}
