using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityStandardAssets.CinematicEffects;

public class CameraSettingsManager : MonoBehaviour
{
    private AntiAliasing m_antiAliasing;
    private AmbientOcclusion m_ambientOcclusion;
    private Bloom m_bloom;
    private CameraMotionBlur m_motionBlur;
    private TonemappingColorGrading m_tonemapping;
    private LensAberrations m_lensAberrations;
    
	private void Awake()
    {
        m_antiAliasing = GetComponent<AntiAliasing>();
        m_ambientOcclusion = GetComponent<AmbientOcclusion>();
        m_bloom = GetComponent<Bloom>();
        m_motionBlur = GetComponent<CameraMotionBlur>();
        m_tonemapping = GetComponent<TonemappingColorGrading>();
        m_lensAberrations = GetComponent<LensAberrations>();
    }

    private void Update()
    {
        m_antiAliasing.enabled = Settings.Instance.GetAntialiasing();
        m_ambientOcclusion.enabled = Settings.Instance.GetSSAO();
        m_bloom.enabled = Settings.Instance.GetBloom();
        m_motionBlur.enabled = Settings.Instance.GetMotionBlur();

        TonemappingColorGrading.ColorGradingSettings colorGrading = m_tonemapping.colorGrading;
        colorGrading.basics.value = (Settings.Instance.GetBrightness() / 2) + 0.5f;
        m_tonemapping.colorGrading = colorGrading;
    }
}
