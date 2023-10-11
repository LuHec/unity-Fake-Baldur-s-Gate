using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using LuHec.Utils;
using UnityEngine;
using UnityEngine.Serialization;

public class Grid3DSystem : MonoBehaviour
{
    private GridXZ<GridObjectOrigin> _grid;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSos;
    [SerializeField] private int gridwidth = 10;
    [SerializeField] private int gridheight = 10;
    [SerializeField] private float cellsize = 10f;
    [SerializeField] private PlacedObjectTypeSO.Dir _dir = PlacedObjectTypeSO.Dir.Up;

    private PlacedObjectTypeSO _placedObjectTypeSo;
    private int _ptr = 0;

    void ChangeBuilding()
    {
        _ptr = (_ptr + 1) % placedObjectTypeSos.Count;
        _placedObjectTypeSo = placedObjectTypeSos[_ptr];
    }

    private void Awake()
    {
        _grid = new GridXZ<GridObjectOrigin>(gridwidth, gridheight, cellsize, Vector3.zero,
            (GridXZ<GridObjectOrigin> g, int x, int y) => new GridObjectOrigin(g, x, y));

        _placedObjectTypeSo = placedObjectTypeSos[0];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _dir = _placedObjectTypeSo.GetNextDir(_dir);
            UtilsClass.CreateWorldTextPopup(_dir.ToString(), Utilties.GetMouse3DPosition("Default"));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeBuilding();
        }

        if (Input.GetMouseButtonDown(0))
        {
            SetBuilding();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DestoryBuilding();
        }
    }

    /// <summary>
    /// 销毁格子上的建筑
    /// </summary>
    private void DestoryBuilding()
    {
        GridObjectOrigin gridObjectOrigin = _grid.GetGridObject(Utilties.GetMouse3DPosition("Default"));
        PlacedObject placedObject = gridObjectOrigin.GetPlaceObject();
        if (placedObject != null)
        {
            List<Vector2Int> gridList =
                placedObject.GetGridPositionList();

            foreach (var gridPos in gridList)
            {
                _grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
            }
            
            placedObject.DestroySelf();
        }
    }

    /// <summary>
    /// 在鼠标处格子建造建筑
    /// </summary>
    void SetBuilding()
    {
        Vector3 mousePos = Utilties.GetMouse3DPosition("Default");
        _grid.GetXZ(mousePos, out int x, out int z);
        List<Vector2Int> gridList =
            _placedObjectTypeSo.GetGridPositionList(new Vector2Int(x, z), _dir);

        // 遍历所有占领的格子，只有全部可以建造才能建造
        bool canBuild = true;
        foreach (var gridPos in gridList)
        {
            if (!_grid.GetGridObject(gridPos.x, gridPos.y).CanBuild())
            {
                canBuild = false;
                break;
            }
        }

        if (canBuild)
        {
            Vector2Int objRotationOffset = _placedObjectTypeSo.GetRotationOffset(_dir);
            Vector3 worldPos = _grid.GetWorldPosition(x, z) +
                               new Vector3(objRotationOffset.x, 0, objRotationOffset.y) * cellsize;
            PlacedObject placedObject =
                PlacedObject.Create(worldPos, new Vector2Int(x, z), _dir, _placedObjectTypeSo);

            foreach (var gridPos in gridList)
            {
                _grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObect(placedObject);
            }
        }
        else
        {
            UtilsClass.CreateWorldTextPopup("Can't create here", mousePos);
        }
    }
}