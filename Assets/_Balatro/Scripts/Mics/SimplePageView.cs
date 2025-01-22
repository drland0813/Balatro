using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePageView : PageView
{
    public override void VisibilityChanged(bool visible)
    {
        GetComponent<CanvasGroup>().alpha = visible ? 1f : 0f;
    }
}
