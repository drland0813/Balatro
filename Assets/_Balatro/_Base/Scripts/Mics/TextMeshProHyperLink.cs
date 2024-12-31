using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class TextMeshProHyperLink : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    [Serializable]
    public class HrefClickEvent : UnityEvent<string, PointerEventData> { }

    [SerializeField]
    private HrefClickEvent _onHrefClick = new HrefClickEvent();

    TextMeshProUGUI _text;

    public HrefClickEvent OnHrefClick
    {
        get { return _onHrefClick; }
        set { _onHrefClick = value; }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_onHrefClick == null)
            return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, Input.mousePosition, eventData.pressEventCamera);
        if (linkIndex >= 0)
        {
            // was a link clicked?
            TMP_LinkInfo linkInfo = _text.textInfo.linkInfo[linkIndex];

            _onHrefClick.Invoke(linkInfo.GetLinkID(), eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }
}
