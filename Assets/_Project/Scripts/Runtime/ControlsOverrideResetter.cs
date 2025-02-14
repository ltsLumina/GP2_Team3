using System;
using UnityEngine;
using UnityEngine.UI;

public class ControlsOverrideResetter : MonoBehaviour
{
    [SerializeField] private Button resetButton;

    private void OnEnable()
    {
        resetButton.onClick.AddListener(OnResetPressed);
    }

    private void OnDisable()
    {
        resetButton.onClick.RemoveAllListeners();
    }

    private void OnResetPressed()
    {
        ControlsRemapping.ResetControlOverrides();
        GetComponentInParent<SettingsMenuView>().Reload();
    }
}
