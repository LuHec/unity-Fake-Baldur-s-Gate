public class GlobalState
{
    public bool TurnMode => _turnMode;
    private bool _turnMode = false;

    public void SetTurnMode(bool val)
    {
        _turnMode = val;
    }
}