using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MessageCenter : ICenter
{
    private MapSystem _mapSystem;
    public MessageCenter(MapSystem mapSystem)
    {
        _mapSystem = mapSystem;
    }

    public void CenterUpdate()
    {
        throw new System.NotImplementedException();
    }
}
