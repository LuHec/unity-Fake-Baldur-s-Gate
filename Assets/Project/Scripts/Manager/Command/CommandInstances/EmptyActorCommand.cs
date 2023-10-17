using System;

/// <summary>
/// 什么都不执行的空指令
/// </summary>
public class EmptyActorCommand : CommandInstance
{
    public override bool Excute(GameActor actor, Action onExcuteFinsihed)
    {
        onExcuteFinsihed?.Invoke();
        return true;
    }

    public override void Undo(GameActor actor)
    {
    }
}