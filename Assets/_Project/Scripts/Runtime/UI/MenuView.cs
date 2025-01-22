using NoSlimes.Loggers;
using UnityEngine;
using UnityEngine.Events;

public abstract class MenuView : LoggerMonoBehaviour
{
    [field: SerializeField] public bool PauseGame { get; private set; } = false;
    public UnityEvent onShowMenu;
    public UnityEvent onHideMenu;

    public abstract void Initialize();
    
    public virtual void Show()
    {
        onShowMenu?.Invoke();

        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        onHideMenu?.Invoke();

        gameObject.SetActive(false);
    }
}
