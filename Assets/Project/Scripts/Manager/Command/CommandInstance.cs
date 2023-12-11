
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CommandInstance
{
    public bool IsRunning = true;
    
    /// <summary>
    /// 选择对象执行命令 
    /// </summary>
    /// <param name="actor">要控制的对象</param>
    public abstract bool Excute(GameActor actor, Action onExcuteFinsihed);

    /// <summary>
    /// 选择对象撤销操作
    /// </summary>
    /// <param name="actor">要控制的对象</param>
    public abstract void Undo(GameActor actor);
}
