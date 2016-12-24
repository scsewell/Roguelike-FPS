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
        WriteJsonFile(controls, GetIniDirectory(), CONTROLS_FILE_NAME);
    }

    public static Controls ReadControls()
    {
        return ParseJsonFile<Controls>(GetIniDirectory(), CONTROLS_FILE_NAME);
    }

    private static void WriteJsonFile<T>(T obj, string directoryPath, string fileName)
    {
        JsonSerializerSettings jss = new JsonSerializerSettings() { ContractResolver = new PrivateContractResolver() };
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented, jss);
        Directory.CreateDirectory(directoryPath);
        File.WriteAllText(directoryPath + fileName, json);
    }

    private static T ParseJsonFile<T>(string directoryPath, string fileName)
    {
        if (Directory.Exists(directoryPath))
        {
            string json = File.ReadAllText(directoryPath + fileName);
            return JsonConvert.DeserializeObject<T>(json);
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
