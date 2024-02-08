using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveComponent
{
    public Action onMoveFinished;
    public bool BIsMoving => pathQueue.Count > 0;

    private PathFinding pathFinding;
    private GameActor actor;
    private List<Vector2Int> pathList = new List<Vector2Int>();
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private float speed = 8f;
    private bool bAllowMove = true;

    public MoveComponent(GameActor actor)
    {
        pathFinding = PathFinding.Instance;
        this.actor = actor;
    }

    public void UpdateMove()
    {
        // 当可以移动，且有路径点时才可以行动
        if (!bAllowMove || !BIsMoving) return;

        var current = actor.transform.position;
        if (Vector3.Distance(current, pathQueue.Peek()) < 0.1f)
        {
            pathQueue.Dequeue();

            if (pathQueue.Count == 0)
            {
                Debug.Log("pathQueue.Count");
                // 寻路完毕
                onMoveFinished?.Invoke();
            }
        }
        else
        {
            actor.transform.position = Vector3.MoveTowards(
                current, pathQueue.Peek(), speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 世界坐标
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Vector3 target)
    {
        if (!bAllowMove) return;

        if (MapSystem.Instance.GetXZ(actor.transform.position.x, actor.transform.position.z) ==
            MapSystem.Instance.GetXZ(target.x, target.z)) return;

        ClearTarget();

        pathList = pathFinding.FindPath(actor.transform.position, target);
        pathFinding.Clear();

        // 转换为路径队列
        foreach (var node in pathList)
        {
            Vector3 nodeV3 = MapSystem.Instance.GetGrid().GetOffsetWorldPosition(node.x, node.y);
            nodeV3.y = actor.transform.position.y;
            pathQueue.Enqueue(nodeV3);
        }
    }

    public void ClearTarget()
    {
        pathList.Clear();
        pathQueue.Clear();

        onMoveFinished?.Invoke();
    }

    public void DisableMove()
    {
        ClearTarget();
        bAllowMove = false;
    }

    public void EnableMove()
    {
        bAllowMove = true;
    }
}