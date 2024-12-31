using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Common.UI;
using GamePlay;

public class GamePlayState : State
{
    GamePlayController _controller;
    GamePlayView _view;
    private int _reviveTime;
    private bool _watchedAdToRevive;
    private int _currentLevelTemp = 0;

    protected override void OnEnter()
    {
        base.OnEnter();
        _view = GameFlow._instance.ShowUI<GamePlayView>("GamePlayView");
        _controller = _view.GetComponent<GamePlayController>();
        _controller.OnWin = () =>
        {
            OnEndGame();
        };

        _controller.OnLose = () =>
        {
            OnEndGame();
        };
    }

    void OnEndGame()
    {
        GameFlow.RequestStateChange(this, new MainMenuPayload());
    }

    protected override void OnExit()
    {
        base.OnExit();
        UIManager.Instance.ReleaseUI(_view, true);
    }
}
