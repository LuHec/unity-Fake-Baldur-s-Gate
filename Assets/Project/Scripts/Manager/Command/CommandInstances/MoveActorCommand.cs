using System;
using System.Collections;
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
        hasExecuted = true;

        // 计算是否能够到达目标点，不能的话就返回false，对应到commandcenter就是返回等待指令状态
        // 为了不冲突，直接把角色设置在目标位置上

        mapSystem.GetGrid().GetXZ(actor.transform.position.x, actor.transform.position.z, out int xStart,
            out int zStart);
        mapSystem.GetGrid().GetXZ(targetX, targetZ, out int xEnd, out int zEnd);

        // 寻路后还原
        // path = pathFinding.FindPath(xStart, zStart, xEnd, zEnd);
        pathFinding.Clear();

        pathPtr = 1;
        if (path == null)
        {
            onExcuteFinished?.Invoke();
            isRunning = false;
            return false;
        }


        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(pathFinding.GetGrid().GetWorldPosition(path[i].X, path[i].Y),
                pathFinding.GetGrid().GetWorldPosition(path[i + 1].X, path[i + 1].Y), Color.green, 20f);
        }

        coroutine = CommandCenter.Instance.StartCoroutine(MoveCoroutine(actor, onExcuteFinished));

        return true;
    }

    public override void Undo(GameActor actor)
    {
        UnDoMove(actor);
    }

    public IEnumerator MoveCoroutine(GameActor actor, Action onMoveFinished)
    {
        while (isRunning)
        {
            Vector3 nextWorldPos = mapSystem.GetGrid().GetOffsetWorldPosition(path[pathPtr].X, path[pathPtr].Y);
            // Vector3 actualPos = actor.CalculateMoveTo(nextWorldPos.x, nextWorldPos.z);
            // Vector3 actualPos = actor.MoveTo(nextWorldPos.x, nextWorldPos.z);


            // // 判定是否进入下个格子范围了
            // var currentGrid = mapSystem.GetXZ(actualPos.x, actualPos.z);
            // if (pathPtr < path.Count - 1 && (currentGrid.x != path[pathPtr].X || currentGrid.y != path[pathPtr].Y))
            // {
            //     pathPtr++;
            // }


            // // 修正高度
            // Vector3 lastPos = mapSystem.GetGrid().GetOffsetWorldPosition(path[^1].X, path[^1].Y);
            //
            // if (Vector2.Distance(new Vector2(actualPos.x, actualPos.z), new Vector2(lastPos.x, lastPos.z)) < 0.1f)
            // {
            //     onMoveFinished?.Invoke();
            //     isRunning = false;
            // }

            yield return null;
        }
    }

    private void UnDoMove(GameActor actor)
    {
        // 网格位置回放
        var gridObject = MapSystem.Instance.GetGridObject(actor.transform.position);

        // 物理位置回放
        actor.transform.position = originWorldPos;
    }

    // /// <summary>
    // /// 先计算出移动点位，然后再移动实际格子，再移动物体
    // /// </summary>
    // /// <param name="actor"></param>
    // /// <param name="onMoveFinished"></param>
    // /// <returns></returns>
    // bool Move(GameActor actor, Action onMoveFinished)
    // {
    //     Vector3 nextWorldPos = mapSystem.GetGrid().GetOffsetWorldPosition(path[pathPtr].X, path[pathPtr].Y);
    //     // Vector3 actualPos = actor.CalculateMoveTo(nextWorldPos.x, nextWorldPos.z);
    //     Vector3 actualPos = actor.MoveTo(nextWorldPos.x, nextWorldPos.z);
    //     
    //
    //     // 判定是否进入下个格子范围了
    //     var currentGrid = mapSystem.GetXZ(actualPos.x, actualPos.z);
    //     if (pathPtr < path.Count - 1 && (currentGrid.x != path[pathPtr].X || currentGrid.y != path[pathPtr].Y))
    //     {
    //         pathPtr++;
    //     }
    //     
    //
    //     // 修正高度
    //     Vector3 lastPos = mapSystem.GetGrid().GetOffsetWorldPosition(path[^1].X, path[^1].Y);
    //
    //     if (Vector2.Distance(new Vector2(actualPos.x, actualPos.z), new Vector2(lastPos.x, lastPos.z)) < 0.1f)
    //     {
    //         onMoveFinished?.Invoke();
    //         IsRunning = false;
    //     }
    //
    //     return true;
    // }
}