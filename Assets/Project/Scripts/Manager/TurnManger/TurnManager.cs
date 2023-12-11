using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using Object = System.Object;

/// <summary>
/// 回合管理器
/// </summary>
public class TurnManager : Singleton<TurnManager>
{
    private HashSet<uint> globalFreeModeActorIdSet;

    private HashSet<TurnInstance> turnInstancesSet;
    private CommandCenter commandCenter;
    private ActorsManagerCenter actorsManagerCenter;
    private bool runTurn = true;
    private PlayerActorContainer playerActorContainer;

    /// <summary>
    /// 每一帧执行完后再删除或添加
    /// </summary>
    private List<TurnInstance> turnsNeedRemove;

    private TurnInstance turnNeedAdd;

    public EventHandler<int> onConCharaChanged;
    public int TurnCount => turnInstancesSet.Count;

    public void Init()
    {
        actorsManagerCenter = ActorsManagerCenter.Instance;
        commandCenter = CommandCenter.Instance;
        turnInstancesSet = new HashSet<TurnInstance>();
        globalFreeModeActorIdSet = new HashSet<uint>();
        turnsNeedRemove = new List<TurnInstance>();

        MessageCenter.Instance.SubmitUpdateTurn(OnMessageCenterUpdateTurn);
        MessageCenter.Instance.SubmitActorDie(OnActorDie);
    }

    public void InitActorContainer(List<uint> playerList)
    {
        playerActorContainer = new PlayerActorContainer(playerList);
        foreach (var id in playerList)
        {
            globalFreeModeActorIdSet.Add(id);
        }
    }


    public void SelectPlayer(int playerIdx)
    {
        playerActorContainer.ChangePlayerByIdx(playerIdx);
    }

    // /// <summary>
    // /// 加入新的回合。可能会有被重组的id，因此会进行去重
    // /// </summary>
    // /// <param name="conActorDynamic_id"></param>
    // public void AddTurn(List<uint> conActorDynamic_id)
    // {
    //     if (conActorDynamic_id.Count == 0) return;
    //
    //     foreach (uint id in conActorDynamic_id)
    //     {
    //         globalFreeModeActorIdSet.Remove(id);
    //     }
    //
    //     _turnInstancesSet.Add(new TurnInstance(conActorDynamic_id));
    // }
    //
    // public bool QueryFreeModeByActorId(uint id)
    // {
    //     return globalFreeModeActorIdSet.Contains(id);
    // }

    public uint GetCurrentPlayerId()
    {
        return playerActorContainer.GetCurrentPlayer;
    }

    public List<uint> GetAllPlayerIdList()
    {
        return playerActorContainer.PlayerActorsIdList;
    }

    /// <summary>
    /// 添加回合会将id从自由列表移除，并初始化角色的回合
    /// </summary>
    /// <param name="turnInstance"></param>
    /// <returns></returns>
    public bool AddTurn(TurnInstance turnInstance)
    {
        if (turnInstance == null) return false;
        foreach (var id in turnInstance.ConActorDynamicIDs)
        {
            globalFreeModeActorIdSet.Remove(id);
            ActorsManagerCenter.Instance.GetActorByDynamicId(id).InitTurnIntance(turnInstance);
        }

        return turnInstancesSet.Add(turnInstance);
    }

    public bool RemoveTurn(TurnInstance turnInstance)
    {
        foreach (uint id in turnInstance.ConActorDynamicIDSet)
        {
            actorsManagerCenter.GetActorByDynamicId(id).InitTurnIntance(null);
            globalFreeModeActorIdSet.Add(id);
        }

        return turnInstancesSet.Remove(turnInstance);
    }

    public bool RemoveActorByDynamicID(uint id, bool isDead = false)
    {
        foreach (var turn in turnInstancesSet)
        {
            if (turn.ConActorDynamicIDSet.Contains(id))
            {
                turn.RemoveActorByDynamicId(id);
                return true;
            }
        }

        if (!isDead)
        {
            globalFreeModeActorIdSet.Add(id);
        }

        return false;
    }

    public void RunTurn()
    {
        // 运行自由模式的actor
        RunFreeMode();

        // 运行回合制模式actor
        RunTurnMode();

        HandlerTurnRemove();
    }

    public bool AddFreeModeActorById(uint id)
    {
        return globalFreeModeActorIdSet.Add(id);
    }

    public bool RemoveFreeModeActorById(uint id)
    {
        return globalFreeModeActorIdSet.Remove(id);
    }

    private void RunFreeMode()
    {
        // Debug.Log(globalFreeModeActorIdSet.Count);
        foreach (var id in globalFreeModeActorIdSet)
        {
            var character = ActorsManagerCenter.Instance.GetActorByDynamicId(id) as Character;
            character.ActorUpdate();

            // 结束后要清除指令
            commandCenter.Excute(character.GetCommand(), character, () => { character.ClearCommandCache(); });

            // 可能会被合并回合打断
            if (runTurn == false)
            {
                runTurn = true;
                break;
            }
        }
    }

    private void RunTurnMode()
    {
        foreach (TurnInstance turnInstance in turnInstancesSet)
        {
            turnInstance.RunTurn();
        }
    }

    private void HandlerTurnRemove()
    {
        foreach (var turn in turnsNeedRemove)
        {
            RemoveTurn(turn);
        }

        turnsNeedRemove.Clear();

        if (turnNeedAdd != null)
        {
            AddTurn(turnNeedAdd);
            turnNeedAdd = null;
        }
    }

    #region #Listener

    /// <summary>
    /// 为了处理加入和移出自由列表的问题，统一设定为退出时清除回合，加入到自由列表
    /// 加入回合时退出自由列表，初始化回合
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    public void OnMessageCenterUpdateTurn(Object sender, EventArgsType.UpdateTurnManagerMessage message)
    {
        runTurn = false;
        // 如果有回合要移除，就要中断回合
        if (message.turnRemoveSet != null && message.turnRemoveSet.Count != 0)
        {
            Debug.Log("Remove Turn");

            foreach (var turnNeedRemove in message.turnRemoveSet)
            {
                turnsNeedRemove.Add(turnNeedRemove);
            }

            // foreach (var instance in message.turnRemoveSet)
            // {
            //     RemoveTurn(instance);
            // }

            // 如果移除后还有残余的，需要中断回合，防止迭代器出错
            // if(_turnInstancesSet.Count != 0) ForceQuitTurn();
        }

        // 如果回合不是原来就有的，就需要中断回合
        // if (AddTurn(message.newTurn))
        //     ForceQuitTurn();
        turnNeedAdd = message.newTurn;
    }

    private void ForceQuitTurn()
    {
        runTurn = false;
    }

    private void OnActorDie(Object sender, EventArgsType.ActorDieMessage message)
    {
        RemoveActorByDynamicID(message.dead_dynamic_id, true);
    }

    #endregion
}