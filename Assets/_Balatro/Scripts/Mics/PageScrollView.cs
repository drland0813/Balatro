using UnityEngine.SocialPlatforms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IPageScrollViewDataSource
{
    int GetNumberOfPagesForPageScrollView(PageScrollView pageScrollView);
    PageView GetPageViewInPageScrollView(PageScrollView pageScrollView, int page);
}

public enum PageDirection
{
    Horizontal,
    Vertical
}

public class PageScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
    [Header("PageView")]
    [SerializeField] protected RectTransform _content;
    [SerializeField] protected float _pageSpacing;
    [SerializeField] protected float _pageWidth;
    [SerializeField] PageDirection _direction;
    [SerializeField] float _elasticity = 0.1f;
    [SerializeField] float _threshold = 0.4f;
    [SerializeField] float _velocityThreshold = 30f;
    [SerializeField] float _velocityThresholdNextPage = 500f;
    [SerializeField] protected RectTransform _viewPort;

    float _currentVelocity;
    float _contentScrollValue;
    float _contentTargetScrollValue;
    bool _isDrag = false;
    bool _isDamping = true;
    int _currentPageCenter;

    IPageScrollViewDataSource _dataSource;
    LinkedList<PageView> _reusablePages = new LinkedList<PageView>();
    Range _visiblePageRange;

    List<PageView> _visiblePages = new List<PageView>();

    public event System.Action<int> OnPageCenterChanged;

    public void SetDataSource(IPageScrollViewDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public PageView GetVisiblePage(int page)
    {
        int idx = page - _visiblePageRange.from;
        if (idx < 0 || idx >= _visiblePages.Count)
            return null;

        return _visiblePages[idx];
    }

    public PageView GetReusablePage()
    {
        if (_reusablePages.Count == 0)
            return null;

        var pageView = _reusablePages.First.Value;
        _reusablePages.RemoveFirst();
        return pageView;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDrag = true;
        _currentVelocity = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 p1;
        Vector2 p2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewPort, eventData.delta, eventData.pressEventCamera, out p1);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewPort, Vector2.zero, eventData.pressEventCamera, out p2);
        var localDelta = p1 - p2;

        if (_direction == PageDirection.Horizontal)
        {
            _contentScrollValue -= localDelta.x;
            _currentVelocity = -localDelta.x / Time.deltaTime;
        }
        else if (_direction == PageDirection.Vertical)
        {
            _contentScrollValue += localDelta.y;
            _currentVelocity = localDelta.y / Time.deltaTime;
        }

        UpdateContentPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
        _contentTargetScrollValue = CalculateTargetValue();
    }

    void LateUpdate()
    {
        Reposition();
    }

    void Reposition()
    {
        if (!_isDrag && _isDamping)
        {
            _contentScrollValue = Mathf.SmoothDamp(_contentScrollValue, _contentTargetScrollValue, ref _currentVelocity, _elasticity);
            if (Mathf.Abs(_currentVelocity) < 1)
            {
                _currentVelocity = 0;
            }
            if (Mathf.Abs(_contentTargetScrollValue - _contentScrollValue) < 0.1)
            {
                _currentVelocity = 0;
                _contentScrollValue = _contentTargetScrollValue;
                _isDamping = false;
            }

            UpdateContentPosition();
        }
    }

    void UpdateContentPosition()
    {
        Vector2 vector = _content.anchoredPosition;
        if (_direction == PageDirection.Horizontal)
        {
            vector.x = -_contentScrollValue;
        }
        else
        {
            vector.y = _contentScrollValue;
        }
        _content.anchoredPosition = vector;

        bool firstUpdate = _visiblePages.Count == 0;
        if (firstUpdate)
        {
            RecalculateVisiblePagesFromScratch();
            OnPageCenterChanged?.Invoke(_currentPageCenter);
        }
        else
        {
            RefreshVisiblePages();
        }

        if (!_isDamping && !_isDrag)
        {
            var pageCenter = CalculateCenterPageIndex();
            if (pageCenter != _currentPageCenter || firstUpdate)
            {
                OnPageCenterChanged?.Invoke(pageCenter);
                _currentPageCenter = pageCenter;
            }
        }
    }

    int CalculateCenterPageIndex()
    {
        float pageIndex = _contentScrollValue / (_pageWidth + _pageSpacing);
        float delta = pageIndex - Mathf.Floor(pageIndex);

        // end swiping
        if (Mathf.Abs(_currentVelocity) > _velocityThreshold)
        {
            if (_currentVelocity > 0)
            {
                if (delta > _threshold || Mathf.Abs(_currentVelocity) > _velocityThresholdNextPage)
                {
                    pageIndex = Mathf.Ceil(pageIndex);
                }
                else
                {
                    pageIndex = Mathf.Floor(pageIndex);
                }
            }
            else
            {
                if (delta > _threshold || Mathf.Abs(_currentVelocity) > _velocityThresholdNextPage)
                {
                    pageIndex = Mathf.Floor(pageIndex);
                }
                else
                {
                    pageIndex = Mathf.Ceil(pageIndex);
                }
            }
        }
        else
        {
            var dir = pageIndex - _currentPageCenter;

            // right direction
            if (dir > 0)
            {
                if (delta > _threshold)
                {
                    pageIndex = Mathf.Ceil(pageIndex);
                }
                else
                {
                    pageIndex = Mathf.Floor(pageIndex);
                }
            }
            else
            {
                if (delta > _threshold)
                {
                    pageIndex = Mathf.Floor(pageIndex);
                }
                else
                {
                    pageIndex = Mathf.Ceil(pageIndex);
                }
            }
        }

        return (int)pageIndex;
    }

    float CalculateTargetValue()
    {
        var pageIndex = CalculateCenterPageIndex();
        var numPages = _dataSource.GetNumberOfPagesForPageScrollView(this);
        if (numPages > 0)
        {
            pageIndex = Mathf.Clamp(pageIndex, 0, numPages - 1);
        }
        return pageIndex * (_pageWidth + _pageSpacing);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        _isDamping = false;
        _currentVelocity = 0;
    }

    public void RefreshVisiblePages()
    {
        Range newVisiblePages = CalculateCurrentVisiblePageRange();
        int oldTo = _visiblePageRange.Last();
        int newTo = newVisiblePages.Last();

        if (newVisiblePages.from > oldTo || newTo < _visiblePageRange.from)
        {
            //We jumped to a completely different segment this frame, destroy all and recreate
            RecalculateVisiblePagesFromScratch();
            return;
        }

        //Remove pages that disappeared to the top
        for (int i = _visiblePageRange.from; i < newVisiblePages.from; i++)
        {
            HidePage(false);
        }
        //Remove pages that disappeared to the bottom
        for (int i = newTo; i < oldTo; i++)
        {
            HidePage(true);
        }
        //Add pages that appeared on top
        for (int i = _visiblePageRange.from - 1; i >= newVisiblePages.from; i--)
        {
            AddPage(i, false);
        }
        //Add pages that appeared on bottom
        for (int i = oldTo + 1; i <= newTo; i++)
        {
            AddPage(i, true);
        }
        _visiblePageRange = newVisiblePages;
    }

    void RecalculateVisiblePagesFromScratch()
    {
        ClearAllPages();
        SetInitialVisiblePages();
    }

    void ClearAllPages()
    {
        while (_visiblePages.Count > 0)
        {
            HidePage(false);
        }
        _visiblePageRange = new Range(0, 0);
    }

    Range CalculateCurrentVisiblePageRange()
    {
        var viewPortSize = _direction == PageDirection.Horizontal ? _viewPort.rect.size.x : _viewPort.rect.size.y;
        var distanceStart = _contentScrollValue - viewPortSize * 0.5f;
        int startIndex = FindPageIndexAtDistance(distanceStart);
        int endIndex = FindPageIndexAtDistance(_contentScrollValue + viewPortSize * 0.5f);

        return new Range(startIndex, endIndex - startIndex + 1);
    }

    void SetInitialVisiblePages()
    {
        Range visiblePages = CalculateCurrentVisiblePageRange();
        for (int i = 0; i < visiblePages.count; i++)
        {
            AddPage(visiblePages.from + i, true);
        }
        _visiblePageRange = visiblePages;
    }

    void AddPage(int page, bool atEnd)
    {
        PageView newPage = _dataSource.GetPageViewInPageScrollView(this, page);
        if (atEnd)
            _visiblePages.Add(newPage);
        else
            _visiblePages.Insert(0, newPage);

        newPage.transform.localPosition = PositionForPage(page);

        newPage.VisibilityChanged(true);
    }

    void HidePage(bool last)
    {
        int page = last ? _visiblePages.Count - 1 : 0;
        PageView removedPage = _visiblePages[page];

        StorePageForReuse(removedPage);
        removedPage.VisibilityChanged(false);

        _visiblePages.RemoveAt(page);
        _visiblePageRange.count -= 1;

        if (!last)
        {
            _visiblePageRange.from += 1;
        }
    }

    void StorePageForReuse(PageView pageView)
    {
        _reusablePages.AddLast(pageView);
    }

    Vector2 PositionForPage(int page)
    {
        var position = page * (_pageWidth + _pageSpacing);
        return _direction == PageDirection.Vertical ? new Vector2(0, position) : new Vector2(position, 0);
    }

    int FindPageIndexAtDistance(float distance)
    {
        if (distance < 0)
            return 0;

        var idx = Mathf.FloorToInt((distance + _pageWidth * 0.5f) / (_pageWidth + _pageSpacing));
        return Mathf.Min(idx, _dataSource.GetNumberOfPagesForPageScrollView(this) - 1);
    }
}

internal static class RangeExtensions
{
    public static int Last(this Range range)
    {
        if (range.count == 0)
        {
            throw new System.InvalidOperationException("Empty range has no to()");
        }
        return (range.from + range.count - 1);
    }

    public static bool Contains(this Range range, int num)
    {
        return num >= range.from && num < (range.from + range.count);
    }
}
