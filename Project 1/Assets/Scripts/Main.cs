using UnityEngine;
using UnityEngine.SceneManagement;
using Framework;
using Framework.Interpolation;

public class Main : ComponentSingleton<Main>
{
    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        SettingManager.Instance.Load();
        SettingManager.Instance.Apply();

        gameObject.AddComponent<ControlsManager>();
        ControlsManager.Instance.Load();
    }

    private void Update()
    {
        ControlsManager.Instance.EarlyUpdate();
        InterpolationController.Instance.VisualUpdate();
    }

    private void FixedUpdate()
    {
        InterpolationController.Instance.MainUpdate();
        BulletManager.Instance.UpdateBullets();
    }

    public void LoadMainScene()
    {
        BulletManager.Instance.DeactivateAll();

        SceneManager.LoadScene(1);
    }
}
