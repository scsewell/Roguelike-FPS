using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
    [SerializeField]
    private OutlineEffect.OutlineType m_outlineType = OutlineEffect.OutlineType.Color1;
    public OutlineEffect.OutlineType OutlineType
    {
        get { return m_outlineType; }
    }

    private Renderer m_renderer;
    public Renderer Renderer
    {
        get { return m_renderer; }
    }

    private OutlineEffect m_outlineEffect;

    private void Awake()
    {
        m_renderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
		if (m_outlineEffect == null)
        {
			m_outlineEffect = Camera.main.GetComponent<OutlineEffect>();
        }
		m_outlineEffect.AddOutline(this);
    }

    private void OnDisable()
    {
        m_outlineEffect.RemoveOutline(this);
    }
}
