using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 同一时间，一个actor只会存在一个TurnInstance中；如果需要同时存在，合并TurnInstance
/// </summary>
public class TurnInstance
{
    private CommandCenter _commandCenter;
    public HashSet<uint> _conActorDynamicIDSet;
    public List<uint> _conActorDynamicIDs;
    private ActorsManagerCenter _actorsManagerCenter;
    private int _turnActorPtr = 0;

    public int turnActorPtr => _turnActorPtr;
    public HashSet<uint> ConActorDynamicIDSet => _conActorDynamicIDSet;
    public List<uint> ConActorDynamicIDs => _conActorDynamicIDs;

    // enum State
    // {
    //     WaitCommand,
    //     RunCommand,
    // }
    //
    // private State _state = State.WaitCommand;

    public TurnInstance()
    {
    }

    public TurnInstance(ActorsManagerCenter actorsManagerCenter, CommandCenter commandCenter)
    {
        _actorsManagerCenter = actorsManagerCenter;
        _commandCenter = commandCenter;
        _conActorDynamicIDs = new List<uint>();
        _conActorDynamicIDSet = new HashSet<uint>();
    }

    public TurnInstance(ActorsManagerCenter actorsManagerCenter, CommandCenter commandCenter,
        List<uint> conActorDynamicIDs)
    {
        _actorsManagerCenter = actorsManagerCenter;
        _commandCenter = commandCenter;
        _conActorDynamicIDs = conActorDynamicIDs;

        _conActorDynamicIDSet = conActorDynamicIDs.ToHashSet();
        foreach (uint id in _conActorDynamicIDs)
        {
            _actorsManagerCenter.GetActorByDynamicId(id).InitTurnIntance(this);
        }
        // SortByActorSpeed();
    }

    /// <summary>
    /// 依据人物速度对回合进行排序
    /// </summary>
    public void SortByActorSpeed()
    {
        _conActorDynamicIDs.Sort((uint ida, uint idb) =>
        {
            return _actorsManagerCenter.GetActorByDynamicId(ida).speed >
                   _actorsManagerCenter.GetActorByDynamicId(idb).speed
                ? 1
                : -1;
        });
    }

    /// <summary>
    /// 回合加入新actor，同时为actor设置回合
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool AddActorByDynamicId(uint id)
    {
        if (_conActorDynamicIDSet.Add(id) == false) return false;

        _conActorDynamicIDs.Add(id);

        _actorsManagerCenter.GetActorByDynamicId(id).InitTurnIntance(this);

        return true;
    }

    private void PushPtr()
    {
        _turnActorPtr = (_turnActorPtr + 1) % _conActorDynamicIDs.Count;
    }

    private void BackPtr()
    {
    }

    public void NextTurn()
    {
        PushPtr();
    }

    public void BackTurn()
    {
        BackPtr();
    }

    void OnExcuteFinished()
    {
        NextTurn();
    }

    // void OnExcuteError()
    // {
    //     _state = State.WaitCommand;
    // }

    public void RunTurn()
    {
        Character character = _actorsManagerCenter.GetActorByDynamicId(_conActorDynamicIDs[turnActorPtr]) as Character;
        RunActorCommand(character);
    }

    // public void RunTurn()
    // {
    //     Character character = _actorsManagerCenter.GetActorByDynamicId(_conActorDynamicIDs[turnActorPtr]) as Character;
    //     if (character.GetActorStateTag() == ActorEnumType.ActorStateTag.Player)
    //     {
    //         switch (_state)
    //         {
    //             case State.WaitCommand:
    //             {
    //                 if (_commandCenter.GenInputCommandCache())
    //                 {
    //                     _state = State.RunCommand;
    //                     RunActorCommand(character);
    //                 }
    //
    //                 break;
    //             }
    //             case State.RunCommand:
    //             {
    //                 RunActorCommand(character);
    //                 break;
    //             }
    //         }
    //     }
    //     else if (character.GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
    //     {
    //         switch (_state)
    //         {
    //             case State.WaitCommand:
    //             {
    //                 _commandCenter.AddCommand(character.GenAICommand(), character);
    //                 _state = State.RunCommand;
    //                 RunActorCommand(character);
    //
    //                 break;
    //             }
    //             case State.RunCommand:
    //             {
    //                 RunActorCommand(character);
    //                 break;
    //             }
    //         }
    //     }
    // }

    public void RunActorCommand(Character character)
    {
        // _commandCenter.AddCommand(character.GenCommandInstance(), character);
        // 如果操作非法，或者没有操作，将会中断执行并且返回等待命令
        _commandCenter.Excute(character.GenCommandInstance(), character, () =>
        {
            OnExcuteFinished();
            character.ClearCommandCache();
        });
    }

    /// <summary>
    ///  寻找指定id删除actor
    /// </summary>
    /// <param name="id"></param>
    /// <param name="removeFromIDPool">是否执行全局删除actor</param>
    /// <returns></returns>
    public bool RemoveActorByDynamicId(uint id, bool removeFromIDPool = false)
    {
        var pos = _conActorDynamicIDs.FindIndex((uint f_id) => { return id == f_id; });
        if (pos == -1) return false;

        _conActorDynamicIDs.RemoveAt(pos);
        _conActorDynamicIDSet.Remove(id);
        // 如果删除位置小于指针，需要重定位
        if (pos < _turnActorPtr) BackPtr();

        // 是否执行全局删除
        if (removeFromIDPool) return _actorsManagerCenter.RemoveConActorByDynamicId(id);

        return true;
    }
}