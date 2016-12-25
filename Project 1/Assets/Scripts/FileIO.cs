using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class FileIO
{
    private static string CONTROLS_FILE_NAME = "ControlBindings.ini";
    private static string SETTINGS_FILE_NAME = "Settings.ini";
    
    private static string EDITOR_CONFIGS = "Configs/";
    
    public static void WriteControls(Controls controls)
    {
        WriteJsonFile(controls, CONTROLS_FILE_NAME);
    }

    public static Controls ReadControls()
    {
        return ParseJsonFile<Controls>(CONTROLS_FILE_NAME);
    }

    public static void WriteSettings(Settings settings)
    {
        WriteJsonFile(settings, SETTINGS_FILE_NAME);
    }

    public static Settings ReadSettings()
    {
        return ParseJsonFile<Settings>(SETTINGS_FILE_NAME);
    }

    private static void WriteJsonFile<T>(T obj, string fileName)
    {
        JsonSerializerSettings jss = new JsonSerializerSettings() { ContractResolver = new PrivateContractResolver(), TypeNameHandling = TypeNameHandling.Auto };
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented, jss);
        string directory = GetIniDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(directory + fileName, json);
    }

    private static T ParseJsonFile<T>(string fileName)
    {
        JsonSerializerSettings jss = new JsonSerializerSettings() { ContractResolver = new PrivateContractResolver(), TypeNameHandling = TypeNameHandling.Auto };
        string directory = GetIniDirectory();
        if (Directory.Exists(directory))
        {
            string json = File.ReadAllText(directory + fileName);
            return JsonConvert.DeserializeObject<T>(json, jss);
        }
        return default(T);
    }

    private static string GetIniDirectory()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.WindowsEditor: return EDITOR_CONFIGS;
            default: return "";
        }
    }
}
