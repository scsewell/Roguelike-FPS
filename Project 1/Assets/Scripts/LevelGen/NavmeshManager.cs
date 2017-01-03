using UnityEngine;
using UnityEngine.AI;

public class NavMeshManager : MonoBehaviour
{
    NavMeshSurface m_surface;

    private void Awake()
    {
        m_surface = gameObject.AddComponent<NavMeshSurface>();
        m_surface.collectObjects = CollectObjects.Children;
    }

    public void BuildNavMesh()
    {
        m_surface.Bake();
    }
}
