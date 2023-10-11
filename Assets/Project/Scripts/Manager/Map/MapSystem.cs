﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局地图管理器
/// </summary>
public class MapSystem : ICenter
{
    private GridXZ<GridObject> _grid;
    private int _gridwidth;
    private int _gridheight;
    private float _cellsize;
    private Vector3 _originPos;
    
    public MapSystem(int gridheight, int gridwidth, float cellsize, Vector3 originPos)
    {
        _gridwidth = gridwidth;
        _gridheight = gridheight;
        _cellsize = cellsize;
        _originPos = originPos;

        _grid = new GridXZ<GridObject>(_gridwidth, _gridheight, _cellsize, _originPos, 
            (GridXZ<GridObject> grid, int x, int y)=>new GridObject(grid, x, y));
    }

    /// <summary>
    /// 返回坐标对应格子上的GameActor
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>GameActor</returns>
    public GameActor GetGridActor(float x, float z)
    {
        return GetGridObject((int)x, (int)z)?.getGameActor();
    }
    
    public GameActor GetGridActor(Vector3 worldPosition)
    {
        return GetGridActor(worldPosition.x, worldPosition.z);
    }

    /// <summary>
    /// 返回坐标对应格子
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>GridObject</returns>
    public GridObject GetGridObject(float x, float z)
    {
        return _grid.GetGridObject((int)x, (int)z);
    }

    public GridObject GetGridObject(Vector3 worldPosition)
    {
        return GetGridObject(worldPosition.x, worldPosition.z);
    }

    /// <summary>
    /// 指定位置上添加Actor
    /// </summary>
    /// <returns></returns>
    public bool SetGridActor(float x, float z, GameActor actor)
    {
        if (GetGridActor(x, z) == null)
        {
            _grid.GetGridObject((int)x, (int)z).SetActor(actor);
        }

        return false;
    }

    /// <summary>
    /// 把actor移动到指定位置
    /// </summary>
    /// <param name="targetX">目标x</param>
    /// <param name="targetZ">目标y</param>
    /// <param name="actor">指定目标</param>
    /// <returns>移动是否成功</returns>
    public bool MoveGameActor(float targetX, float targetZ, GameActor actor)
    {
        if (PathFind())
        {
            List<Vector2Int> gridList =
                actor.PlacedObject.GetGridPositionList(new Vector2Int((int)actor.X, (int)actor.Z),
                    PlacedObjectTypeSO.Dir.Down);

            foreach (var gridPos in gridList)
            {
                GridObject gridObject = GetGridObject(gridPos.x, gridPos.y);
                gridObject.ClearActor();
            }

            List<Vector2Int> gridTargetList =
                actor.PlacedObject.GetGridPositionList(new Vector2Int((int)targetX, (int)targetZ),
                    PlacedObjectTypeSO.Dir.Down);
            
            foreach (var gridTargetPos in gridTargetList)
            {
                GridObject gridObject = GetGridObject(gridTargetPos.x, gridTargetPos.y);
                gridObject.ClearActor();
            }
            
            return true;
        }

        return false;
    }
    
    public bool PathFind()
    {
        return true;
    }
    
    public void CenterUpdate()
    {
        
    }
}