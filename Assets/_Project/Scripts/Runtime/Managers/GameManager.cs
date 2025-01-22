using System.Collections.Generic;
using NoSlimes.Loggers;
using UnityEngine;

public class State : LoggerMonoBehaviour
{
    public virtual void EnterState() { }
    public virtual void UpdateState() {}
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
            if(_player != null)
                return _player;

            return FindAnyObjectByType<Player>();
        }
        set { _player = value; }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        states = GameObject.FindObjectsByType<State>(FindObjectsSortMode.None);

        foreach (State s in states)
        {
            if (s.GetType() == typeof(PlayingState))
            {
                currentState = s;
                return;
            }
        }
    }
    
    
    public void Update()
    {
        currentState?.UpdateState();
    }
    
    public void SwitchState<T>() where T : State
    {
        foreach (State s in states)
        {
            if (s.GetType() == typeof(T))
            {
                currentState?.ExitState();
                currentState = s;
                currentState?.EnterState();
                return;
            }
        }
    }
}
