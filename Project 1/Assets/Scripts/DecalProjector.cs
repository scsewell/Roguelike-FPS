using UnityEngine;

[ExecuteInEditMode]
public class DecalProjector : MonoBehaviour
{
    public Material m_material;

    private Bounds m_bounds;

    public void Start()
    {
        m_bounds = new Bounds(transform.position, transform.lossyScale);

        DecalSystem.Instance.AddDecal(this);
    }

    public void OnEnable()
    {
        DecalSystem.Instance.AddDecal(this);
    }

    public void OnDisable()
    {
        DecalSystem.Instance.RemoveDecal(this);
    }

    public Bounds GetBounds()
    {
        return m_bounds;
    }

    public void OnDrawGizmos()
    {
        var col = new Color(0.0f, 0.7f, 1f, 0.1f);
        col.a = 0.1f;
        Gizmos.color = col;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        col.a = 0.2f;
        Gizmos.color = col;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
