using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindText : MonoBehaviour
{
    private TMP_Text keybindText;
    [SerializeField] private InputActionReference inputActionReference;
    [SerializeField] private int keybindIndex;

    private void Start()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    private void OnEnable()
    {
        keybindText = GetComponent<TMP_Text>();
        OnTextChanged(keybindText);
    }

    private void OnTextChanged(UnityEngine.Object @object)
    {
        if(@object == keybindText)
        {
            Debug.Log("Text changed");

            string newText = keybindText.text.ToUpper().Replace("[KEYBIND]", inputActionReference.action.GetBindingDisplayString(keybindIndex));
            keybindText.text = newText;

            Canvas.ForceUpdateCanvases();
        }
    }
}
