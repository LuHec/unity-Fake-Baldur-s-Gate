using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Unity.VisualScripting;

public class GridXZ<TGridObject>
{
    public int Width => _width;
    public int Height => _height;
    public float Cellsize => _cellsize;
    
    public event EventHandler<OnGridChangedEventArgs> OnGridChanged;

    public class OnGridChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }

    public void Submit(EventHandler<OnGridChangedEventArgs> listener)
    {
        OnGridChanged += listener;
    }

    private int _width;
    private int _height;
    private float _cellsize;
    private Vector3 _originPos;
    TGridObject[,] _gridArray;
    private TextMesh[,] _debugTextArray;

    /// <summary>
    ///  初始化格子并画线
    /// </summary>
    /// <param name="width">横向数量</param>
    /// <param name="height">纵向数量</param>
    /// <param name="cellsize">格子长度</param>
    public GridXZ(int width, int height, float cellsize, Vector3 startPos, Func<GridXZ<TGridObject>, int, int, TGridObject> CreateGridObject)
    {
        _width = width;
        _height = height;
        _cellsize = cellsize;
        _originPos = startPos;

        _gridArray = new TGridObject[_width, _height];
        _debugTextArray = new TextMesh[_width, _height];

        // Submit(UpdateMap);

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                _gridArray[x, z] = CreateGridObject(this, x, z);
                // _debugTextArray[x, z] = UtilsClass.CreateWorldText(_gridArray[x, z].ToString(), null,
                //     GetWorldPosition(x, z) + new Vector3(0.5f * cellsize, 0, 0.5f * cellsize), 8, Color.black,
                //     TextAnchor.MiddleCenter, TextAlignment.Center);

                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 1000f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 1000f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, 1000f);
        Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, 1000f);
    }

    void UpdateMap(object sender, OnGridChangedEventArgs arg)
    {
        _debugTextArray[arg.x, arg.z].text = _gridArray.GetValue(arg.x, arg.z)?.ToString();
    }
    
    // 返回格子对应的世界坐标
    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * _cellsize + _originPos;
    }
    
    public Vector3 GetOffsetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * _cellsize + _originPos + new Vector3(_cellsize / 2, 0, _cellsize / 2) ;
    }


    public void GetXZ(Vector3 worldPos, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPos - _originPos).x / _cellsize);
        z = Mathf.FloorToInt((worldPos - _originPos).z / _cellsize);
    }

    public void GetXZ(Vector2 xzPos, out int x, out int z)
    {
        x = Mathf.FloorToInt((xzPos.x - _originPos.x) / _cellsize);
        z = Mathf.FloorToInt((xzPos.y - _originPos.z) / _cellsize);
    }
    
    public void GetXZ(float x1, float z1, out int x, out int z)
    {
        x = Mathf.FloorToInt((x1 - _originPos.x) / _cellsize);
        z = Mathf.FloorToInt((z1 - _originPos.z) / _cellsize);
    }

    public void SetGridObject(int x, int z, TGridObject val)
    {
        if (x < 0 || x >= _width || z < 0 || z >= _height) return;

        _gridArray[x, z] = val;
    }

    /// <summary>
    /// 设置格子值（直接替换）
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="val"></param>
    public void SetGridObject(Vector3 worldPos, TGridObject val)
    {
        GetXZ(worldPos, out var x, out var z);
        SetGridObject(x, z, val);
        
        OnGridObjectChanged(x, z);
    }

    public TGridObject GetGridObject(int x, int z)
    {
        if (x < 0 || x >= _width || z < 0 || z >= _height) return default(TGridObject);

        return _gridArray[x, z];
    }

    public TGridObject GetGridObject(Vector3 worldPos)
    {
        GetXZ(worldPos, out var x, out var z);
        return GetGridObject(x, z);
    }

    /// <summary>
    /// 当改变格子时触发事件,当外部想要直接修改格子本体而非替换本体时使用
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void OnGridObjectChanged(int x, int z)
    {
        // OnGridChanged?.Invoke(this, new OnGridChangedEventArgs{x = x, z = z});
    }
}