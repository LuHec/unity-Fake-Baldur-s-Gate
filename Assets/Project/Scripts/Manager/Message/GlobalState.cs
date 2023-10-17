public class GlobalState
{
    public bool TurnMode => _turnMode;
    private bool _turnMode = false;
    public bool EditMode => _editMode;
    private bool _editMode = true;

    public void SetTurnMode(bool val)
    {
        _turnMode = val;
    }

    public void SetEditMode(bool val)
    {
        _editMode = val;
    }
}