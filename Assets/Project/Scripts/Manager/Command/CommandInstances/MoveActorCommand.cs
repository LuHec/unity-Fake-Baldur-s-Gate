using LuHec.Utils;
using UnityEngine;

/// <summary>
/// 移动单位指令，在初始化的时候指定移动位置。Excute可以在update中执行
/// </summary>
public class MoveActorCommand : CommandInstance
{
    private MapSystem _mapSystem;
    private float _targetX;
    private float _targetZ;
    
    public MoveActorCommand(MapSystem mapSystem, float targetX, float targetZ)
    {
        _mapSystem = mapSystem;
        _targetX = targetX;
        _targetZ = targetZ;
    }

    public void SetTargetPoint(float x, float z)
    {
        _targetX = x;
        _targetZ = z;
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
        if (_mapSystem.MoveGameActor(_targetX, _targetZ, actor))
        {
            actor.MoveTo(_targetX, _targetZ);    
        }
        
    }
    
    private void UnDoMove(GameActor actor)
    {
        
    }
}