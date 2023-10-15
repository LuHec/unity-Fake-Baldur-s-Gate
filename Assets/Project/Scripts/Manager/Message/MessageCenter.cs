using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MessageCenter : Singleton<MessageCenter>
{
    private MapSystem _mapSystem;
    public GlobalState globalState;

    private void Start()
    {
        _mapSystem = MapSystem.Instance;
        globalState = new GlobalState();
    }
    
}
