
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CommandInstance
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="actor">要控制的对象</param>
    public abstract void Excute(GameActor actor);

    /// <summary>
    /// 撤销操作
    /// </summary>
    /// <param name="actor">要控制的对象</param>
    public abstract void Undo(GameActor actor);
}
