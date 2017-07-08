using UnityEngine;
using Framework.Interpolation;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        SettingManager.Instance.Load();
        SettingManager.Instance.Apply();

        gameObject.AddComponent<ControlsManager>();
        ControlsManager.Instance.Load();

        gameObject.AddComponent<InterpolationController>();
    }
    
    private void Update()
    {
        ControlsManager.Instance.EarlyUpdate();
    }
}
