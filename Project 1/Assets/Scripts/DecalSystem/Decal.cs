using UnityEngine;

public class Decal : MonoBehaviour
{
	public Material material;
	public Sprite sprite;

	public float maxAngle = 80.0f;
	public float pushDistance = 0.005f;
	
	private void OnDrawGizmosSelected()
    {
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}

    public void BuildDecal(GameObject target)
    {
        Mesh mesh = DecalBuilder.BuildDecalForObject(this, target);
        if (mesh != null)
        {
            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            filter.mesh = mesh;
            Renderer renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}