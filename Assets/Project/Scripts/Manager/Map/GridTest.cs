using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    private GridXZ<GridObject> _gridXZ;

    private void Start()
    {
        _gridXZ = new GridXZ<GridObject>(20, 10, 10, new Vector3(-5, 0),
            (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y, false));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _gridXZ.GetGridObject(UtilsClass.GetMouseWorldPosition()).SetValue();
        }
    }

    public class GridObject
    {
        private GridXZ<GridObject> _gridXZ;
        private bool _val;
        private int _x;
        private int _y;

        public GridObject(GridXZ<GridObject> g, int x, int y, bool val)
        {
            _gridXZ = g;
            _x = x;
            _y = y;
            _val = val;
        }

        public void SetValue()
        {
            _val = !_val;
            _gridXZ.OnGridObjectChanged(_x, _y);
        }

        public override string ToString()
        {
            return _val.ToString();
        }
    }
}