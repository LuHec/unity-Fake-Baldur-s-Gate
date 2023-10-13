using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于发布指令给指定角色，不关心具体角色和指令
/// </summary>
public class CommandCenter : ICenter
{
    private InputCommandsGenerator _inputCommandsGenerator;
    private MapSystem _mapSystem;
    private CommandInstance _commandInstanceCache;

    public CommandCenter()
    {
        _inputCommandsGenerator = new InputCommandsGenerator();
    }
    
    public CommandInstance GetCommandCache()
    {
        var res = _commandInstanceCache;
        _commandInstanceCache = null;
        return res;
    }

    public bool CanGenCommandCache => _commandInstanceCache == null;
    
    /// <summary>
    ///  返回监听的输入指令,如果没有输入则返回false
    /// </summary>
    /// <returns>CommandInstance</returns>
    public bool GenInputCommandCache()
    {
        if (!CanGenCommandCache) return false;
        _commandInstanceCache = _inputCommandsGenerator.GetInputCommand();
        return _commandInstanceCache != null;
    }

    /// <summary>
    /// 执行命令，如果是空指令不会执行
    /// </summary>
    /// <param name="cmd">命令</param>
    /// <param name="actor">对象</param>
    public void Excute(CommandInstance cmd, GameActor actor, Action onExcuteFinished)
    {
        cmd?.Excute(actor, onExcuteFinished);
    }

    public void AddCommand(CommandInstance cmd, GameActor actor)
    {
        actor.AddCommand(cmd);   
    }

    public void CenterUpdate()
    {
    }
}