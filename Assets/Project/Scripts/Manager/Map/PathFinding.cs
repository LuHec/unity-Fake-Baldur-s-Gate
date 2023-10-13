using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathFinding : Singleton<PathFinding>
{
    // 横向走消耗1，斜着走消耗1.4（即sqrt(2)），乘以10方便计算
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private GridXZ<GridObject> _grid;

    // 已经检索过的点
    private List<GridObject> _openList;

    // 还未检索过的点
    private List<GridObject> _closeList;

    public GridXZ<GridObject> GetGrid() => _grid;

    public void Init(GridXZ<GridObject> grid)
    {
        _grid = grid;
    }

    public void Clear()
    {
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                GridObject pathNode = _grid.GetGridObject(x, y);
                pathNode.gCost = 0;
                pathNode.hCost = 0;
                pathNode.fCost = 0;
                pathNode.cameFromNode = null;
            }
        }

        _openList.Clear();
        _closeList.Clear();
    }

    public List<GridObject> FindPath(int startX, int startY, int endX, int endY)
    {
        GridObject startNode = _grid.GetGridObject(startX, startY);
        GridObject endNode = _grid.GetGridObject(endX, endY);

        _openList = new List<GridObject> { startNode };
        _closeList = new List<GridObject>();

        // 初始化,原点在左下角
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                GridObject pathNode = _grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                // 这里不初始化hcost是因为有些点是不会经过的，没有必要去算所有的hcost，只在遍历到的时候计算hcost
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (_openList.Count > 0)
        {
            // c# 没有优先队列，因此需要手动找到最近的点
            GridObject currentNode = GetLowestFCostNode(_openList);
            // 如果当前最近点是终点，直接返回路径
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            // 如果还未到达终点，则删除当前点，并加入到已经遍历过的列表中
            // 该点已经是最近的走法了，不需要再更新
            _openList.Remove(currentNode);
            _closeList.Add(currentNode);

            // 和迪杰斯特拉一样，更新周围的点。如果当前点到目标点的消耗小于它的起始点数，则更新为当前的消耗
            List<GridObject> neighbourNodes = GetNeighbourList(currentNode);
            foreach (var neighbourNode in neighbourNodes)
            {
                if (_closeList.Contains(neighbourNode)) continue;

                if (!neighbourNode.Reachable)
                {
                    _closeList.Add(neighbourNode);
                    continue;
                }

                int currentCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (currentCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = currentCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                }

                if (!_openList.Contains(neighbourNode))
                {
                    _openList.Add(neighbourNode);
                }
            }
        }

        return null;
    }

    private List<GridObject> CalculatePath(GridObject endNode)
    {
        List<GridObject> resPath = new List<GridObject>();
        resPath.Add(endNode);
        while (resPath[^1].cameFromNode != null)
        {
            resPath.Add(resPath[^1].cameFromNode);
        }

        resPath.Reverse();
        return resPath;
    }

    /// <summary>
    /// 计算从A走到B需要的最小点数
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    public int CalculateDistanceCost(GridObject startNode, GridObject endNode)
    {
        int xDistance = Mathf.Abs(startNode.X - endNode.X);
        int yDistance = Mathf.Abs(startNode.Y - endNode.Y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        // 斜角移动，规定只能经过方格的顶点，因此移动格数取最小的那个，剩下的横向或者纵向移动
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    public GridObject GetLowestFCostNode(List<GridObject> pathNodesList)
    {
        GridObject lowestGridObject = pathNodesList[0];
        foreach (var pathNode in pathNodesList)
        {
            if (pathNode.fCost < lowestGridObject.fCost) lowestGridObject = pathNode;
        }

        return lowestGridObject;
    }

    /// <summary>
    /// 返回当前点周围的8个点
    /// </summary>
    /// <param name="currentNode"></param>
    /// <returns></returns>
    List<GridObject> GetNeighbourList(GridObject currentNode)
    {
        List<GridObject> neighbourList = new List<GridObject>();
        List<List<Vector2Int>> path = new List<List<Vector2Int>>();
        path.Add(new List<Vector2Int> { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1) });
        path.Add(new List<Vector2Int> { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) });
        path.Add(new List<Vector2Int> { new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) });
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                int neiX = currentNode.X + path[x][y].x;
                int neiY = currentNode.Y + path[x][y].y;
                if (neiX >= 0 && neiX < _grid.Width && neiY >= 0 && neiY < _grid.Height &&
                    path[x][y] != Vector2Int.zero)
                {
                    neighbourList.Add(_grid.GetGridObject(neiX, neiY));
                }
            }
        }

        return neighbourList;
    }
}