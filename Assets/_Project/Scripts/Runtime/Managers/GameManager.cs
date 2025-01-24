using NoSlimes.Loggers;
using UnityEngine;

public abstract class State : LoggerMonoBehaviour
{
    public virtual void EnterState() { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private State[] states;
    [SerializeField]
    private State currentState;
    private Player _player;
    public Player Player
    {
        get
        {
            if (_player != null)
                return _player;

            return FindAnyObjectByType<Player>();
        }
        set => _player = value; 
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        states = FindObjectsByType<State>(FindObjectsSortMode.None);

        foreach (State s in states)
        {
            if (s.GetType() == typeof(PlayingState))
            {
                currentState = s;
                return;
            }
        }
    }

    public void Start()
    {
        // make sure we store the player in GameManager, it makes it so much easier to access the player at any time in any script
        Player = FindFirstObjectByType<Player>();
    }


    public void Update()
    {
        if (currentState)
            currentState.UpdateState();
    }

    public void SwitchState<T>() where T : State
    {
        foreach (State s in states)
        {
            if (s is T newState)
            {
                if (currentState)
                    currentState.ExitState();

                currentState = newState;
                currentState.EnterState();
                return;
            }
        }
    }
}
