using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
	[Range(0,2)]
    public int color;
	public bool blockOutlines;

	[HideInInspector]
	public int originalLayer;
	[HideInInspector]
	public Material originalMaterial;

    private OutlineEffect m_outlineEffect;

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
