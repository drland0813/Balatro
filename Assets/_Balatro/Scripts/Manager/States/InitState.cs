using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class InitState : State
    {
        protected override void OnEnter()
        {
            GameFlow.RequestStateChange(this, new LoadingPayLoad());
        }
    }
}