using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于发布指令给指定角色，不关心具体角色和指令
/// </summary>
public class CommandCenter : Singleton<CommandCenter>
{
    private InputCommandsGenerator _inputCommandsGenerator;
    private MapSystem _mapSystem;
    private ActorsManagerCenter _actorsManagerCenter;
    
    /// <summary>
    /// 执行命令，如果是空指令不会执行，直接返回错误
    /// </summary>
    /// <param name="cmd">命令</param>
    /// <param name="actor">对象</param>
    public bool Excute(CommandInstance cmd, GameActor actor, Action onExcuteFinished)
    {
        if (cmd == null) return false;
        return cmd.Excute(actor, onExcuteFinished);
    }
    
    /// <summary>
    /// 执行命令，如果是空指令不会执行，直接返回错误
    /// </summary>
    /// <param name="cmd">命令</param>
    /// <param name="actor">对象</param>
    public bool Excute(CommandInstance cmd, uint dynamicId, Action onExcuteFinished)
    {
        if (cmd == null) return false;
        return cmd.Excute(_actorsManagerCenter.GetActorByDynamicId(dynamicId), onExcuteFinished);
    }

    // public void AddCommand(CommandInstance cmd, GameActor actor)
    // {
    //     actor.AddCommand(cmd);   
    // }
    //
    // public void AddCommand(CommandInstance cmd, uint dynamicId)
    // {
    //     _actorsManagerCenter.GetActorByDynamicId(dynamicId)?.AddCommand(cmd);   
    // }

    #region Generate Command
    
    public CommandInstance GetMoveActorCommand(GameActor actor, Vector3 position)
    {
        return new MoveActorCommand(position.x, position.z, actor.transform.position);
    }

    public CommandInstance GetAttackActorCommand(GameActor gridActor)
    {
        return new AttackActorCommand(gridActor);
    }

    #endregion
    
}