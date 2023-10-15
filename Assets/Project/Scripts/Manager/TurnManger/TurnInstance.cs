using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 同一时间，一个actor只会存在一个TurnInstance中；如果需要同时存在，合并TurnInstance
/// </summary>
public class TurnInstance
{
    public GameTurn.Turn NowTurn => _turn;
    private bool _playerControlledTurn;
    private GameTurn.Turn _turn = GameTurn.Turn.PlayerTurn;
    private CommandCenter _commandCenter;

    private List<uint> _conActorDynamicIDs;
    private int _turnActorPtr = 0;
    private ActorsManagerCenter _actorsManagerCenter;

    public TurnInstance(ActorsManagerCenter actorsManagerCenter, CommandCenter commandCenter, List<uint> conActorDynamicIDs)
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

    // public void RunTurn(Action onExcuteFinished, Action onExcuteError)
    // {
    //     if (NowTurn == GameTurn.Turn.PlayerTurn)
    //     {
    //         if (_conActorDynamicIDs != null)
    //         {
    //             GameActor actor = _playerControlledActors[_playerControlledPtr];
    //             _commandCenter.AddCommand(_commandCenter.GetCommandCache(), actor);
    //
    //             // 如果操作非法，将会中断执行并且返回等待命令
    //             if (_commandCenter.Excute(actor.GetCommand(), actor, onExcuteFinished) == false)
    //             {
    //                 onExcuteError();
    //             }
    //         }
    //     }
    // }
    
    public void RunTurn(Action onExcuteFinished, Action onExcuteError)
    {
        if (NowTurn == GameTurn.Turn.PlayerTurn)
        {
            if (_conActorDynamicIDs != null)
            {
                uint actorDynamicId = _conActorDynamicIDs[_turnActorPtr];
                GameActor actor = _actorsManagerCenter.GetActorByDynamicId(actorDynamicId);
                _commandCenter.AddCommand(_commandCenter.GetCommandCache(), actor);
                // 如果操作非法，或者没有操作，将会中断执行并且返回等待命令
                if (_commandCenter.Excute(actor.GetCommand(), actor, onExcuteFinished) == false)
                {
                    onExcuteError();
                }
            }
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

        // 如果删除位置小于指针，需要重定位
        if (pos < _turnActorPtr) BackPtr();

        // 是否执行全局删除
        if (removeFromIDPool) return _actorsManagerCenter.RemoveConActorByDynamicId(id);

        return true;
    }
}