using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Pathfiding
{
    // 横向走消耗1，斜着走消耗1.4（即sqrt(2)），乘以10方便计算
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private GridXZ<PathNode> _grid;

    // 已经检索过的点
    private List<PathNode> _openList;

    // 还未检索过的点
    private List<PathNode> _closeList;

    private int _width;
    private int _height;
    private int _cellSize;

    public GridXZ<PathNode> GetGrid() => _grid;

    public Pathfiding(int width, int height, int cellSize)
    {
        _width = width;
        _height = height;

        _grid = new GridXZ<PathNode>(width, height, cellSize, Vector3.zero,
            (grid, x, z) => { return new PathNode(grid, x, z); });
        
    }

    public void Clear()
    {
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                PathNode pathNode = _grid.GetGridObject(x, y);
                pathNode.gCost = 0;
                pathNode.hCost = 0;
                pathNode.fCost = 0;
                pathNode.cameFromNode = null;
            }
        }
        
        _openList.Clear();
        _closeList.Clear();
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = _grid.GetGridObject(startX, startY);
        PathNode endNode = _grid.GetGridObject(endX, endY);

        _openList = new List<PathNode> { startNode };
        _closeList = new List<PathNode>();

        // 初始化,原点在左下角
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                PathNode pathNode = _grid.GetGridObject(x, y);
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
            PathNode currentNode = GetLowestFCostNode(_openList);
            Debug.Log(currentNode.x + " " + currentNode.y);
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
            List<PathNode> neighbourNodes = GetNeighbourList(currentNode);
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

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> resPath = new List<PathNode>();
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
    /// <param name="a"></param>
    /// <param name="b"></param>
    public int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        // 斜角移动，规定只能经过方格的顶点，因此移动格数取最小的那个，剩下的横向或者纵向移动
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    public PathNode GetLowestFCostNode(List<PathNode> pathNodesList)
    {
        PathNode lowestPathNode = pathNodesList[0];
        foreach (var pathNode in pathNodesList)
        {
            if (pathNode.fCost < lowestPathNode.fCost) lowestPathNode = pathNode;
        }

        return lowestPathNode;
    }

    /// <summary>
    /// 返回当前点周围的8个点
    /// </summary>
    /// <param name="currentNode"></param>
    /// <returns></returns>
    List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        List<List<Vector2Int>> path = new List<List<Vector2Int>>();
        path.Add(new List<Vector2Int> { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1) });
        path.Add(new List<Vector2Int> { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) });
        path.Add(new List<Vector2Int> { new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) });
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                int neiX = currentNode.x + path[x][y].x;
                int neiY = currentNode.y + path[x][y].y;
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