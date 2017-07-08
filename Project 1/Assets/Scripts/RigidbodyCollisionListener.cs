using UnityEngine;

public class RigidbodyCollisionListener : MonoBehaviour
{
    public delegate void CollisionHandler(Collision collision);
    public event CollisionHandler OnCollision;

    private void OnCollisionEnter(Collision collision)
    {
        if (OnCollision != null)
        {
            OnCollision(collision);
        }
    }
}