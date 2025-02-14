using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QualitySettingsManager : MonoBehaviour
{
    [Header("Quality")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Fullscreen")]
    [SerializeField] private TMP_Dropdown fullscreenModeDropdown;

    private Resolution[] availableResolutions;

    private void OnEnable()
    {
        SettingsManager.OnLoadSettings += LoadSettings;

        // Quality
        string[] qualityNames = QualitySettings.names;
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(qualityNames.ToList().ConvertAll(n =>
            new TMP_Dropdown.OptionData(n)
        ));

        // Resolution
        availableResolutions = Screen.resolutions
            .Select(r => new Resolution { width = r.width, height = r.height })
            .Distinct()
            .ToArray();

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(availableResolutions.Select(r =>
            new TMP_Dropdown.OptionData($"{r.width}x{r.height}")
        ).ToList());

        // Fullscreen
        string[] fullscreenModeNames = Enum.GetNames(typeof(FullScreenMode));
        fullscreenModeDropdown.ClearOptions();
        fullscreenModeDropdown.AddOptions(fullscreenModeNames.ToList().ConvertAll(n =>
            new TMP_Dropdown.OptionData(InsertSpaces(n))
        ));

        qualityDropdown.onValueChanged.AddListener(OnQualityValueChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionValueChanged);
        fullscreenModeDropdown.onValueChanged.AddListener(OnFullscreenValueChanged);

        LoadSettings();
    }

    private void OnDisable()
    {
        qualityDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        fullscreenModeDropdown.onValueChanged.RemoveAllListeners();

        SettingsManager.OnLoadSettings -= LoadSettings;

        //this is strange, but somewhat needed to load the resolution correctly if menu is closed
        LoadSettings();
    }

    private void LoadSettings()
    {
        qualityDropdown.value = SettingsManager.GetSetting<int>("QualityLevel", QualitySettings.names.Length - 1);
        fullscreenModeDropdown.value = SettingsManager.GetSetting<int>("Fullscreen", (int)FullScreenMode.ExclusiveFullScreen);

        //Resolution
        int resolutionIndex = Array.FindIndex(availableResolutions, r => r.width == Screen.width && r.height == Screen.height);
        if (resolutionIndex < 0) resolutionIndex = 0;

        resolutionDropdown.value = SettingsManager.GetSetting<int>("Resolution", resolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    private void OnQualityValueChanged(int val)
    {
        QualitySettings.SetQualityLevel(val);
        SettingsManager.SetSetting("QualityLevel", val);
    }

    private void OnResolutionValueChanged(int val)
    {
        if (val < 0 || val >= availableResolutions.Length) return;

        Resolution resolution = availableResolutions[val];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        SettingsManager.SetSetting("Resolution", val);
    }

    private void OnFullscreenValueChanged(int val)
    {
        Screen.fullScreenMode = (FullScreenMode)val;
        SettingsManager.SetSetting("Fullscreen", val);
    }

    private string InsertSpaces(string input)
    {
        return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
    }
}
