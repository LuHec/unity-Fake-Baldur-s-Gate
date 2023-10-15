using System;
using System.Collections.Generic;
using LuHec.Utils;
using UnityEngine;

/// <summary>
/// 移动单位指令，在初始化的时候指定移动位置。Excute可以在update中执行
/// </summary>
public class MoveActorCommand : CommandInstance
{
    private MapSystem _mapSystem;
    private PathFinding _pathFinding;
    private List<GridObject> _path;
    private int _pathPtr;
    private bool _hasFound = false;
    private float _targetX;
    private float _targetZ;

    public MoveActorCommand(float targetX, float targetZ)
    {
        _mapSystem = MapSystem.Instance;
        _pathFinding = PathFinding.Instance;
        _targetX = targetX;
        _targetZ = targetZ;
    }

    public void SetTargetPoint(float x, float z)
    {
        _targetX = x;
        _targetZ = z;
    }

    public override bool Excute(GameActor actor, Action onExcuteFinished)
    {
        // 计算是否能够到达目标点，不能的话就返回false，对应到commandcenter就是返回等待指令状态
        if (!_hasFound)
        {
            _hasFound = true;
            _mapSystem.GetGrid().GetXZ(actor.transform.position.x, actor.transform.position.z, out int xStart,
                out int zStart);
            _mapSystem.GetGrid().GetXZ(_targetX, _targetZ, out int xEnd, out int zEnd);
            _path = _pathFinding.FindPath(xStart, zStart, xEnd, zEnd);

            _pathPtr = 1;
            if (_path == null) return false;
        }

        for (int i = 0; i < _path.Count - 1; i++)
        {
            Debug.DrawLine(_pathFinding.GetGrid().GetWorldPosition(_path[i].X, _path[i].Y),
                _pathFinding.GetGrid().GetWorldPosition(_path[i + 1].X, _path[i + 1].Y), Color.green, 20f);
        }

        return Move(actor, onExcuteFinished);
    }

    public override void Undo(GameActor actor)
    {
        UnDoMove(actor);
    }

    /// <summary>
    /// 先计算出移动点位，然后再移动实际格子，再移动物体
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="onMoveFinished"></param>
    /// <returns></returns>
    bool Move(GameActor actor, Action onMoveFinished)
    {
        Vector3 nextWorldPos = _mapSystem.GetGrid().GetOffsetWorldPosition(_path[_pathPtr].X, _path[_pathPtr].Y);
        Vector3 actualPos = actor.CalculateMoveTo(nextWorldPos.x, nextWorldPos.z);

        if (_mapSystem.MoveGameActor(actualPos.x, actualPos.z, actor) && _pathPtr < _path.Count - 1)
        {
            _pathPtr++;
        }

        actor.DirectMoveTo(actualPos);

        // 修正高度
        Vector3 currPos = _mapSystem.GetGrid().GetOffsetWorldPosition(_path[^1].X, _path[^1].Y);
        currPos.y = actor.transform.position.y;

        // 由于斜角移动会遇到已存在物体，所以强制更改最后一格
        if (Vector3.Distance(actualPos, currPos) < 0.1)
        {
            // var pos = _mapSystem.GetXZ(currPos.x, currPos.z);
            // _mapSystem.GetGrid().OnGridObjectChanged(pos.x, pos.y);
            _mapSystem.MoveGameActor(actualPos.x, actualPos.z, actor, true);
            _pathFinding.Clear();
            onMoveFinished();
        }


        return true;
    }

    private void UnDoMove(GameActor actor)
    {
    }
}