using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InputQuestObjective : QuestObjective
{
    [SerializeField] private InputActionReference inputAction;
    [SerializeField] private int bindingIndex = 0;

    public override string ObjectiveDescription
    {
        get
        {
            var baseDesc = base.ObjectiveDescription;
            return baseDesc.ToUpper().Replace("[KEYBIND]", inputAction.action.GetBindingDisplayString(bindingIndex));
        }
    }

    public override void StartObjective()
    {
        currentProgress = 0;
        totalProgress = 1;

        isStarted = true;
        inputAction.action.performed += RegisterInput;
    }

    public void RegisterInput(InputAction.CallbackContext context)
    {
        if (!IsStarted || IsComplete) return;

        //int index = context.action.GetBindingIndexForControl(context.control);
        ////if (index != bindingIndex) return;

        currentProgress++;
        CheckStatus();
    }


    public override void CheckStatus()
    {
        if (currentProgress >= totalProgress)
        {
            isComplete = true;
            EndObjective();
        }
    }

    private async void EndObjective()
    {
        await Awaitable.WaitForSecondsAsync(1.3f);
        QuestObjectiveManager.Instance.EndQuest(this);
    }

    public override void ResetObjective()
    {
        base.ResetObjective();
        inputAction.action.performed -= RegisterInput;
    }
}
