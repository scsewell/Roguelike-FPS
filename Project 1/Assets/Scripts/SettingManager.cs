using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Framework.SettingManagement;
using Framework.IO;

public class SettingManager : Singleton<SettingManager>
{
    public enum TextureResolution
    {
        Low,
        Medium,
        High,
    };

    public enum ShadowQualityLevels
    {
        Off,
        Low,
        Medium,
        High,
        Ultra,
    };

    public struct Categories
    {
        public static readonly string Screen = "Screen";
        public static readonly string Quality = "Quality";
        public static readonly string Audio = "Audio";
        public static readonly string Other = "Other";
        public static readonly string Controls = "Controls";
    }

    private Settings m_settings;
    public Settings Settings
    {
        get { return m_settings; }
    }

    private Setting<float> m_fov;
    public float FieldOfView
    {
        get { return m_fov.Value; }
    }

    private Setting<float> m_brightness;
    public float Brightness
    {
        get { return m_brightness.Value; }
    }

    private Setting<bool> m_useAntialiasing;
    public bool UseAntialiasing
    {
        get { return m_useAntialiasing.Value; }
    }

    private Setting<bool> m_useSSAO;
    public bool UseSSAO
    {
        get { return m_useSSAO.Value; }
    }

    private Setting<bool> m_useBloom;
    public bool UseBloom
    {
        get { return m_useBloom.Value; }
    }

    private Setting<bool> m_useMotionBlur;
    public bool UseMotionBlur
    {
        get { return m_useMotionBlur.Value; }
    }

    private Setting<bool> m_showFPS;
    public bool ShowFPS
    {
        get { return m_showFPS.Value; }
    }

    public SettingManager()
    {
        m_settings = new Settings();

        m_settings.Add(Categories.Screen,"Resolution", Screen.currentResolution,
            GetSupportedResolutions(),
            (v) => SerializeResolution(v),
            (s) => ParseResolution(s),
            (v) => Screen.SetResolution(v.width, v.height, Screen.fullScreen)
            );

        m_settings.Add(Categories.Screen, "Fullscreen", true, (v) => Screen.fullScreen = v);

        m_settings.Add(Categories.Screen, "Target Frame Rate", 60,
            (new int[] { 10, 30, 60, 120, 144, 500 }).Select(x => x.ToString()).ToArray(),
            (v) => v.ToString(),
            (s) => int.Parse(s),
            (v) => Application.targetFrameRate = v
            );

        m_settings.Add(Categories.Screen, "VSync", true, (v) => QualitySettings.vSyncCount = (v ? 1 : 0));
        m_brightness = m_settings.Add(Categories.Screen, "Brightness", 1, 0, 2, false);

        m_settings.Add(Categories.Quality, "Shadow Quality", ShadowQualityLevels.High,
            Enum.GetNames(typeof(ShadowQualityLevels)),
            (v) => v.ToString(),
            (s) => (ShadowQualityLevels)Enum.Parse(typeof(ShadowQualityLevels), s),
            (v) => QualitySettings.SetQualityLevel((int)v)
            );

        m_settings.Add(Categories.Quality, "Texture Resolution", TextureResolution.High,
            Enum.GetNames(typeof(TextureResolution)),
            (v) => v.ToString(),
            (s) => (TextureResolution)Enum.Parse(typeof(TextureResolution), s),
            (v) => QualitySettings.masterTextureLimit = 2 - (int)v
            );

        m_useAntialiasing =  m_settings.Add(Categories.Quality, "Antialiasing", true);
        m_useSSAO = m_settings.Add(Categories.Quality, "SSAO", true);
        m_useBloom = m_settings.Add(Categories.Quality, "Bloom", true);
        m_useMotionBlur = m_settings.Add(Categories.Quality, "Motion Blur", true);

        m_settings.Add(Categories.Audio, "Volume", 1, 0, 1, false, (v) => AudioListener.volume = v);

        m_fov = m_settings.Add(Categories.Other, "Field Of View", 65, 40, 80, true);
        m_showFPS = m_settings.Add(Categories.Other, "Show FPS", false);
    }
    
    public void UseDefaults()
    {
        m_settings.UseDefaults();
    }

    public void Apply()
    {
        m_settings.Apply();
    }

    public void Save()
    {
        FileIO.WriteFile(JsonConverter.ToJson(m_settings.Serialize()), FileIO.GetInstallDirectory(), "Settings.ini");
    }

    public void Load()
    {
        string str = FileIO.ReadFile(FileIO.GetInstallDirectory(), "Settings.ini");
        if (str == null || !m_settings.Deserialize(JsonConverter.FromJson<SerializableSettings>(str)))
        {
            m_settings.UseDefaults();
            Save();
        }
    }

    private string SerializeResolution(Resolution res)
    {
        return res.width + " x " + res.height;
    }

    private Resolution ParseResolution(string s)
    {
        Resolution res = new Resolution();
        string[] split = s.Split('x');
        res.width = int.Parse(split[0].Trim());
        res.height = int.Parse(split[1].Trim());
        return res;
    }

    private string[] GetSupportedResolutions()
    {
        List<string> resolutions = new List<string>();
        foreach (Resolution res in Screen.resolutions)
        {
            string resolution = SerializeResolution(res);
            if (!resolutions.Contains(resolution))
            {
                resolutions.Add(resolution);
            }
        }
        resolutions.Reverse();
        return resolutions.ToArray();
    }
}