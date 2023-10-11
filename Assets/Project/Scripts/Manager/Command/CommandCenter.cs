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

    public CommandCenter(MapSystem mapSystem)
    {
        _mapSystem = mapSystem;
        _inputCommandsGenerator = new InputCommandsGenerator(_mapSystem);
    }
    
    /// <summary>
    ///  返回监听的输入指令
    /// </summary>
    /// <returns>CommandInstance</returns>
    public CommandInstance GetInputCommand()
    {
        return _inputCommandsGenerator.GetInputCommand();
    }

    /// <summary>
    /// 执行命令，如果是空指令不会执行
    /// </summary>
    /// <param name="cmd">命令</param>
    /// <param name="actor">对象</param>
    public void Excute(CommandInstance cmd, GameActor actor)
    {
        cmd?.Excute(actor);
    }
        
    public void CenterUpdate()
    {
        
    }
}