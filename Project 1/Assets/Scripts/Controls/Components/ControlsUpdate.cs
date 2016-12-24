using UnityEngine;

[RequireComponent(typeof(ControlsEarlyUpdate))]
public class ControlsUpdate : MonoBehaviour
{
    private void Awake()
    {
       Controls.Instance.Load();
    }
    
    private void FixedUpdate()
    {
        Controls.Instance.FixedUpdate();
    }

    public void EarlyUpdate()
    {
        Controls.Instance.EarlyUpdate();
    }

    public void Update()
    {
        Controls.Instance.Update();
    }
}
