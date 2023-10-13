public class PathNode
{
    private GridXZ<PathNode> _grid;
    public int x;
    public int y;

    // 起始点数
    public int gCost;

    // 到目标点的期望消耗点数
    public int hCost;

    // 总计点数
    public int fCost;

    // 上一个节点
    public PathNode cameFromNode;

    public bool Reachable
    {
        get { return _reachable; }
        set { _reachable = value; }
    }

    private bool _reachable = true;

    public PathNode(GridXZ<PathNode> grid, int x, int y)
    {
        _grid = grid;
        this.x = x;
        this.y = y;
    }


    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}