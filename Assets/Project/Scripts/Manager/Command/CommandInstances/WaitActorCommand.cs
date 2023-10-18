using System;
using UnityEngine;

/// <summary>
/// 什么都不执行的空指令
/// </summary>
public class WaitActorCommand : CommandInstance
{
    private bool hasWaited = false;
    public override bool Excute(GameActor actor, Action onExcuteFinsihed)
    {
        if (!hasWaited)
        {
            hasWaited = true;
            actor.Wait(onExcuteFinsihed);
        }
        
        return true;
    }

    public override void Undo(GameActor actor)
    {
    }
}