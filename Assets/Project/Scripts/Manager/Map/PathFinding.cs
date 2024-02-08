using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathFinding
{
    public static PathFinding Instance = new PathFinding();

    private PathFinding()
    {
    }

    // 横向走消耗1，斜着走消耗1.4（即sqrt(2)），乘以10方便计算
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private GridXZ<GridObject> grid;

    // 已经检索过的点
    private List<GridObject> openList;

    // 还未检索过的点
    private List<GridObject> closeList;

    public GridXZ<GridObject> GetGrid() => grid;

    public void Init(GridXZ<GridObject> grid)
    {
        this.grid = grid;
        openList = new List<GridObject>();
        closeList = new List<GridObject>();

        Clear();
    }

    public void Clear()
    {
        // 初始化,原点在左下角
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                GridObject pathNode = grid.GetGridObject(x, y);
                pathNode.hCost = 0;
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        openList.Clear();
        closeList.Clear();
    }

    /// <summary>
    /// 返回的是网格坐标
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPath(Vector3 start, Vector3 end)
    {
        MapSystem.Instance.GetGrid().GetXZ(start.x, start.z, out int xStart,
            out int zStart);
        MapSystem.Instance.GetGrid().GetXZ(end.x, end.z, out int xEnd, out int zEnd);

        return FindPath(xStart, zStart, xEnd, zEnd);
    }

    // public List<GridObject> FindPath(int startX, int startY, int endX, int endY)
    // {
    //     if (startX == endX && startY == endY) return null;
    //
    //     GridObject startNode = grid.GetGridObject(startX, startY);
    //     GridObject endNode = grid.GetGridObject(endX, endY);
    //
    //     // _openList.Add(startNode);
    //     openList = new List<GridObject> { startNode };
    //     closeList = new List<GridObject>();
    //
    //     // // // 初始化,原点在左下角
    //     // for (int x = 0; x < _grid.Width; x++)
    //     // {
    //     //     for (int y = 0; y < _grid.Height; y++)
    //     //     {
    //     //         GridObject pathNode = _grid.GetGridObject(x, y);
    //     //         pathNode.gCost = int.MaxValue;
    //     //         pathNode.CalculateFCost();
    //     //         // 这里不初始化hcost是因为有些点是不会经过的，没有必要去算所有的hcost，只在遍历到的时候计算hcost
    //     //         pathNode.cameFromNode = null;
    //     //     }
    //     // }
    //
    //     startNode.gCost = 0;
    //     startNode.hCost = CalculateDistanceCost(startNode, endNode);
    //     startNode.CalculateFCost();
    //
    //     while (openList.Count > 0)
    //     {
    //         // c# 没有优先队列，因此需要手动找到最近的点
    //         GridObject currentNode = GetLowestFCostNode(openList);
    //         // 如果当前最近点是终点，直接返回路径
    //         if (currentNode == endNode)
    //         {
    //             return CalculatePath(endNode);
    //         }
    //
    //         // 如果还未到达终点，则删除当前点，并加入到已经遍历过的列表中
    //         // 该点已经是最近的走法了，不需要再更新
    //         openList.Remove(currentNode);
    //         closeList.Add(currentNode);
    //
    //         // 和迪杰斯特拉一样，更新周围的点。如果当前点到目标点的消耗小于它的起始点数，则更新为当前的消耗
    //         List<GridObject> neighbourNodes = GetNeighbourList(currentNode);
    //         foreach (var neighbourNode in neighbourNodes)
    //         {
    //             if (closeList.Contains(neighbourNode)) continue;
    //
    //             if (!neighbourNode.Reachable)
    //             {
    //                 closeList.Add(neighbourNode);
    //                 continue;
    //             }
    //
    //             int currentCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
    //             if (currentCost < neighbourNode.gCost)
    //             {
    //                 neighbourNode.cameFromNode = currentNode;
    //                 neighbourNode.gCost = currentCost;
    //                 neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
    //             }
    //
    //             if (!openList.Contains(neighbourNode))
    //             {
    //                 openList.Add(neighbourNode);
    //             }
    //         }
    //     }
    //
    //     return null;
    // }

    public List<Vector2Int> FindPath(int startX, int startY, int endX, int endY)
    {
        if (startX == endX && startY == endY) return null;

        GridObject startNode = grid.GetGridObject(startX, startY);
        GridObject endNode = grid.GetGridObject(endX, endY);

        // _openList.Add(startNode);
        openList = new List<GridObject> { startNode };
        closeList = new List<GridObject>();

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            // c# 没有优先队列，因此需要手动找到最近的点
            GridObject currentNode = GetLowestFCostNode(openList);
            // 如果当前最近点是终点，直接返回路径
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            // 如果还未到达终点，则删除当前点，并加入到已经遍历过的列表中
            // 该点已经是最近的走法了，不需要再更新
            openList.Remove(currentNode);
            closeList.Add(currentNode);

            // 和迪杰斯特拉一样，更新周围的点。如果当前点到目标点的消耗小于它的起始点数，则更新为当前的消耗
            List<GridObject> neighbourNodes = GetNeighbourList(currentNode);
            foreach (var neighbourNode in neighbourNodes)
            {
                if (closeList.Contains(neighbourNode)) continue;

                if (!neighbourNode.Reachable)
                {
                    closeList.Add(neighbourNode);
                    continue;
                }

                int currentCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (currentCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = currentCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                }

                if (!openList.Contains(neighbourNode))
                {
                    openList.Add(neighbourNode);
                }
            }
        }

        return null;
    }

    private List<Vector2Int> CalculatePath(GridObject endNode)
    {
        List<Vector2Int> resPath = new List<Vector2Int>();
        while (endNode != null)
        {
            resPath.Add(new Vector2Int(endNode.X, endNode.Y));
            endNode = endNode.cameFromNode;
        }

        resPath.Reverse();
        return resPath;
    }

    // private List<GridObject> CalculatePath(GridObject endNode)
    // {
    //     List<GridObject> resPath = new List<GridObject> { endNode };
    //     
    //     while (resPath[^1].cameFromNode != null)
    //     {
    //         resPath.Add(resPath[^1].cameFromNode);
    //     }
    //
    //     resPath.Reverse();
    //     return resPath;
    // }

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
                if (neiX >= 0 && neiX < grid.Width && neiY >= 0 && neiY < grid.Height &&
                    path[x][y] != Vector2Int.zero)
                {
                    neighbourList.Add(grid.GetGridObject(neiX, neiY));
                }
            }
        }

        return neighbourList;
    }
}