
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CommandInstance
{
    public bool isRunning = true;
    public bool hasExecuted = false;
    public Coroutine coroutine;

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

    /// <summary>
    /// 中断命令执行
    /// </summary>
    public void Interrupt()
    {
        isRunning = false;
        hasExecuted = true;
        
        if (coroutine != null)
        {
            CommandCenter.Instance.StopCoroutine(coroutine);
        }
    }
}
