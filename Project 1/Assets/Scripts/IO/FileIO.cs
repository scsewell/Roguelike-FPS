using UnityEngine;
using System.IO;

public static class FileIO
{
    private static string EDITOR_CONFIGS = "Configs/";
    private static string SETTINGS_FILE_NAME = "Settings.ini";
    private static string CONTROLS_FILE_NAME = "ControlBindings.ini"; 
    
    public static void WriteSettings(Settings settings)
    {
        WriteJsonFile(settings, GetIniDirectory(), SETTINGS_FILE_NAME);
    }

    public static Settings ReadSettings()
    {
        return ParseJsonFile<Settings>(GetIniDirectory(), SETTINGS_FILE_NAME);
    }

    public static void WriteControls(Controls controls)
    {
        WriteJsonFile(controls, GetIniDirectory(), CONTROLS_FILE_NAME);
    }

    public static Controls ReadControls()
    {
        return ParseJsonFile<Controls>(GetIniDirectory(), CONTROLS_FILE_NAME);
    }

    private static void WriteJsonFile<T>(T obj, string directory, string fileName)
    {
        Directory.CreateDirectory(directory);
        File.WriteAllText(directory + fileName, JsonConverter.ToJson(obj));
    }

    private static T ParseJsonFile<T>(string directory, string fileName) where T : new()
    {
        string fullPath = directory + fileName;
        if (Directory.Exists(directory) && File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            return JsonConverter.FromJson<T>(json);
        }
        return new T();
    }

    private static string GetIniDirectory()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.WindowsEditor: return EDITOR_CONFIGS;
            default: return Application.dataPath + "/../";
        }
    }
}
