using UnityEngine;

public class TestBuff : Buff, IBuffTurnEnter, IBuffTurnExit
{
    public void OnTurnEnter()
    {
        Debug.Log("Enter Turn");
    }

    public void OnTurnExit()
    {
        Debug.Log("Exit Turn");
    }
}