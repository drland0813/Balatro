using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Common.UI;
using Common.Utils;

public class GameFlow : MonoBehaviour
{
    public static GameFlow _instance;
    private GameFlowStateMachine _stateMachine;
    private void Start()
    {
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
        _instance = this;
        _stateMachine = new GameFlowStateMachine(this);
    }

    public static void RequestStateChange<T>(object sender, RequestPayload<T> requestPayload = null, bool playTransition = false) where T : State
    {
        if (_instance == null) { return; }
        SceneTransition(() =>
        {
            _instance._stateMachine.HandleStateChangeRequest(sender, requestPayload);
        });
    }

    public T ShowUI<T>(string uiPath, bool overlay = false) where T : UIController
    {
        return UIManager.Instance.ShowUIOnTop<T>(uiPath, overlay);
    }

    public static void SceneTransition(System.Action onSceneOutFinished)
    {
        UIManager.Instance.SetUIInteractable(false);
        Color color;
        ColorUtility.TryParseHtmlString("#000000", out color);

        SceneDirector.Instance.Transition(new TransitionFade()
        {
            duration = 0.667f,
            color = color,
            tweenIn = TweenFunc.TweenType.Sine_EaseInOut,
            tweenOut = TweenFunc.TweenType.Sine_EaseOut,
            onStepOutDidFinish = () =>
            {
                onSceneOutFinished.Invoke();
            },
            onStepInDidFinish = () =>
            {
                UIManager.Instance.SetUIInteractable(true);
            }
        });
    }
}
