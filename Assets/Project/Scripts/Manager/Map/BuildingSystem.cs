using System.Collections.Generic;
using CodeMonkey.Utils;
using LuHec.Utils;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildingSystem : Singleton<BuildingSystem>
{
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSos;
    [SerializeField] private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Up;

    private PlacedObjectTypeSO placedObjectTypeSo;
    private int ptr = 0;
    private GridXZ<GridObject> grid;
    private MapDataToJsonSystem mapDataToJsonSystem;

    void Start()
    {
        grid = MapSystem.Instance.GetGrid();
        placedObjectTypeSo = placedObjectTypeSos[0];

        mapDataToJsonSystem = new MapDataToJsonSystem();
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
                mapDataToJsonSystem.SaveMap();
            }
        }
    }

    public void ChangeBuilding()
    {
        ptr = (ptr + 1) % placedObjectTypeSos.Count;
        placedObjectTypeSo = placedObjectTypeSos[ptr];
    }

    /// <summary>
    /// 销毁格子上的建筑
    /// </summary>
    private void DestoryBuilding()
    {
        GridObject gridObject =
            grid.GetGridObject(PlayerInput.Instance.GetMouse3DPosition(LayerMask.GetMask("Default")));
        PlacedObject placedObject = gridObject.GetPlaceObject();
        if (placedObject != null)
        {
            List<Vector2Int> gridList =
                placedObject.GetGridPositionList();

            foreach (var gridPos in gridList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
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
        grid.GetXZ(mousePos, out int x, out int z);
        List<Vector2Int> gridList =
            placedObjectTypeSo.GetGridPositionList(new Vector2Int(x, z), dir);

        // 遍历所有占领的格子，只有全部可以建造才能建造
        bool canBuild = true;
        foreach (var gridPos in gridList)
        {
            if (!grid.GetGridObject(gridPos.x, gridPos.y).CanBuild())
            {
                canBuild = false;
                break;
            }
        }

        if (canBuild)
        {
            mapDataToJsonSystem.RecordBuild(mousePos);
            Vector2Int objRotationOffset = placedObjectTypeSo.GetRotationOffset(dir);
            Vector3 worldPos = grid.GetWorldPosition(x, z) +
                               new Vector3(objRotationOffset.x, 0, objRotationOffset.y) *
                               MapSystem.Instance.GetGrid().Cellsize;
            PlacedObject placedObject =
                PlacedObject.Create(worldPos, new Vector2Int(x, z), dir, placedObjectTypeSo);

            foreach (var gridPos in gridList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObect(placedObject);
            }
        }
        else
        {
            UtilsClass.CreateWorldTextPopup("Can't create here", mousePos);
        }
    }
}