using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.UI;

public class MainMenuState : State
{
    MainMenuView _view;
    

    protected override void OnEnter()
    {
        base.OnEnter();
        _view = GameFlow._instance.ShowUI<MainMenuView>("MainMenuView");
        _view.Init();
        _view.OnPlay = () =>
        {
            GameFlow.RequestStateChange(this, new GamePlayPayload());
        };
    }

    protected override void OnExit()
    {
        base.OnExit();
        UIManager.Instance.ReleaseUI(_view, false);
    }
}
