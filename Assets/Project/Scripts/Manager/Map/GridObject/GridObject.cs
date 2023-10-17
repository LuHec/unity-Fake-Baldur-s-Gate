public class GridObject
{
    private GridXZ<GridObject> _grid;
    private int _x;
    private int _y;
    public int X => _x;
    public int Y => _y;
    private GameActor _actor;
    private PlacedObject _placedObject;

    public bool Reachable => _actor == null;

    public GridObject cameFromNode;

    // 起始点数
    public int gCost;

    // 到目标点的期望消耗点数
    public int hCost;

    // 总计点数
    public int fCost;


    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }


    public GridObject(GridXZ<GridObject> grid, int x, int y)
    {
        _grid = grid;
        _x = x;
        _y = y;
    }

    public bool Empty()
    {
        return _actor == null;
    }

    public bool SetActor(GameActor actor)
    {
        if (Empty())
        {
            _actor = actor;
            _grid.OnGridObjectChanged(_x, _y);
            return true;
        }

        return false;
    }

    public void ClearActor()
    {
        _actor = null;
        _grid.OnGridObjectChanged(_x, _y);
    }

    public override string ToString()
    {
        if (_actor) return _actor.name;
        return _x + "," + _y;
    }

    #region #build

    public GameActor GetActor() => _actor;


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

    public PlacedObject GetPlaceObject() => _placedObject;

    #endregion
}