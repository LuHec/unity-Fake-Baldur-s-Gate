using System;

public class GlobalState
{
    public bool TurnMode => turnMode;
    private bool turnMode = false;
    public bool EditMode => editMode;
    private bool editMode = false;

    public void SetTurnMode(bool val)
    {
        turnMode = val;
    }

    public void SetEditMode(bool val)
    {
        editMode = val;
    }
}