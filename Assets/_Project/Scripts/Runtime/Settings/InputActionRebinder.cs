using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputActionRebinder : MonoBehaviour
{
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private int bindingIndex;

    private InputAction action;
    private Button rebindButton;

    private void OnEnable()
    {
        rebindButton = GetComponentInChildren<Button>();
        rebindButton.onClick.AddListener(RebindAction);

        action = InputSystem.actions.FindAction(actionReference.action.id);

        SetButtonText();
    }

    private void OnDestroy()
    {
        rebindButton.onClick.RemoveAllListeners();
    }

    private void RebindAction()
    {
        rebindButton.GetComponentInChildren<TextMeshProUGUI>().text = "...";

        ControlsRemapping.OnSuccessfulRebinding += OnSuccessfulRebinding;

        bool isGamepad = action.bindings[bindingIndex].path.Contains("Gamepad");

        if (isGamepad)
            ControlsRemapping.RemapGamepadAction(action, bindingIndex);
        else
            ControlsRemapping.RemapKeyboardAction(action, bindingIndex);
    }

    private void SetButtonText()
    {
        rebindButton.GetComponentInChildren<TMP_Text>().text = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
    }

    private void OnSuccessfulRebinding(InputAction action)
    {
        ControlsRemapping.OnSuccessfulRebinding -= OnSuccessfulRebinding;
        SetButtonText();
    }
}
