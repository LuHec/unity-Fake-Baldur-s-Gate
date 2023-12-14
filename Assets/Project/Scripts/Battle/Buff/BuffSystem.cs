using System;
using System.Collections.Generic;

public class BuffSystem
{
    private List<Buff> buffs;
    private Action onTurnEnter;
    private Action onTurnExit;
    public Action onActorDie;

    public BuffSystem()
    {
        buffs = new List<Buff>();
        AddBuff(new TestBuff());
    }

    public void AddBuff(Buff buff)
    {
        buffs.Add(buff);

        if (buff is IBuffTurnEnter enterBuff)
        {
            onTurnEnter += enterBuff.OnTurnEnter;
        }

        if (buff is IBuffTurnExit exitBuff)
        {
            onTurnExit += exitBuff.OnTurnExit;
        }
    }


    public void OnTurnEnter()
    {
        onTurnEnter();
    }

    public void OnTurnExit()
    {
        onTurnExit();
    }
}