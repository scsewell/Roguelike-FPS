using System.Collections;
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
        InterpolationController.Instance.EarlyFixedUpdate();
        BulletManager.Instance.UpdateBullets();
    }

    public void LoadMainScene()
    {
        BulletManager.Instance.Clear();

        SceneManager.LoadScene(1);

        StartCoroutine(GenerateLevel());
    }

    private IEnumerator GenerateLevel()
    {
        yield return null;
        GetComponent<LevelGenerator>().GenerateLevel();
    }
}
