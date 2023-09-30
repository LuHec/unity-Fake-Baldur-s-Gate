using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid3DSystem : MonoBehaviour
{
    private GridXZ<GridObject> _grid;
    [SerializeField] private int gridwidth = 10;
    [SerializeField] private int gridheight = 10;
    [SerializeField] private float cellsize = 10f;

    private void Awake()
    {
        _grid = new GridXZ<GridObject>(gridwidth, gridheight, cellsize, Vector3.zero,
            (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y));
    }

    public class GridObject
    {
        private GridXZ<GridObject> _grid;
        private int _x;
        private int _y;

        public GridObject(GridXZ<GridObject> g, int x, int y)
        {
            _grid = g;
            _x = x;
            _y = y;
        }

        public override string ToString()
        {
            return _x + "," + _y;
        }
    }
}