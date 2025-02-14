using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;

    private void OnEnable()
    {
        SettingsManager.OnLoadSettings += LoadSettings;

        masterVolumeSlider.onValueChanged.AddListener((val) => { OnVolumeValueChanged("MasterVol", val); SetLabelText(); });
        musicVolumeSlider.onValueChanged.AddListener((val) => { OnVolumeValueChanged("MusicVol", val); SetLabelText(); });
        sfxVolumeSlider.onValueChanged.AddListener((val) => { OnVolumeValueChanged("SFXVol", val); SetLabelText(); });

        LoadSettings();
    }

    private void OnDisable()
    {
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        musicVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();

        SettingsManager.OnLoadSettings -= LoadSettings;
    }

    private void LoadSettings()
    {
        masterVolumeSlider.value = SettingsManager.GetSetting<float>("MasterVol", masterVolumeSlider.maxValue);
        musicVolumeSlider.value = SettingsManager.GetSetting<float>("MusicVol", musicVolumeSlider.maxValue);
        sfxVolumeSlider.value = SettingsManager.GetSetting<float>("SFXVol", sfxVolumeSlider.maxValue);

        SetLabelText();
    }

    private void SetLabelText()
    {
        masterVolumeText.text = $"Master: {Mathf.Round(masterVolumeSlider.value * 100)}";
        musicVolumeText.text = $"Music: {Mathf.Round(musicVolumeSlider.value * 100)}";
        sfxVolumeText.text = $"SFX: {Mathf.Round( sfxVolumeSlider.value * 100 )}";

        FMODUnity.RuntimeManager.GetVCA("vca:/MasterVCA").setVolume(SettingsManager.GetSetting<float>("MasterVol"));
        FMODUnity.RuntimeManager.GetVCA("vca:/MusicVCA").setVolume(SettingsManager.GetSetting<float>("MusicVol"));
        FMODUnity.RuntimeManager.GetVCA("vca:/SFXVCA").setVolume(SettingsManager.GetSetting<float>("SFXVol"));
    }

    private void OnVolumeValueChanged(string key, float value)
    {
        SettingsManager.SetSetting(key, value);
    }
}
