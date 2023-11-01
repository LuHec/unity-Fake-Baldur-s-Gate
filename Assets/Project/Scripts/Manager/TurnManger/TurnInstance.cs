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

    #region #Tag

    public bool IsGameModeTurn => _isGameModeTurn;
    private bool _isGameModeTurn = false;

    #endregion

    #region #delgate

    private EventHandler<EventArgsType.TurnNeedRemoveMessage> TurnNeedRemoveHandler;

    private void InitListen()
    {
        _commandCenter = CommandCenter.Instance;
        _actorsManagerCenter = ActorsManagerCenter.Instance;
        MessageCenter.Instance.ListenOnTurnNeedRemove(ref TurnNeedRemoveHandler);
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isGameModeTurn">是否为手动切换的gamemode，缺省false</param>
    public TurnInstance(bool isGameModeTurn = false)
    {
        _isGameModeTurn = isGameModeTurn;

        InitListen();

        _conActorDynamicIDs = new List<uint>();
        _conActorDynamicIDSet = new HashSet<uint>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="conActorDynamicIDs"></param>
    /// <param name="isGameModeTurn">是否为手动切换的gamemode，缺省false</param>
    public TurnInstance(List<uint> conActorDynamicIDs, bool isGameModeTurn = false)
    {
        _isGameModeTurn = isGameModeTurn;

        InitListen();

        _conActorDynamicIDs = new List<uint>();
        _conActorDynamicIDSet = new HashSet<uint>();

        // 添加id，需要从自由列表中移除
        foreach (uint id in conActorDynamicIDs)
        {
            _actorsManagerCenter.GetActorByDynamicId(id).InitTurnIntance(this);
            AddActorByDynamicId(id);
        }
    }

    public void SetGameTurnMode(bool isGameModeTurn)
    {
        _isGameModeTurn = isGameModeTurn;
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

        // 如果在自由模式中需要退出来
        TurnManager.Instance.RemoveFreeModeActorById(id);

        return true;
    }

    private void PushPtr()
    {
        _turnActorPtr = (_turnActorPtr + 1) % _conActorDynamicIDs.Count;
    }

    private void BackPtr()
    {
        _turnActorPtr -= 1;
        if (_turnActorPtr < 0) _turnActorPtr = _conActorDynamicIDs.Count - 1;
    }

    public void NextTurn()
    {
        PushPtr();
    }

    public void BackTurn()
    {
        BackPtr();

        foreach (var uid in _conActorDynamicIDs)
        {
            Character character = ActorsManagerCenter.Instance.GetActorByDynamicId(uid) as Character;
            character.CmdQue.Undo();
        }
    }

    void OnExcuteFinished()
    {
        NextTurn();
    }


    public void RunTurn()
    {
        CheckTurn();
        Character character = _actorsManagerCenter.GetActorByDynamicId(_conActorDynamicIDs[turnActorPtr]) as Character;
        RunActorCommand(character);
    }

    /// <summary>
    /// 检查回合是否需要被移除
    /// </summary>
    private void CheckTurn()
    {
        // 手动切换的不用检查
        if (IsGameModeTurn) return;

        // 只剩下一个就要移除
        if (ConActorDynamicIDSet.Count == 1)
        {
            TurnNeedRemoveHandler(this, new EventArgsType.TurnNeedRemoveMessage(this));
            return;
        }

        // 全是玩家就要移除
        bool needRemove = true;
        foreach (var id in ConActorDynamicIDSet)
        {
            if (_actorsManagerCenter.GetActorByDynamicId(id).GetActorStateTag() != ActorEnumType.ActorStateTag.Player)
            {
                if ((_actorsManagerCenter.GetActorByDynamicId(id) as Character).GetCharacterType() !=
                    ActorEnumType.AIMode.Follow)
                {
                    needRemove = false;
                    break;
                }
            }
        }

        if (needRemove) TurnNeedRemoveHandler(this, new EventArgsType.TurnNeedRemoveMessage(this));
    }

    private void RunActorCommand(Character character)
    {
        _commandCenter.AddCommand(character.GenCommandInstance(), character);
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
    public bool RemoveActorByDynamicId(uint id)
    {
        var pos = _conActorDynamicIDs.FindIndex((uint f_id) => { return id == f_id; });
        if (pos == -1) return false;

        _conActorDynamicIDs.RemoveAt(pos);
        _conActorDynamicIDSet.Remove(id);
        // 如果删除位置小于指针，需要重定位
        if (pos < _turnActorPtr) BackPtr();

        return true;
    }

    /// <summary>
    /// 回合结束后把角色移出回合模式
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool RemoveActorToFreeMode(uint id)
    {
        var pos = _conActorDynamicIDs.FindIndex((uint f_id) => { return id == f_id; });
        if (pos == -1) return false;

        _conActorDynamicIDs.RemoveAt(pos);
        _conActorDynamicIDSet.Remove(id);
        // 如果删除位置小于指针，需要重定位往后退一格
        if (pos < _turnActorPtr) BackPtr();

        return true;
    }
}