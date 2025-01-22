using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsRemapping : MonoBehaviour
{
    private static string FilePath => Application.persistentDataPath + "/controlsOverrides.dat";

    private static InputActionMap gameActionMap;
    private static Dictionary<string, string> overridesDictionary = new();
    public static ReadOnlyDictionary<string, string> OverridesDictionary => new(overridesDictionary);

    public static event Action<InputAction> OnSuccessfulRebinding;

    private void Awake()
    {
        gameActionMap = InputSystem.actions.FindActionMap("Player");

        //if (File.Exists(FilePath))
        //    LoadControlOverrides();
    }

    public static void RemapKeyboardAction(InputAction actionToRebind, int targetBinding)
    {
        actionToRebind.Disable();

        var rebindOperation = actionToRebind.PerformInteractiveRebinding(targetBinding)
            .WithControlsHavingToMatchPath("<Keyboard>")
            .WithBindingGroup("Keyboard")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation => OnSuccessfulRebinding?.Invoke(null))
            .OnComplete(operation =>
            {
                operation.Dispose();
                AddOverrideToDictionary(actionToRebind.id, actionToRebind.bindings[targetBinding].effectivePath, targetBinding);
                
                //SaveControlOverrides();
                
                actionToRebind.Enable();
                OnSuccessfulRebinding?.Invoke(actionToRebind);
            })
            .Start();
    }

    public static void RemapGamepadAction(InputAction actionToRebind, int targetBinding)
    {
        actionToRebind.Disable();

        var rebindOperation = actionToRebind.PerformInteractiveRebinding(targetBinding)
            .WithControlsHavingToMatchPath("<Gamepad>")
            .WithBindingGroup("Gamepad")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation => OnSuccessfulRebinding?.Invoke(null))
            .OnComplete(operation =>
            {
                operation.Dispose();
                AddOverrideToDictionary(actionToRebind.id, actionToRebind.bindings[targetBinding].effectivePath, targetBinding);
                
                //SaveControlOverrides();
                
                actionToRebind.Enable();
                OnSuccessfulRebinding?.Invoke(actionToRebind);
            })
            .Start();
    }

    private static void AddOverrideToDictionary(Guid actionId, string path, int bindingIndex)
    {
        string key = string.Format("{0} : {1}", actionId.ToString(), bindingIndex);

        if (overridesDictionary.ContainsKey(key))
        {
            overridesDictionary[key] = path;
        }
        else
        {
            overridesDictionary.Add(key, path);
        }
    }

    public static void SaveControlOverrides()
    {
        FileStream file = new(FilePath, FileMode.OpenOrCreate);
        BinaryFormatter bf = new();
        bf.Serialize(file, overridesDictionary);
        file.Close();
    }

    public static void LoadControlOverrides()
    {
        if (!File.Exists(FilePath))
        {
            return;
        }

        FileStream file = new(FilePath, FileMode.OpenOrCreate);
        BinaryFormatter bf = new();
        overridesDictionary = bf.Deserialize(file) as Dictionary<string, string>;
        file.Close();

        if (overridesDictionary == null)
        {
            overridesDictionary = new();
            return;
        }

        foreach(var item in overridesDictionary)
        {
            if(item.Value == null)
            {
                overridesDictionary.Remove(item.Key);
            }
        }

        foreach (var item in overridesDictionary)
        {
            string[] split = item.Key.Split(new string[] { " : " }, StringSplitOptions.None);
            Guid id = Guid.Parse(split[0]);
            int index = int.Parse(split[1]);
            gameActionMap.asset.FindAction(id).ApplyBindingOverride(index, item.Value);
        }
    }
}
