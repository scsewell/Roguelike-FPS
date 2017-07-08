using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraSettingsManager : MonoBehaviour
{
    [SerializeField]
    private bool m_duplicateProfile = true;

    private PostProcessingProfile m_postProfile;

    private void Awake()
    {
        PostProcessingBehaviour post = GetComponent<PostProcessingBehaviour>();

        if (m_duplicateProfile)
        {
            m_postProfile = Instantiate(post.profile);
            post.profile = m_postProfile;
        }
        else
        {
            m_postProfile = post.profile;
        }
    }

    private void Update()
    {
        m_postProfile.antialiasing.enabled = SettingManager.Instance.UseAntialiasing;
        m_postProfile.ambientOcclusion.enabled = SettingManager.Instance.UseSSAO;
        m_postProfile.bloom.enabled = SettingManager.Instance.UseBloom;
        m_postProfile.motionBlur.enabled = SettingManager.Instance.UseMotionBlur;

        ColorGradingModel.Settings settings = m_postProfile.colorGrading.settings;
        ColorGradingModel.BasicSettings basicSettings = settings.basic;
        basicSettings.postExposure = SettingManager.Instance.Brightness - 1;
        settings.basic = basicSettings;
        m_postProfile.colorGrading.settings = settings;
    }
}