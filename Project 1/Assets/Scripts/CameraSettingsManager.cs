using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityStandardAssets.CinematicEffects;

public class CameraSettingsManager : MonoBehaviour
{
    private AntiAliasing m_antiAliasing;
    private AmbientOcclusion m_ambientOcclusion;
    private UnityStandardAssets.CinematicEffects.Bloom m_bloom;
    private CameraMotionBlur m_motionBlur;
    private TonemappingColorGrading m_tonemapping;
    private LensAberrations m_lensAberrations;

    private Settings m_settings;

	private void Awake()
    {
        m_settings = GameObject.FindGameObjectWithTag("GameController").GetComponent<Settings>();

        m_antiAliasing = GetComponent<AntiAliasing>();
        m_ambientOcclusion = GetComponent<AmbientOcclusion>();
        m_bloom = GetComponent<UnityStandardAssets.CinematicEffects.Bloom>();
        m_motionBlur = GetComponent<CameraMotionBlur>();
        m_tonemapping = GetComponent<TonemappingColorGrading>();
        m_lensAberrations = GetComponent<LensAberrations>();
    }

    private void Update()
    {
        m_antiAliasing.enabled = m_settings.GetAntialiasing();
        m_ambientOcclusion.enabled = m_settings.GetSSAO();
        m_bloom.enabled = m_settings.GetBloom();
        m_motionBlur.enabled = m_settings.GetMotionBlur();

        TonemappingColorGrading.ColorGradingSettings colorGrading = m_tonemapping.colorGrading;
        colorGrading.basics.value = (m_settings.GetBrightness() / 2) + 0.5f;
        m_tonemapping.colorGrading = colorGrading;
    }
}
