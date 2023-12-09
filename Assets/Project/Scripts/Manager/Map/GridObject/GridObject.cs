public class GridObject
{
    private GridXZ<GridObject> grid;
    private int x;
    private int y;
    public int X => x;
    public int Y => y;
    // private GameActor _actor;
    private PlacedObject placedObject;

    public bool Reachable => true;

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
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    // public bool Empty()
    // {
    //     return _actor == null;
    // }

    // public bool SetActor(GameActor actor)
    // {
    //     if (Empty())
    //     {
    //         _actor = actor;
    //         _grid.OnGridObjectChanged(_x, _y);
    //         return true;
    //     }
    //
    //     return false;
    // }

    // public void ClearActor()
    // {
    //     _actor = null;
    //     grid.OnGridObjectChanged(x, y);
    // }

    public override string ToString()
    {
        // if (_actor) return _actor.name;
        return x + "," + y;
    }

    #region #build

    // public GameActor GetActor() => _actor;


    public bool CanBuild()
    {
        return placedObject == null;
    }

    public void SetPlacedObect(PlacedObject placedObject)
    {
        if (CanBuild())
        {
            this.placedObject = placedObject;
        }

        grid.OnGridObjectChanged(x, y);
    }

    public void ClearPlacedObject()
    {
        placedObject = null;
    }

    public PlacedObject GetPlaceObject() => placedObject;

    #endregion
}