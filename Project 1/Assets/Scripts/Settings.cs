using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class Settings
{
    // limits on values
    public const float MIN_FOV = 40.0f;
    public const float MAX_FOV = 80.0f;
    public const float MIN_BRIGHTNESS = 0f;
    public const float MAX_BRIGHTNESS = 2f;
    public const float MIN_SHADOW_DISTANCE = 0f;
    public const float MAX_SHADOW_DISTANCE = 40f;
    public static int[] TARGET_FRAME_RATES = { 10, 30, 60, 120, 144, 500 };
    public enum TextureResolution { LOW, MEDIUM, HIGH };
    public enum ShadowQualityLevels { OFF, LOW, MEDIUM, HIGH, ULTRA };

    // default values
    private const bool DEF_FULLSCREEN = true;
    private const bool DEF_VSYNC = true;
    private const bool DEF_AA = true;
    private const bool DEF_BLOOM = true;
    private const bool DEF_MOTION_BLUR = true;
    private const bool DEF_SSAO = true;
    private const bool DEF_SHOW_FPS = false;
    private const int DEF_FRAMERATE = 60;
    private const float DEF_FOV = 65;
    private const float DEF_BRIGHTNESS = 1;
    private const float DEF_SHADOW_DISTANCE = 20;
    private const float DEF_VOLUME = 1;
    private const TextureResolution DEF_TEXTURE_RES = TextureResolution.HIGH;
    private const ShadowQualityLevels DEF_SHADOW_QUALITY = ShadowQualityLevels.HIGH;


    // settings
    private bool m_fullscreen;
    public bool GetFullscreen()
    {
        return m_fullscreen;
    }
    public void SetFullscreen(bool value)
    {
        m_fullscreen = value;
    }
    
    private bool m_showFPS;
    public bool GetShowFPS()
    {
        return m_showFPS;
    }
    public void SetShowFPS(bool value)
    {
        m_showFPS = value;
    }

    private bool m_antialiasing;
    public bool GetAntialiasing()
    {
        return m_antialiasing;
    }
    public void SetAntialiasing(bool value)
    {
        m_antialiasing = value;
    }

    private bool m_vsync;
    public bool GetVsync()
    {
        return m_vsync;
    }
    public void SetVsync(bool value)
    {
        m_vsync = value;
    }

    private bool m_bloom;
    public bool GetBloom()
    {
        return m_bloom;
    }
    public void SetBloom(bool value)
    {
        m_bloom = value;
    }

    private bool m_motionBlur;
    public bool GetMotionBlur()
    {
        return m_motionBlur;
    }
    public void SetMotionBlur(bool value)
    {
        m_motionBlur = value;
    }

    private bool m_SSAO;
    public bool GetSSAO()
    {
        return m_SSAO;
    }
    public void SetSSAO(bool value)
    {
        m_SSAO = value;
    }

    private float m_fieldOfView;
    public float GetFieldOfView()
    {
        return m_fieldOfView;
    }
    public void SetFieldOfView(float value)
    {
        m_fieldOfView = value;
    }

    private float m_brightness;
    public float GetBrightness()
    {
        return m_brightness;
    }
    public void SetBrightness(float value)
    {
        m_brightness = value;
    }

    private float m_shadowDistance;
    public float GetShadowDistance()
    {
        return m_shadowDistance;
    }
    public void SetShadowDistance(float value)
    {
        m_shadowDistance = value;
    }

    private float m_volume;
    public float GetVolume()
    {
        return m_volume;
    }
    public void SetVolume(float value)
    {
        m_volume = value;
    }

    private int m_frameRate;
    public string GetFrameRate()
    {
        return m_frameRate.ToString();
    }
    public void SetFrameRate(string value)
    {
        m_frameRate = int.Parse(value);
    }

    private TextureResolution m_textureRes;
    public string GetTextureResolution()
    {
        return m_textureRes.ToString();
    }
    public void SetTextureResolution(string value)
    {
        m_textureRes = (TextureResolution)Enum.Parse(typeof(TextureResolution), value);
    }

    private ShadowQualityLevels m_shadowQuality;
    public string GetShadowQuality()
    {
        return m_shadowQuality.ToString();
    }
    public void SetShadowQuality(string value)
    {
        m_shadowQuality = (ShadowQualityLevels)Enum.Parse(typeof(ShadowQualityLevels), value);
    }

    private Resolution m_resolution;
    public string GetResolution()
    {
        return m_resolution.width + " x " + m_resolution.height;
    }
    public void SetResolution(string val) // string in the form of "1920 x 1080"
    {
        string[] split = val.Split('x');
        m_resolution.width = int.Parse(split[0].Trim());
        m_resolution.height = int.Parse(split[1].Trim());
    }

    public static string[] GetSupportedResolutions()
    {
        List<string> resolutions = new List<string>();
        foreach (Resolution res in Screen.resolutions)
        {
            string resolution = res.width + " x " + res.height;
            if (!resolutions.Contains(resolution))
            {
                resolutions.Add(resolution);
            }
        }
        resolutions.Reverse();
        return resolutions.ToArray();
    }


    private static Settings m_instance = new Settings();
    public static Settings Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    public Settings()
    {
        LoadDefaults();
    }

	public void Apply()
    {
        Application.targetFrameRate = m_frameRate;

        if (m_resolution.width != Screen.currentResolution.width || m_resolution.height != Screen.currentResolution.height || m_fullscreen != Screen.fullScreen)
        {
            Screen.SetResolution(m_resolution.width, m_resolution.height, m_fullscreen, m_frameRate);
        }

        QualitySettings.SetQualityLevel((int)m_shadowQuality);
        QualitySettings.masterTextureLimit = (Enum.GetValues(typeof(TextureResolution)).Cast<int>().Max() - (int)m_textureRes);
        QualitySettings.vSyncCount = (m_vsync ? 1 : 0);
        QualitySettings.shadowDistance = m_shadowDistance;

        AudioListener.volume = m_volume;
    }

    public void LoadDefaults()
    {
        m_fullscreen        = DEF_FULLSCREEN;
        m_showFPS           = DEF_SHOW_FPS;
        m_antialiasing      = DEF_AA;
        m_vsync             = DEF_VSYNC;
        m_bloom             = DEF_BLOOM;
        m_motionBlur        = DEF_MOTION_BLUR;
        m_SSAO              = DEF_SSAO;
        m_frameRate         = DEF_FRAMERATE;
        m_fieldOfView       = DEF_FOV;
        m_brightness        = DEF_BRIGHTNESS;
        m_shadowDistance    = DEF_SHADOW_DISTANCE;
        m_volume            = DEF_VOLUME;
        m_textureRes        = DEF_TEXTURE_RES;
        m_shadowQuality     = DEF_SHADOW_QUALITY;

        m_resolution = Screen.resolutions[Screen.resolutions.Length - 1];
    }

    public void Save()
    {
        FileIO.WriteSettings(m_instance);
    }
	
	public void Load()
    {
        m_instance = FileIO.ReadSettings();
    }
}