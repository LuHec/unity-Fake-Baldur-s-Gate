using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridObject
{
    private GridXZ<GridObject> _grid;
    private int _x;
    private int _y;
    private PlacedObject _placedObject;

    public GridObject(GridXZ<GridObject> g, int x, int y)
    {
        _grid = g;
        _x = x;
        _y = y;
    }

    public bool CanBuild()
    {
        return _placedObject == null;
    }

    public void SetPlacedObect(PlacedObject placedObject)
    {
        if (CanBuild())
        {
            _placedObject = placedObject;
        }

        _grid.OnGridObjectChanged(_x, _y);
    }

    public void ClearPlacedObject()
    {
        _placedObject = null;
    }

    public override string ToString()
    {
        if (_placedObject)
            return "object";
        return _x + "," + _y;
    }

    public PlacedObject GetPlaceObject() => _placedObject;
}