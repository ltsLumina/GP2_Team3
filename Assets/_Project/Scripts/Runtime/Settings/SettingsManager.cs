using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsManager
{
    private static string FilePath => $"{Application.persistentDataPath}/settings.json";

    private static Dictionary<string, string> _settings;

    static SettingsManager()
    {
        LoadSettings();
    }

    public static T GetSetting<T>(string key)
    {
        if (_settings == null) LoadSettings();

        if (_settings.TryGetValue(key, out string jsonValue))
        {
            return JsonConvert.DeserializeObject<T>(jsonValue);
        }

        Debug.LogWarning($"Setting with key {key} not found");
        return default;
    }

    public static void SetSetting(string key, object value, bool saveImmediately = false)
    {
        if (_settings == null) LoadSettings();

        string jsonValue = JsonConvert.SerializeObject(value);
        _settings[key] = jsonValue;

        if (saveImmediately)
            SaveSettings();
    }

    public static bool ApplySettings()
    {
        try
        {
            SaveSettings();
            ControlsRemapping.SaveControlOverrides();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to apply settings: {ex.Message}");
            return false;
        }
    }

    public static void LoadSettings()
    {
        try
        {
            if (!System.IO.File.Exists(FilePath))
            {
                _settings = new Dictionary<string, string>();
                SaveSettings();
                return;
            }

            string json = System.IO.File.ReadAllText(FilePath);
            _settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load settings: {ex.Message}");
            _settings = new Dictionary<string, string>();
        }

        try
        {
            ControlsRemapping.LoadControlOverrides();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load control overrides: {ex.Message}");
        }
    }

    public static void SaveSettings()
    {
        try
        {
            string json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            System.IO.File.WriteAllText(FilePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save settings: {ex.Message}");
        }
    }
}
