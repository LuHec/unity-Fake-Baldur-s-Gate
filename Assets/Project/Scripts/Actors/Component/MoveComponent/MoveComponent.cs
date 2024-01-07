using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent
{
    public Action onMoveFinished;

    private PathFinding pathFinding;
    private GameActor actor;
    private List<Vector2Int> path;
    private int pathPtr = 0;
    private Coroutine moveCoroutine;

    private float speed = 8f;

    public MoveComponent(GameActor actor)
    {
        pathFinding = PathFinding.Instance;
        this.actor = actor;
    }

    /// <summary>
    /// 每次都要重置
    /// </summary>
    /// <param name="target"></param>
    public void Move(Vector3 target)
    {
        if (MapSystem.Instance.GetXZ(actor.transform.position.x, actor.transform.position.z) ==
            MapSystem.Instance.GetXZ(target.x, target.z)) return;
        
        if (moveCoroutine != null) actor.StopCoroutine(moveCoroutine);
        
        path = pathFinding.FindPath(actor.transform.position, target);
        pathFinding.Clear();
        pathPtr = 0;

        moveCoroutine = actor.StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Vector3 end = MapSystem.Instance.GetGrid().GetOffsetWorldPosition(path[^1].x, path[^1].y);
        Vector3 next = MapSystem.Instance.GetGrid().GetOffsetWorldPosition(path[0].x, path[0].y);
        Vector3 current = actor.transform.position;
        end.y = next.y = current.y;

        while (actor.transform.position != end)
        {
            current = actor.transform.position;
            next.y = current.y;
            end.y = current.y;

            actor.transform.position = Vector3.MoveTowards(
                current, new Vector3(next.x, current.y, next.z), speed * Time.deltaTime);
            // 检测是否到达下一个目标
            if (pathPtr < path.Count - 1 && Vector3.Distance(current, next) < 0.1f)
            {
                pathPtr++;
                next = MapSystem.Instance.GetGrid().GetOffsetWorldPosition(path[pathPtr].x, path[pathPtr].y);
            }

            yield return null;
        }
        
        onMoveFinished?.Invoke();
    }
}