﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局地图管理器
/// </summary>
public class MapSystem : Singleton<MapSystem>
{
    private GridXZ<GridObject> _grid;
    [SerializeField] private int _gridwidth = 10;
    [SerializeField] private int _gridheight = 10;
    [SerializeField] private float _cellsize = 10f;
    [SerializeField] private Vector3 _originPos = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        InitMap();
    }

    public void InitMap()
    {
        _grid = new GridXZ<GridObject>(_gridwidth, _gridheight, _cellsize, _originPos,
            (GridXZ<GridObject> grid, int x, int y) => new GridObject(grid, x, y));
    }

    public GridXZ<GridObject> GetGrid() => _grid;


    /// <summary>
    /// 返回坐标对应格子上的GameActor
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>GameActor</returns>
    public GameActor GetGridActor(float x, float z)
    {
        return GetGridObject((int)x, (int)z)?.GetActor();
    }

    public GameActor GetGridActor(Vector3 worldPosition)
    {
        return GetGridActor(worldPosition.x, worldPosition.z);
    }

    public Vector2Int GetXZ(float x, float z)
    {
        _grid.GetXZ(x, z, out int x1, out int z1);
        return new Vector2Int(x1, z1);
    }

    /// <summary>
    /// 返回坐标对应格子，参数需要计算后坐标
    /// </summary>
    /// <param name="x">计算后坐标</param>
    /// <param name="z">计算后坐标</param>
    /// <returns>GridObject</returns>
    public GridObject GetGridObject(int x, int z)
    {
        return _grid.GetGridObject(x, z);
    }

    public GridObject GetGridObject(float x, float y)
    {
        var vec2 = GetXZ(x, y);
        return GetGridObject(vec2.x, vec2.y);
    }
    
    public GridObject GetGridObject(Vector3 worldPosition)
    {
        return GetGridObject(worldPosition.x, worldPosition.z);
    }

    /// <summary>
    /// 指定位置上添加Actor，需要世界坐标
    /// </summary>
    /// <returns></returns>
    public bool SetGridActor(float x, float z, GameActor actor)
    {
        var gridPos = GetXZ(x, z);
        if (GetGridActor(gridPos.x, gridPos.y) == null)
        {
            _grid.GetGridObject(gridPos.x, gridPos.y).SetActor(actor);
        }

        return false;
    }
    
    /// <summary>
    /// 指定位置上添加Actor,需要格子坐标
    /// </summary>
    /// <returns></returns>
    public bool SetGridActor(int x, int z, GameActor actor)
    {
        if (GetGridActor(x, z) == null)
        {
            _grid.GetGridObject(x, z).SetActor(actor);
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
    public bool MoveGameActor(float targetX, float targetZ, GameActor actor, bool force = false)
    {
        // 移动只在格子范围内不进行修改
        _grid.GetXZ(new Vector2(actor.transform.position.x, actor.transform.position.z), out int x1, out int z1);
        _grid.GetXZ(new Vector2(targetX, targetZ), out int x2, out int z2);

        if (force)
        {
            _grid.GetGridObject(x1, z1).ClearActor();
            _grid.GetGridObject(x2, z2).SetActor(actor);
            return false;
        }
        
        if (x1 == x2 && z1 == z2) return false;


        if (_grid.GetGridObject(x1, z1).GetActor() == null || _grid.GetGridObject(x1, z1).GetActor() == actor)
        {
            _grid.GetGridObject(x1, z1).ClearActor();
            _grid.GetGridObject(x2, z2).SetActor(actor);
        }

        // // 原始位置
        // List<Vector2Int> gridList =
        //     actor.PlacedObject.GetGridPositionList(new Vector2Int(x1, z1),
        //         PlacedObjectTypeSO.Dir.Up);
        //
        // foreach (var gridPos in gridList)
        // {
        //     GridObject gridObject = GetGridObject(gridPos.x, gridPos.y);
        //     gridObject.ClearActor();
        // }
        //
        // // 目标位置
        //
        // List<Vector2Int> gridTargetList =
        //     actor.PlacedObject.GetGridPositionList(new Vector2Int(x2, z2),
        //         PlacedObjectTypeSO.Dir.Up);
        //
        // foreach (var gridTargetPos in gridTargetList)
        // {
        //     GridObject gridObject = GetGridObject(gridTargetPos.x, gridTargetPos.y);
        //     gridObject.SetActor(actor);
        // }

        return true;
    }
}