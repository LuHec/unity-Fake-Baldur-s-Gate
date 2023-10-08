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
            (GridXZ<GridObject> g, int x, int y)=>new GridObject(g, x, y));
    }

    public void CenterUpdate()
    {
        
    }
}