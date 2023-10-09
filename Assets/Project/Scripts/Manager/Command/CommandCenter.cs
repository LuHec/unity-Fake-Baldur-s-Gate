using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于发布指令给当前控制角色
/// </summary>
public class CommandCenter : Singleton<CommandCenter>, ICenter
{
    private ActorsManagerCenter _actorsManagerCenter;

    new void Awake()
    {
        base.Awake();

        _actorsManagerCenter = ActorsManagerCenter.Instance;
    }
        
    public void CenterUpdate()
    {
        
    }
}