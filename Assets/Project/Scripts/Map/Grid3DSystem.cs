using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using LuHec.Utils;
using UnityEngine;
using UnityEngine.Serialization;

public class Grid3DSystem : MonoBehaviour
{
    private GridXZ<GridObject> _grid;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSos; 
    [SerializeField] private int gridwidth = 10;
    [SerializeField] private int gridheight = 10;
    [SerializeField] private float cellsize = 10f;
    [SerializeField] private PlacedObjectTypeSO.Dir _dir = PlacedObjectTypeSO.Dir.Up;
    
    private PlacedObjectTypeSO placedObjectTypeSo;
    private int _ptr = 0;

    void ChangeBuilding()
    {
        _ptr = (_ptr + 1) % placedObjectTypeSos.Count;
        placedObjectTypeSo = placedObjectTypeSos[_ptr];
    }

    private void Awake()
    {
        _grid = new GridXZ<GridObject>(gridwidth, gridheight, cellsize, Vector3.zero,
            (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y));

        placedObjectTypeSo = placedObjectTypeSos[0];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _dir = placedObjectTypeSo.GetNextDir(_dir);
            UtilsClass.CreateWorldTextPopup(_dir.ToString(), Utilties.GetMouse3DPosition("Default"));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeBuilding();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Utilties.GetMouse3DPosition("Default");
            GridObject gridObj = _grid.GetGridObject(pos);
            _grid.GetXZ(pos, out int x, out int z);
            List<Vector2Int> gridList =
                placedObjectTypeSo.GetGridPositionList(new Vector2Int(x, z), _dir);

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
                Transform buildTransform = Instantiate(placedObjectTypeSo.prefab, pos,
                    Quaternion.Euler(
                        new Vector3(0, placedObjectTypeSo.GetRotationAngle(_dir), 0))
                );
                
                Vector2Int objOffset = placedObjectTypeSo.GetRotationOffset(_dir);
                buildTransform.transform.position = _grid.GetWorldPosition(x, z) + new Vector3(objOffset.x, 0, objOffset.y)  * cellsize;

                foreach (var gridPos in gridList)
                {
                    _grid.GetGridObject(gridPos.x, gridPos.y).SetTransform(buildTransform);
                }
            }
            else
            {
                UtilsClass.CreateWorldTextPopup("Can't create here", pos);
            }
        }
    }

    public class GridObject
    {
        private GridXZ<GridObject> _grid;
        private int _x;
        private int _y;
        private Transform objTransform;

        public GridObject(GridXZ<GridObject> g, int x, int y)
        {
            _grid = g;
            _x = x;
            _y = y;
        }

        public bool CanBuild()
        {
            return objTransform == null;
        }

        public void SetTransform(Transform trans)
        {
            if (CanBuild())
            {
                objTransform = trans;
            }

            _grid.OnGridObjectChanged(_x, _y);
        }

        public void ClearTransform()
        {
            objTransform = null;
        }

        public override string ToString()
        {
            if (objTransform)
                return "object";
            return _x + "," + _y;
        }
    }
}