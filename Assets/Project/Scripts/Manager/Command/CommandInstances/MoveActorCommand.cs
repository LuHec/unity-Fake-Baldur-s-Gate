using System;
using System.Collections.Generic;
using LuHec.Utils;
using UnityEngine;

/// <summary>
/// 移动单位指令，在初始化的时候指定移动位置。Excute可以在update中执行
/// </summary>
public class MoveActorCommand : CommandInstance
{
    private MapSystem mapSystem;
    private PathFinding pathFinding;
    private List<GridObject> path;
    private int pathPtr;
    private bool hasFound = false;
    private float targetX;
    private float targetZ;
    private Vector3 originWorldPos;

    public MoveActorCommand(float targetX, float targetZ, Vector3 originWorldPos)
    {
        mapSystem = MapSystem.Instance;
        pathFinding = PathFinding.Instance;
        this.targetX = targetX;
        this.targetZ = targetZ;
        // 记录初始位置用以回放
        this.originWorldPos = originWorldPos;
    }

    public void SetTargetPoint(float x, float z)
    {
        targetX = x;
        targetZ = z;
    }

    public override bool Excute(GameActor actor, Action onExcuteFinished)
    {
        // 计算是否能够到达目标点，不能的话就返回false，对应到commandcenter就是返回等待指令状态
        // 为了不冲突，直接把角色设置在目标位置上
        if (!hasFound)
        {
            hasFound = true;
            mapSystem.GetGrid().GetXZ(actor.transform.position.x, actor.transform.position.z, out int xStart,
                out int zStart);
            mapSystem.GetGrid().GetXZ(targetX, targetZ, out int xEnd, out int zEnd);
            path = pathFinding.FindPath(xStart, zStart, xEnd, zEnd);

            pathPtr = 1;
            if (path == null)
            {
                onExcuteFinished();
                return false;
            }
        }

        // for (int i = 0; i < path.Count - 1; i++)
        // {
        //     Debug.DrawLine(pathFinding.GetGrid().GetWorldPosition(path[i].X, path[i].Y),
        //         pathFinding.GetGrid().GetWorldPosition(path[i + 1].X, path[i + 1].Y), Color.green, 20f);
        // }
        // if (path == null)
        // {
        //     Debug.LogWarning("path is null");
        //     onExcuteFinished();
        //     return false;
        // }
        // var end = mapSystem.GetGrid().GetOffsetWorldPosition(path[pathPtr].X, path[pathPtr].Y);
        // mapSystem.MoveGameActor(end.x, end.y, actor);

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
        Vector3 nextWorldPos = mapSystem.GetGrid().GetOffsetWorldPosition(path[pathPtr].X, path[pathPtr].Y);
        Vector3 actualPos = actor.CalculateMoveTo(nextWorldPos.x, nextWorldPos.z);

        if (mapSystem.MoveGameActor(actualPos.x, actualPos.z, actor) && pathPtr < path.Count - 1)
        {
            pathPtr++;
        }

        actor.DirectMoveTo(actualPos);

        // 修正高度
        Vector3 currPos = mapSystem.GetGrid().GetOffsetWorldPosition(path[^1].X, path[^1].Y);
        currPos.y = actor.transform.position.y;

        // 由于斜角移动会遇到已存在物体，所以强制更改最后一格
        if (Vector3.Distance(actualPos, currPos) < 0.1)
        {
            mapSystem.MoveGameActor(actualPos.x, actualPos.z, actor, true);
            pathFinding.Clear();
            onMoveFinished();
        }

        return true;
    }

    private void UnDoMove(GameActor actor)
    {
        // 网格位置回放
        var gridObject = MapSystem.Instance.GetGridObject(actor.transform.position);
        gridObject.ClearActor();

        // 物理位置回放
        actor.transform.position = originWorldPos;
        MapSystem.Instance.GetGridObject(originWorldPos).SetActor(actor);
    }
}