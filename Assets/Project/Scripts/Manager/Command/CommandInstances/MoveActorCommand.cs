using LuHec.Utils;
using UnityEngine;

/// <summary>
/// 移动单位指令，在初始化的时候指定移动位置。Excute可以在update中执行
/// </summary>
public class MoveActorCommand : CommandInstance
{
    private MapSystem _mapSystem;
    private float _x;
    private float _z;
    
    public MoveActorCommand(MapSystem mapSystem, float x, float z)
    {
        _mapSystem = mapSystem;
        _x = x;
        _z = z;
    }

    public void SetTargetPoint(float x, float z)
    {
        _x = x;
        _z = z;
    }

    public override void Excute(GameActor actor)
    {
        Move(actor);
    }

    public override void Undo(GameActor actor)
    {
        UnDoMove(actor);
    }

    void Move(GameActor actor)
    {
        _mapSystem.MoveGameActor(_x, _z, actor);
        actor.MoveTo(_x, _z);
    }
    
    private void UnDoMove(GameActor actor)
    {
        
    }
}