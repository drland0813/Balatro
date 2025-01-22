using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IFadePageScrollViewDataSource
{
    GameObject PageWillShow(int index, Transform rootContent);
    void PageDidHide(int index, GameObject page);
    void PageDidFocus(int index, GameObject page);
    void UpdateFadeProgress(int index, GameObject page, float progress);
    int GetNumberOfPages();
}

public class FadePageScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
    [SerializeField] protected Transform _rootContent;
    [SerializeField] protected float _pageSpacing;
    [SerializeField] protected float _pageWidth;
    [SerializeField] PageDirection _direction;
    [SerializeField] float _elasticity = 0.1f;
    [SerializeField] float _threshold = 0.4f;
    [SerializeField] float _velocityThreshold = 30f;
    [SerializeField] float _velocityThresholdNextPage = 500f;
    [SerializeField] protected RectTransform _viewPort;

    GameObject _activePage;
    GameObject _enterPage;

    int _currentPageIdx;
    int _enterPageIdx;
    float _pageScrollValue;
    float _currentVelocity;
    float _pageTargetScrollValue;
    bool _isDrag = false;
    bool _isDamping = true;

    IFadePageScrollViewDataSource _dataSource;

    public int CurrentPageIndex { get { return _currentPageIdx; } }
    public int EnterPageIndex { get { return _enterPageIdx; } }
    public GameObject ActivePage { get { return _activePage; } }
    public Transform RootContent { get { return _rootContent; } }

    public bool InteractableLocked
    {
        get;
        set;
    }

    public void Init(int pageIdx, IFadePageScrollViewDataSource dataSource)
    {
        _dataSource = dataSource;
        _currentPageIdx = pageIdx;
        _activePage = _dataSource.PageWillShow(pageIdx, _rootContent);
        _dataSource.PageDidFocus(_currentPageIdx, _activePage);
    }

    public void ScrollToNextPage()
    {
        _isDamping = true;
        var numPages = _dataSource.GetNumberOfPages();
        var pageIdx = Mathf.Min(_currentPageIdx + 1, numPages - 1);
        if (pageIdx < numPages)
        {
            NewPageEnter();
            _pageTargetScrollValue = (pageIdx - _currentPageIdx) * (_pageWidth + _pageSpacing);
        }
    }

    public void SetPageWidth(float width)
    {
        _pageWidth = width;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InteractableLocked)
            return;

        _isDrag = true;
        _currentVelocity = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (InteractableLocked)
            return;

        Vector2 p1;
        Vector2 p2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewPort, eventData.delta, eventData.pressEventCamera, out p1);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewPort, Vector2.zero, eventData.pressEventCamera, out p2);
        var localDelta = p1 - p2;

        if (_direction == PageDirection.Horizontal)
        {
            if (IsNoMorePages(Mathf.Sign(_pageScrollValue - localDelta.x)))
                return;

            _pageScrollValue -= localDelta.x;
            _currentVelocity = -localDelta.x / Time.deltaTime;
        }
        else if (_direction == PageDirection.Vertical)
        {
            if (IsNoMorePages(Mathf.Sign(_pageScrollValue + localDelta.y)))
                return;

            _pageScrollValue += localDelta.y;
            _currentVelocity = localDelta.y / Time.deltaTime;
        }

        if (!IsNoMorePages(Mathf.Sign(_pageScrollValue)) && _enterPage == null)
            NewPageEnter();

        UpdateFadeProgress();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InteractableLocked)
            return;

        _isDrag = false;

        Vector2 p1;
        Vector2 p2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewPort, eventData.delta, eventData.pressEventCamera, out p1);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewPort, Vector2.zero, eventData.pressEventCamera, out p2);
        var localDelta = p1 - p2;

        if (_direction == PageDirection.Horizontal)
        {
            _currentVelocity = -localDelta.x / Time.deltaTime;
        }
        else
        {
            _currentVelocity = localDelta.y / Time.deltaTime;
        }
        _isDamping = true;
        _pageTargetScrollValue = CalculateTargetValue();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        // _isDamping = false;
        // _currentVelocity = 0;
    }

    void LateUpdate()
    {
        if (!_isDrag && _isDamping)
        {
            _pageScrollValue = Mathf.SmoothDamp(_pageScrollValue, _pageTargetScrollValue, ref _currentVelocity, _elasticity);
            if (Mathf.Abs(_currentVelocity) < 1)
            {
                _currentVelocity = 0;
            }

            if (Mathf.Abs(_pageTargetScrollValue - _pageScrollValue) < 0.1)
            {
                _currentVelocity = 0;
                _pageScrollValue = _pageTargetScrollValue;
                _isDamping = false;
            }

            UpdateFadeProgress();
        }
    }

    bool IsNoMorePages(float sign)
    {
        return (_currentPageIdx == 0 && sign < 0)
                || (_currentPageIdx == _dataSource.GetNumberOfPages() - 1 && sign > 0);
    }

    float CalculateTargetValue()
    {
        var pageIndex = CalculateCenterPageIndex();
        var numPages = _dataSource.GetNumberOfPages();
        if (numPages > 0)
        {
            pageIndex = Mathf.Clamp(pageIndex, 0, numPages - 1);
        }
        return (pageIndex - _currentPageIdx) * (_pageWidth + _pageSpacing);
    }

    int CalculateCenterPageIndex()
    {
        float delta = _pageScrollValue / (_pageWidth + _pageSpacing);
        // end swiping
        if (Mathf.Abs(_currentVelocity) > _velocityThreshold)
        {
            if (_currentVelocity > 0)
            {
                if (Mathf.Abs(delta) > _threshold || Mathf.Abs(_currentVelocity) > _velocityThresholdNextPage)
                {
                    return Mathf.CeilToInt(_currentPageIdx + delta);
                }
                else
                {
                    return Mathf.FloorToInt(_currentPageIdx + delta);
                }
            }
            else
            {
                if (Mathf.Abs(delta) > _threshold || Mathf.Abs(_currentVelocity) > _velocityThresholdNextPage)
                {
                    return Mathf.FloorToInt(_currentPageIdx + delta);
                }
                else
                {
                    return Mathf.CeilToInt(_currentPageIdx + delta);
                }
            }
        }
        else
        {
            var dir = Mathf.Sign(_pageScrollValue);

            // right direction
            if (dir > 0)
            {
                if (Mathf.Abs(delta) > _threshold)
                {
                    return Mathf.CeilToInt(_currentPageIdx + delta);
                }
                else
                {
                    return Mathf.FloorToInt(_currentPageIdx + delta);
                }
            }
            else
            {
                if (Mathf.Abs(delta) > _threshold)
                {
                    return Mathf.FloorToInt(_currentPageIdx + delta);
                }
                else
                {
                    return Mathf.CeilToInt(_currentPageIdx + delta);
                }
            }
        }
    }

    void UpdateFadeProgress()
    {
        var sign = Mathf.Sign(_pageScrollValue);
        var f = Mathf.Abs(_pageScrollValue) / (_pageWidth + _pageSpacing);
        if (f >= 1f)
        {
            _dataSource.PageDidHide(_currentPageIdx, _activePage);
            _activePage = _enterPage;
            _enterPage = null;
            _currentPageIdx = _enterPageIdx;
            _pageScrollValue -= sign * (_pageWidth + _pageSpacing);
            _activePage.transform.localPosition = Vector3.zero;

            _dataSource.PageDidFocus(_currentPageIdx, _activePage);

            if (_isDamping || _isDrag)
            {
                if (IsNoMorePages(sign))
                {
                    _enterPage = null;
                }
                else
                {
                    NewPageEnter();
                }
            }

            f -= 1f;
        }
        else if ((_isDamping || _isDrag) && (_enterPageIdx - _currentPageIdx) * sign < 0 && _enterPage != null)
        {
            _dataSource.PageDidHide(_enterPageIdx, _enterPage);
            _enterPage = null;

            if (!IsNoMorePages(sign))
                NewPageEnter();
        }

        _dataSource.UpdateFadeProgress(_currentPageIdx, _activePage, 1f - f);

        if ((_isDamping || _isDrag) && _enterPage != null)
        {
            var distance = Mathf.Lerp(sign * (_pageWidth + _pageSpacing), 0, f);
            var position = Vector3.zero;
            if (_direction == PageDirection.Horizontal)
            {
                position.x = distance;
            }
            else
            {
                position.y = distance;
            }
            _enterPage.transform.localPosition = position;

            distance = Mathf.Lerp(0, -sign * (_pageWidth + _pageSpacing) / 5f, f);
            position = Vector3.zero;
            if (_direction == PageDirection.Horizontal)
            {
                position.x = distance;
            }
            else
            {
                position.y = distance;
            }
            _activePage.transform.localPosition = position;
        }

        if (!_isDamping && !_isDrag && _enterPage != null)
        {
            _dataSource.PageDidHide(_enterPageIdx, _enterPage);
            _enterPage = null;

            _dataSource.PageDidFocus(_currentPageIdx, _activePage);
        }
    }

    void NewPageEnter()
    {
        var sign = Mathf.Sign(_pageScrollValue);
        _enterPageIdx = sign > 0 ? _currentPageIdx + 1 : _currentPageIdx - 1;

        _enterPage = _dataSource.PageWillShow(_enterPageIdx, _rootContent);
        _enterPage.transform.SetAsLastSibling();
        _dataSource.UpdateFadeProgress(_enterPageIdx, _enterPage, 1f);
#if UNITY_EDITOR
        _enterPage.name = _enterPageIdx.ToString();
#endif
    }
}
