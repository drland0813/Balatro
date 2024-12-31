using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Base;
using MagicTouch;
using UnityEngine;

public class GameFlowStateMachine : StateMachine
{
    private readonly MonoBehaviour _coroutineRunner;
    private Coroutine _updateRoutine;
    private readonly InitState _initState;
    private readonly MainMenuState _mainMenuState;
    private readonly GamePlayState _classicGameState;
    private readonly LoadingState _loadingState;


    public GameFlowStateMachine(MonoBehaviour coroutineRunner)
    {
        //return;
        _coroutineRunner = coroutineRunner;
        _initState = new InitState();
        _loadingState = new LoadingState();
        _mainMenuState = new MainMenuState();
        _classicGameState = new GamePlayState();
        AddStates(_initState, _loadingState, _mainMenuState, _classicGameState);
        OnStateChange += CheckUpdate;
        ChangeState(_initState);
    }

    public bool HandleStateChangeRequest<T>(object sender, RequestPayload<T> requestPayload) where T : State
    {
        if (_currentState.GetType() == typeof(T)) { return false; }
        var changeType = typeof(T);
        Debug.LogWarning("change state " + changeType);
        if (changeType == typeof(GamePlayState))
        {
            ChangeState(_classicGameState);
        }
        else if (changeType == typeof(MainMenuState))
        {
            ChangeState(_mainMenuState);
        }
        else if (changeType == typeof(LoadingState))
        {
            ChangeState(_loadingState);
        }
        return true;
    }

    private void CheckUpdate(IState newState)
    {
        var type = newState.GetType();
        var bindings = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        var methodInfo = type.GetMethod("OnUpdate", bindings);

        var isUpdateMethodOverridden = methodInfo != null && methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType;

        if (!isUpdateMethodOverridden)
        {
            if (_updateRoutine != null) { _coroutineRunner.StopCoroutine(_updateRoutine); }
            return;
        }

        _updateRoutine = _coroutineRunner.StartCoroutine(UpdateRoutine());

        IEnumerator UpdateRoutine()
        {
            while (true)
            {
                yield return null;
                StateUpdate();
            }
        }
    }
}