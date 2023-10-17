using System.Collections.Generic;
using CodeMonkey.Utils;
using LuHec.Utils;
using UnityEngine;

public class BuildingSystem : Singleton<BuildingSystem>
{
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSos;
    [SerializeField] private PlacedObjectTypeSO.Dir _dir = PlacedObjectTypeSO.Dir.Up;

    private PlacedObjectTypeSO _placedObjectTypeSo;
    private int _ptr = 0;
    private GridXZ<GridObject> _grid;
    private MapDataToJsonSystem _mapDataToJsonSystem;

    void Start()
    {
        _grid = MapSystem.Instance.GetGrid();
        _placedObjectTypeSo = placedObjectTypeSos[0];

        _mapDataToJsonSystem = new MapDataToJsonSystem();
    }

    public void Update()
    {
        if (MessageCenter.Instance.globalState.EditMode)
        {
            if (PlayerInput.Instance.IsLClick)
            {
                SetBuilding();
            }

            if (PlayerInput.Instance.IsRClick)
            {
                _mapDataToJsonSystem.SaveMap();
            }
        }
    }

    public void ChangeBuilding()
    {
        _ptr = (_ptr + 1) % placedObjectTypeSos.Count;
        _placedObjectTypeSo = placedObjectTypeSos[_ptr];
    }

    /// <summary>
    /// 销毁格子上的建筑
    /// </summary>
    private void DestoryBuilding()
    {
        GridObject gridObject =
            _grid.GetGridObject(PlayerInput.Instance.GetMouse3DPosition(LayerMask.GetMask("Default")));
        PlacedObject placedObject = gridObject.GetPlaceObject();
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
        Vector3 mousePos = PlayerInput.Instance.GetMouse3DPosition(LayerMask.GetMask("Default"));
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
            _mapDataToJsonSystem.RecordBuild(mousePos);
            Vector2Int objRotationOffset = _placedObjectTypeSo.GetRotationOffset(_dir);
            Vector3 worldPos = _grid.GetWorldPosition(x, z) +
                               new Vector3(objRotationOffset.x, 0, objRotationOffset.y) * MapSystem.Instance.GetGrid().Cellsize;
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