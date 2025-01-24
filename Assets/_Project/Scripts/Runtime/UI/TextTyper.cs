using System.Threading;
using TMPro;
using UnityEngine;

public class TextTyper
{
    private readonly TMP_Text textComponent;
    private string message;
    private float letterPause;
    private CancellationTokenSource cancellationTokenSource;

    public TextTyper(TMP_Text textComponent)
    {
        this.textComponent = textComponent;
    }

    public TextTyper SetWithLetterPause(string message, float letterPause)
    {
        this.letterPause = letterPause;
        this.message = message;
        return this;
    }

    public TextTyper SetWithTotalDuration(string message, float totalDuration)
    {
        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("Message is null or empty! This should not happen");
            return this;
        }
        
        this.letterPause = totalDuration / message.Length;
        this.message = message;
        return this;
    }

    public Awaitable Start()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();

        textComponent.text = string.Empty;
        return TypeText(message, cancellationTokenSource.Token);
    }

    public TextTyper FinishEarly()
    {
        cancellationTokenSource?.Cancel();
        textComponent.text = message;
        return this;
    }

    private async Awaitable TypeText(string message, CancellationToken cancellationToken)
    {
        foreach (char letter in message.ToCharArray())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            textComponent.text += letter;
            await Awaitable.WaitForSecondsAsync(letterPause);
        }
    }
}
