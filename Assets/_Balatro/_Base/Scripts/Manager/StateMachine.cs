using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
public interface IState
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
}

public abstract record RequestPayload<T> where T : State;

public record LoadingPayLoad : RequestPayload<LoadingState>
{
    public Action OnComplete;
};

public record MainMenuPayload : RequestPayload<MainMenuState>;
public record GamePlayPayload : RequestPayload<GamePlayState>;

public interface IPreloadable
{
    UniTask Preload(PreloadPayload payload = null);
}

public class PreloadPayload
{
    public static readonly PreloadPayload Empty = new();
}

public abstract class State : IState
{

    public State() { }


    protected virtual void OnEnter() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnExit() { }

    void IState.OnEnter() => OnEnter();
    void IState.OnUpdate() => OnUpdate();
    void IState.OnExit() => OnExit();
}

public class StateMachine
{
    public event Action<IState> OnStateChange;

    protected readonly HashSet<IState> _states = new();
    protected IState _currentState;

    public void AddStates(params IState[] states)
    {
        foreach (var state in states)
        {
            AddState(state);
        }
    }

    public void AddState(IState state)
    {
        if (!_states.Add(state))
        {
            Debug.Log($"State is already added: {state.GetType().Name}");
        };
    }


    public void StateUpdate()
    {
        if (_currentState == null) { return; }

        _currentState.OnUpdate();
    }

    public void ChangeState<T>() where T : IState
    {
        var state = _states.FirstOrDefault(match => match is T);
        if (state != null)
        {
            ChangeState(state);
        }
    }

    public void ChangeState(IState state)
    {
        if (!_states.Contains(state))
        {
            Debug.LogError($"Cannot transition to {state.GetType().Name}: State was not added");
            return;
        }

        _currentState?.OnExit();
        _currentState = state;
        _currentState.OnEnter();

        OnStateChange?.Invoke(state);
    }

    public void Shutdown()
    {
        _currentState.OnExit();
        _currentState = null;

        foreach (var state in _states)
        {
            (state as IDisposable)?.Dispose();
        }
    }
}