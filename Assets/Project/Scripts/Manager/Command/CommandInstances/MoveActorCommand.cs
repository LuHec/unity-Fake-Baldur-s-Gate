using LuHec.Utils;
using UnityEngine;

/// <summary>
/// 移动单位指令 
/// </summary>
public class MoveActorCommand : CommandInstance
{
    private float _x;
    private float _y;
    
    public MoveActorCommand(float x, float y)
    {
        _x = x;
        _y = y;
    }

    public void SetPoint(float x, float y)
    {
        _x = x;
        _y = y;
    }

    public override void Excute(GameActor actor)
    {
        Move(actor);
    }

    public override void Undo(GameActor actor)
    {
        
    }

    void Move(GameActor actor)
    {
        actor.MoveUnit(_x, _y);
    }
}