using System;
using NoSlimes.Loggers;
using UnityEngine;
using UnityEngine.UI;

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

    public Image HealthBar;
    public Image HealthBarFill;

    public State CurrentState => currentState;

    public static event Action OnReady;

    public static event Action OnGameplay;

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
                break;
            }
        }


    }

    public void Start()
    {
        // make sure we store the player in GameManager, it makes it so much easier to access the player at any time in any script
        Player = FindFirstObjectByType<Player>();
        
        HealthBar = GameObject.FindWithTag("BossHealth").GetComponent<Image>();
        HealthBarFill = GameObject.FindWithTag("BossHealthFill").GetComponent<Image>();
        
        HealthBar.gameObject.SetActive(false);
        HealthBar.gameObject.SetActive(false);

        OnReady?.Invoke();
    }

    private void OnEnable()
    {
        OnGameplay?.Invoke();
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
