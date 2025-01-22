using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Common;
using Common.UI;
using Common.Utils;

public class MainMenuView : UIController
{
    public Action OnPlay;

    public void Init()
    {
        
    }
    
    public void ClickPlay()
    {
        OnPlay?.Invoke();
    }
}
