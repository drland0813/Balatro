using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Common.UI;
using DG.Tweening;
using TMPro;

public class LoadingState : State
{
    private LoadingScreenView _loadingScreen;
    public Action OnComplete;
    protected override void OnEnter()
    {
        base.OnEnter();
        _loadingScreen = GameFlow._instance.ShowUI<LoadingScreenView>("LoadingScreenView");
        _loadingScreen.OnComplete = () =>
        {
            GameFlow.RequestStateChange(this, new MainMenuPayload());
        };
        var timeOut = 10f;
        _loadingScreen.StartLoading(timeOut);
    }

    protected override void OnExit()
    {
        base.OnExit();
        UIManager.Instance.ReleaseUI(_loadingScreen, false);
    }
    
    
}
public class LoadingScreenView : UIController
{
    [SerializeField] private Image _progressBar; 
    [SerializeField] private TextMeshProUGUI _loadingText;
    
    public Action OnComplete;


    public void StartLoading(float timeOut)
    {
        StartCoroutine(Loading(timeOut));
    }

    private IEnumerator Loading(float timeOut)
    {
        _loadingText.text = $"Loading...";
        _progressBar.fillAmount = 0f;
        yield return _progressBar.DOFillAmount(0.8f, 2f).WaitForCompletion();

#if !UNITY_EDITOR
        var time = 0f;
        while (time < timeOut)
        {
            if (AdsManager.Instance.IsInitialized)
            {
                yield return _progressBar.DOFillAmount(1f, 0.5f).WaitForCompletion();
                OnComplete?.Invoke();
                yield break;
            }
            time += Time.deltaTime;
            yield return null;
        }
#endif
        yield return _progressBar.DOFillAmount(1f, 0.5f).WaitForCompletion();
        OnComplete?.Invoke();
    }
}