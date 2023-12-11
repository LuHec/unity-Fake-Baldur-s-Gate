using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 同一时间，一个actor只会存在一个TurnInstance中；如果需要同时存在，合并TurnInstance
/// </summary>
public class TurnInstance
{
    private CommandCenter commandCenter;
    private HashSet<uint> conActorDynamicIDSet;
    private List<uint> conActorDynamicIDs;
    private ActorsManagerCenter actorsManagerCenter;
    private int turnActorPtr = 0;

    // 实时参数
    public int TurnActorPtr => turnActorPtr;
    public uint CurrentActorId => conActorDynamicIDs[TurnActorPtr];
    public HashSet<uint> ConActorDynamicIDSet => conActorDynamicIDSet;
    public List<uint> ConActorDynamicIDs => conActorDynamicIDs;

    #region #Tag

    public bool IsGameModeTurn => isGameModeTurn;
    private bool isGameModeTurn = false;

    #endregion

    #region #delgate

    private EventHandler<EventArgsType.TurnNeedRemoveMessage> TurnNeedRemoveHandler;

    private void InitListen()
    {
        commandCenter = CommandCenter.Instance;
        actorsManagerCenter = ActorsManagerCenter.Instance;
        MessageCenter.Instance.ListenOnTurnNeedRemove(ref TurnNeedRemoveHandler);
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isGameModeTurn">是否为手动切换的gamemode，缺省false</param>
    public TurnInstance(bool isGameModeTurn = false)
    {
        this.isGameModeTurn = isGameModeTurn;

        InitListen();

        conActorDynamicIDs = new List<uint>();
        conActorDynamicIDSet = new HashSet<uint>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="conActorDynamicIDs"></param>
    /// <param name="isGameModeTurn">是否为手动切换的gamemode，缺省false</param>
    public TurnInstance(List<uint> conActorDynamicIDs, bool isGameModeTurn = false)
    {
        this.isGameModeTurn = isGameModeTurn;

        InitListen();

        this.conActorDynamicIDs = new List<uint>();
        conActorDynamicIDSet = new HashSet<uint>();

        // 添加id，需要从自由列表中移除
        foreach (uint id in conActorDynamicIDs)
        {
            actorsManagerCenter.GetActorByDynamicId(id).InitTurnIntance(this);
            AddActorByDynamicId(id);
        }
    }

    public void SetGameTurnMode(bool isGameModeTurn)
    {
        this.isGameModeTurn = isGameModeTurn;
    }

    /// <summary>
    /// 依据人物速度对回合进行排序
    /// </summary>
    public void SortByActorSpeed()
    {
        conActorDynamicIDs.Sort((uint ida, uint idb) =>
        {
            return actorsManagerCenter.GetActorByDynamicId(ida).speed >
                   actorsManagerCenter.GetActorByDynamicId(idb).speed
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
        if (conActorDynamicIDSet.Add(id) == false) return false;

        conActorDynamicIDs.Add(id);
        actorsManagerCenter.GetActorByDynamicId(id).InitTurnIntance(this);

        // 如果在自由模式中需要退出来
        TurnManager.Instance.RemoveFreeModeActorById(id);

        return true;
    }

    private void PushPtr()
    {
        turnActorPtr = (turnActorPtr + 1) % conActorDynamicIDs.Count;
    }

    private void BackPtr()
    {
        turnActorPtr -= 1;
        if (turnActorPtr < 0) turnActorPtr = conActorDynamicIDs.Count - 1;
    }

    public void NextTurn()
    {
        PushPtr();
    }

    public void BackTurn()
    {
        BackPtr();

        foreach (var uid in conActorDynamicIDs)
        {
            Character character = ActorsManagerCenter.Instance.GetActorByDynamicId(uid) as Character;
            character.CmdQue.Undo();
        }
    }

    public void RunTurn()
    {
        CheckTurn();
        Character character = actorsManagerCenter.GetActorByDynamicId(conActorDynamicIDs[TurnActorPtr]) as Character;
        character.ActorUpdate();
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
            if (actorsManagerCenter.GetActorByDynamicId(id).GetActorStateTag() != ActorEnumType.ActorStateTag.Player)
            {
                if ((actorsManagerCenter.GetActorByDynamicId(id) as Character).GetCharacterType() !=
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
        // AI和玩家分开处理，玩家需要等待命令
        var command = character.GetCommand();
        if (command == null) return;
        
        if (command.IsRunning)
        {
            commandCenter.Excute(command, character, null);
        }
        else
        {
            character.ClearCommandCache();
            NextTurn();
        }
    }

    /// <summary>
    ///  寻找指定id删除actor
    /// </summary>
    /// <param name="id"></param>
    /// <param name="removeFromIDPool">是否执行全局删除actor</param>
    /// <returns></returns>
    public bool RemoveActorByDynamicId(uint id)
    {
        var pos = conActorDynamicIDs.FindIndex((uint f_id) => { return id == f_id; });
        if (pos == -1) return false;

        conActorDynamicIDs.RemoveAt(pos);
        conActorDynamicIDSet.Remove(id);
        // 如果删除位置小于指针，需要重定位
        if (pos < turnActorPtr) BackPtr();

        return true;
    }

    void OnExcuteFinished()
    {
        NextTurn();
    }
}