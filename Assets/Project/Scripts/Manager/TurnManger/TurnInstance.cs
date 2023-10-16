using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 同一时间，一个actor只会存在一个TurnInstance中；如果需要同时存在，合并TurnInstance
/// </summary>
public class TurnInstance
{
    private CommandCenter _commandCenter;

    private HashSet<uint> _conActorIDSet;
    private List<uint> _conActorDynamicIDs;

    private int _turnActorPtr = 0;
    private ActorsManagerCenter _actorsManagerCenter;

    private bool _battleMode = false;

    public int turnActorPtr => _turnActorPtr;
    public HashSet<uint> conActorIDSet => _conActorIDSet;

    enum State
    {
        WaitCommand,
        RunCommand,
    }

    private State _state = State.WaitCommand;

    public TurnInstance(ActorsManagerCenter actorsManagerCenter, CommandCenter commandCenter,
        List<uint> conActorDynamicIDs)
    {
        _actorsManagerCenter = actorsManagerCenter;
        _commandCenter = commandCenter;
        _conActorDynamicIDs = conActorDynamicIDs;
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
        _state = State.WaitCommand;
        NextTurn();
    }

    void OnExcuteError()
    {
        _state = State.WaitCommand;
    }

    public void RunTurn()
    {
        Character character = _actorsManagerCenter.GetActorByDynamicId(_conActorDynamicIDs[turnActorPtr]) as Character;
        if (character.GetActorStateTag() == ActorEnumType.ActorStateTag.Player)
        {
            switch (_state)
            {
                case State.WaitCommand:
                {
                    if (_commandCenter.GenInputCommandCache())
                    {
                        _state = State.RunCommand;
                        RunActorCommand(character);
                    }

                    break;
                }
                case State.RunCommand:
                {
                    RunActorCommand(character);
                    break;
                }
            }
        }
        else if (character.GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
        {
            switch (_state)
            {
                case State.WaitCommand:
                {
                    _commandCenter.AddCommand(character.GenAICommand(), character);
                    _state = State.RunCommand;
                    RunActorCommand(character);

                    break;
                }
                case State.RunCommand:
                {
                    RunActorCommand(character);
                    break;
                }
            }
        }
    }

    public void RunActorCommand(GameActor actor)
    {
        _commandCenter.AddCommand(_commandCenter.GetCommandCache(), actor);
        // 如果操作非法，或者没有操作，将会中断执行并且返回等待命令
        if (_commandCenter.Excute(actor.GetCommand(), actor, OnExcuteFinished) == false)
        {
            OnExcuteError();
        }
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
        _conActorIDSet.Remove(id);
        // 如果删除位置小于指针，需要重定位
        if (pos < _turnActorPtr) BackPtr();

        // 是否执行全局删除
        if (removeFromIDPool) return _actorsManagerCenter.RemoveConActorByDynamicId(id);

        return true;
    }
}