using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class OutlineEffect : MonoBehaviour
{
    [SerializeField]
    private Shader m_outlineShader;
    [SerializeField]
    private Shader m_outlinePostShader;

    [SerializeField] [Range(0, 4)]
    private float m_lineThickness = 2.0f;
    [SerializeField] [Range(0, 10)]
    private float m_lineIntensity = 1.0f;
    [SerializeField] [Range(0, 1)]
    private float m_fillAmount = 0.2f;

    [SerializeField]
    private Color m_lineColor0 = Color.red;
    [SerializeField]
    private Color m_lineColor1 = Color.green;
    [SerializeField]
    private Color m_lineColor2 = Color.blue;

    [SerializeField]
    private bool m_cornerOutlines = false;
    [SerializeField]
    private bool m_addLinesBetweenColors = false;

    public enum OutlineType
    {
        Block,
        Color1,
        Color2,
        Color3,
    }

    private static Dictionary<int, Material> m_outlineTypeToMat;
    
    private Camera m_cam;
    private Material m_outlineShaderMaterial;
    private RenderTexture m_renderTexture;
    private RenderTexture m_extraRenderTexture;
    private CommandBuffer m_outlineCommands;

    private List<Outline> m_outlines = new List<Outline>();

    private bool m_renderOutlines = false;
    private bool RenderOutlines
    {
        get { return m_renderOutlines; }
        set
        {
            if (m_renderOutlines != value)
            {
                if (value)
                {
                    m_cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, m_outlineCommands);
                }
                else
                {
                    m_cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, m_outlineCommands);
                }
                m_renderOutlines = value;
            }
        }
    }

    private void Awake()
    {
        if (m_outlineTypeToMat == null)
        {
            m_outlineTypeToMat = new Dictionary<int, Material>();
            m_outlineTypeToMat.Add((int)OutlineType.Block, CreateBufferMaterial(new Color(0, 0, 0, 1)));
            m_outlineTypeToMat.Add((int)OutlineType.Color1, CreateBufferMaterial(new Color(1, 0, 0, 1)));
            m_outlineTypeToMat.Add((int)OutlineType.Color2, CreateBufferMaterial(new Color(0, 1, 0, 1)));
            m_outlineTypeToMat.Add((int)OutlineType.Color3, CreateBufferMaterial(new Color(0, 0, 1, 1)));
        }

        m_outlineShaderMaterial = new Material(m_outlinePostShader);

        m_cam = GetComponent<Camera>();
        m_outlineCommands = new CommandBuffer();
    }

    private void OnEnable()
    {
        CreateRenderTextures();
    }

    private void OnDestroy()
    {
        ReleaseRenderTextures();
    }

    private void OnPreCull()
    {
        UpdateOulineBufferCommands();

        if (m_renderOutlines)
        {
            if (m_renderTexture.width != m_cam.pixelWidth || m_renderTexture.height != m_cam.pixelHeight)
            {
                CreateRenderTextures();
            }
        }
    }

    private void UpdateOulineBufferCommands()
    {
        m_outlineCommands.name = "BufferOutlines";

        m_outlineCommands.Clear();

        m_outlineCommands.SetRenderTarget(new RenderTargetIdentifier(m_renderTexture));
        m_outlineCommands.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));

        bool renderOutlines = false;
        foreach (Outline outline in m_outlines)
        {
            if (outline.Renderer.enabled)
            {
                if (outline.OutlineType != OutlineType.Block)
                {
                    renderOutlines = true;
                }
                m_outlineCommands.DrawRenderer(outline.Renderer, m_outlineTypeToMat[(int)outline.OutlineType]);
            }
        }

        RenderOutlines = renderOutlines;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (RenderOutlines)
        {
            UpdateOutlineMaterial();
            m_outlineShaderMaterial.SetTexture("_OutlineSource", m_renderTexture);
            if (m_addLinesBetweenColors)
            {
                Graphics.Blit(source, m_extraRenderTexture, m_outlineShaderMaterial, 0);
                m_outlineShaderMaterial.SetTexture("_OutlineSource", m_extraRenderTexture);
            }
            Graphics.Blit(source, destination, m_outlineShaderMaterial, 1);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void CreateRenderTextures()
    {
        ReleaseRenderTextures();

        m_renderTexture = new RenderTexture(m_cam.pixelWidth, m_cam.pixelHeight, 16, RenderTextureFormat.Default);
        m_extraRenderTexture = new RenderTexture(m_cam.pixelWidth, m_cam.pixelHeight, 16, RenderTextureFormat.Default);
    }

    private void ReleaseRenderTextures()
    {
        if (m_renderTexture != null)
        {
            m_renderTexture.Release();
            m_renderTexture = null;
        }
        if (m_extraRenderTexture != null)
        {
            m_extraRenderTexture.Release();
            m_extraRenderTexture = null;
        }
    }

    private void UpdateOutlineMaterial()
    {
        m_outlineShaderMaterial.SetFloat("_LineThickness", 0.001f * m_lineThickness);
        m_outlineShaderMaterial.SetFloat("_LineIntensity", m_lineIntensity);
        m_outlineShaderMaterial.SetFloat("_FillAmount", m_fillAmount);
        m_outlineShaderMaterial.SetColor("_LineColor1", m_lineColor0);
        m_outlineShaderMaterial.SetColor("_LineColor2", m_lineColor1);
        m_outlineShaderMaterial.SetColor("_LineColor3", m_lineColor2);

        if (m_cornerOutlines)
        {
            m_outlineShaderMaterial.EnableKeyword("CORNER_OUTLINES");
        }
        else
        {
            m_outlineShaderMaterial.DisableKeyword("CORNER_OUTLINES");
        }
    }
    
    private Material CreateBufferMaterial(Color emissionColor)
    {
        Material m = new Material(m_outlineShader);
        m.SetColor("_Color", emissionColor);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
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
