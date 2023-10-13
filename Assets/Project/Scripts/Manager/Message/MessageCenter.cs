using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MessageCenter : Singleton<MessageCenter>, ICenter
{
    private MapSystem _mapSystem;
    public MessageCenter()
    {
        _mapSystem = MapSystem.Instance;
    }

    public void CenterUpdate()
    {
        
    }
}
